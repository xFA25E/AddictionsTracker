using AddictionsTracker.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.IO;
using System;

namespace AddictionsTracker.Services;

public class Database
{
    string connectionString = GetConnectionString();
    SqliteConnection GetConnection() => new SqliteConnection(connectionString);

    public Database()
    {
        ExecuteNonQuery(Queries.CreateTables);
    }

    public IEnumerable<Addiction> GetAddictions()
    {
        var addictions = new Dictionary<int, Addiction>();

        ExecuteReader(
            Queries.GetAddictions,
            reader =>
            {
                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var title = reader.GetString(1);
                    addictions.Add(id, new Addiction(id, title));
                };
            }
        );

        ExecuteReader(
            Queries.GetFailures,
            reader =>
            {
                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var failedAt = reader.GetDateTime(1);
                    var note = reader.GetString(2);
                    var addictionId = reader.GetInt32(3);

                    var failure = new Failure(id, failedAt, note);
                    addictions[addictionId].Failures.Add(failure);
                }
            }
        );

        return addictions.Values;
    }

    public Addiction InsertAddiction(string addictionTitle)
    {
        int? addictionId = null;
        ExecuteReader(
            Queries.InsertAddiction,
            reader =>
            {
                reader.Read();
                addictionId = reader.GetInt32(0);
            },
            ("$addiction_title", addictionTitle)
        );

        return new Addiction(addictionId.Value, addictionTitle);
    }

    public void UpdateAddiction(Addiction addiction, string addictionTitle)
    {
        ExecuteNonQuery(
            Queries.UpdateAddiction,
            ("$addiction_id", addiction.Id),
            ("$addiction_title", addictionTitle)
        );
    }

    public void DeleteAddiction(Addiction addiction)
    {
        ExecuteNonQuery(
            Queries.DeleteAddiction,
            ("$addiction_id", addiction.Id)
        );
    }

    public Failure InsertFailure(Addiction addiction, DateTime failedAt, string note)
    {
        int? failureId = null;
        ExecuteReader(
            Queries.InsertFailure,
            reader =>
            {
                reader.Read();
                failureId = reader.GetInt32(0);
            },
            ("$addiction_id", addiction.Id),
            ("$failed_at", failedAt),
            ("$note", note)
        );

        return new Failure(failureId.Value, failedAt, note);
    }

    public void UpdateFailure(
        Failure failure,
        DateTime? failedAt = null,
        string? note = null
    )
    {
        ExecuteNonQuery(
            Queries.UpdateFailure,
            ("$failure_id", failure.Id),
            ("$failed_at", failedAt),
            ("$note", note)
        );
    }

    public void DeleteFailure(Failure failure)
    {
        ExecuteNonQuery(Queries.DeleteFailure, ("$failure_id", failure.Id));
    }

    void ExecuteNonQuery(
        string commandText,
        params (string, object?)[] parameters
    )
    {
        ExecuteCommand(
            commandText,
            command => command.ExecuteNonQuery(),
            parameters
        );
    }

    void ExecuteReader(
        string commandText,
        Action<SqliteDataReader> callback,
        params (string, object?)[] parameters
    )
    {
        ExecuteCommand(
            commandText,
            command =>
            {
                using (var reader = command.ExecuteReader())
                {
                    callback(reader);
                }
            },
            parameters
        );
    }

    void ExecuteCommand(
        string commandText,
        Action<SqliteCommand> callback,
        params (string, object?)[] parameters
    )
    {
        using (var conn = GetConnection())
        {
            conn.Open();
            var command = conn.CreateCommand();
            command.CommandText = commandText;
            foreach (var (parameterName, value) in parameters)
            {
                command.Parameters.AddWithValue(
                    parameterName,
                    value ?? DBNull.Value
                );
            }
            callback(command);
        }
    }

    static string GetConnectionString()
    {
        var dataDirectory = Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData,
            Environment.SpecialFolderOption.Create
        );

        var connBuilder = new SqliteConnectionStringBuilder()
        {
            DataSource = Path.Combine(dataDirectory, "AddictionsTracker.db")
        };

        return connBuilder.ToString();
    }

    static class Queries
    {
        public static readonly string CreateTables = @"
PRAGMA foreign_keys = ON;

CREATE TABLE IF NOT EXISTS addiction (
  addiction_id INTEGER PRIMARY KEY,
  addiction_title TEXT UNIQUE NOT NULL CHECK(LENGTH(addiction_title) <> 0)
);

CREATE TABLE IF NOT EXISTS failure (
  failure_id INTEGER PRIMARY KEY,
  failed_at DATETIME NOT NULL,
  note TEXT NOT NULL,
  addiction_id INTEGER NOT NULL,
  UNIQUE (addiction_id, failed_at),
  FOREIGN KEY (addiction_id)
  REFERENCES addiction (addiction_id)
  ON DELETE CASCADE
  ON UPDATE CASCADE
);

CREATE TRIGGER IF NOT EXISTS failure_failed_at_insert_constraint
  BEFORE INSERT
  ON failure
  FOR EACH ROW
    WHEN TYPEOF(NEW.failed_at) <> 'text'
    OR unixepoch(NEW.failed_at) IS NULL
    OR unixepoch(NEW.failed_at) % 60 <> 0
    BEGIN
      SELECT RAISE(ABORT, 'failed_at must be a valid iso8601 string and seconds must be zero');
    END;

CREATE TRIGGER IF NOT EXISTS failure_failed_at_update_constraint
  BEFORE UPDATE
  OF failed_at
  ON failure
  FOR EACH ROW
    WHEN TYPEOF(NEW.failed_at) <> 'text'
    OR unixepoch(NEW.failed_at) IS NULL
    OR unixepoch(NEW.failed_at) % 60 <> 0
    BEGIN
      SELECT RAISE(ABORT, 'failed_at must be a valid iso8601 string and seconds must be zero');
    END;
";

        public static readonly string InsertAddiction = @"
INSERT INTO addiction (addiction_title) VALUES ($addiction_title);

SELECT last_insert_rowid();
";

        public static readonly string UpdateAddiction = @"
UPDATE addiction
   SET addiction_title = $addiction_title
 WHERE addiction_id = $addiction_id;
";

        public static readonly string DeleteAddiction = @"
DELETE FROM addiction WHERE addiction_id = $addiction_id;
";

        public static readonly string InsertFailure = @"
INSERT INTO failure (addiction_id, failed_at, note)
VALUES ($addiction_id, $failed_at, $note);

SELECT last_insert_rowid();
";

        public static readonly string UpdateFailure = @"
UPDATE failure
   SET failed_at = COALESCE($failed_at, failed_at),
       note = COALESCE($note, note)
 WHERE failure_id = $failure_id;
";

        public static readonly string DeleteFailure = @"
DELETE FROM failure WHERE failure_id = $failure_id;
";

        public static readonly string GetAddictions = @"
SELECT addiction_id, addiction_title FROM addiction;
";

        public static readonly string GetFailures = @"
SELECT failure_id, failed_at, note, addiction_id
  FROM failure
       LEFT JOIN addiction USING(addiction_id);
";
    }
}
