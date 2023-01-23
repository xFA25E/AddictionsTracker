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
        var addictions = new Dictionary<string, Addiction>();

        ExecuteReader(
            Queries.GetAddictions,
            reader =>
            {
                while (reader.Read())
                {
                    var title = reader.GetString(0);
                    addictions.Add(title, new Addiction(title));
                };
            }
        );

        ExecuteReader(
            Queries.GetFailures,
            reader =>
            {
                while (reader.Read())
                {
                    var failedAt = reader.GetDateTime(0);
                    var note = reader.IsDBNull(1) ? null : reader.GetString(1);
                    var failure = new Failure(failedAt, note);

                    var addictionTitle = reader.GetString(2);
                    addictions[addictionTitle].Failures.Add(failure);
                }
            }
        );

        return addictions.Values;
    }

    public void InsertAddiction(string addictionTitle)
    {
        ExecuteNonQuery(
            Queries.InsertAddiction,
            ("$addiction_title", addictionTitle)
        );
    }

    public void UpdateAddiction(string addictionTitle, string newAddictionTitle)
    {
        ExecuteNonQuery(
            Queries.UpdateAddiction,
            ("$addiction_title", addictionTitle),
            ("$new_addiction_title", newAddictionTitle)
        );
    }

    public void DeleteAddiction(string addictionTitle)
    {
        ExecuteNonQuery(
            Queries.DeleteAddiction,
            ("$addiction_title", addictionTitle)
        );
    }

    public void InsertFailure(
        string addictionTitle,
        DateTime failedAt,
        string? note = null
    )
    {
        ExecuteNonQuery(
            Queries.InsertFailure,
            ("$addiction_title", addictionTitle),
            ("$failed_at", failedAt),
            ("$note", note)
        );
    }

    public void UpdateFailure(
        string addictionTitle,
        DateTime failedAt,
        string? newNote = null,
        DateTime? newFailedAt = null
    )
    {
        ExecuteNonQuery(
            Queries.UpdateFailure,
            ("$addiction_title", addictionTitle),
            ("$failed_at", failedAt),
            ("$new_failed_at", newFailedAt),
            ("$new_note", newNote)
        );
    }

    public void DeleteFailure(
        string addictionTitle,
        DateTime failedAt
    )
    {
        ExecuteNonQuery(
            Queries.DeleteFailure,
            ("$addiction_title", addictionTitle),
            ("$failed_at", failedAt)
        );
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
        Action<SqliteDataReader> callback
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
            }
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
  addiction_title TEXT UNIQUE NOT NULL
);

CREATE TABLE IF NOT EXISTS failure (
  addiction_id INTEGER NOT NULL,
  failed_at DATETIME NOT NULL,
  note TEXT,
  PRIMARY KEY (failed_at, addiction_id),
  FOREIGN KEY (addiction_id)
  REFERENCES addiction (addiction_id)
  ON DELETE CASCADE
  ON UPDATE CASCADE
);
";

        public static readonly string InsertAddiction = @"
INSERT INTO addiction (addiction_title) VALUES ($addiction_title);
";

        public static readonly string UpdateAddiction = @"
UPDATE addiction
   SET addiction_title = $new_addiction_title
 WHERE addiction_title = $addiction_title
";

        public static readonly string DeleteAddiction = @"
DELETE FROM addiction WHERE addiction_title = $addiction_title
";

        public static readonly string InsertFailure = @"
INSERT INTO failure (addiction_id, failed_at, note)
VALUES ((SELECT addiction_id
           FROM addiction
          WHERE addiction_title = $addiction_title),
          $failed_at,
          $note);
";

        public static readonly string UpdateFailure = @"
UPDATE failure
   SET failed_at = COALESCE($new_failed_at, failed_at),
       note = COALESCE($new_note, note)
 WHERE failed_at = $failed_at
   AND addiction_id = (SELECT addiction_id
                         FROM addiction
                        WHERE addiction_title = $addiction_title)
";

        public static readonly string DeleteFailure = @"
DELETE FROM failure
 WHERE failed_at = $failed_at
   AND addiction_id = (SELECT addiction_id
                         FROM addiction
                        WHERE addiction_title = $addiction_title)
";

        public static readonly string GetAddictions = @"
SELECT addiction_title FROM addiction;
";

        public static readonly string GetFailures = @"
SELECT failed_at, note, addiction_title
  FROM failure
       LEFT JOIN addiction USING(addiction_id);
";
    }
}
