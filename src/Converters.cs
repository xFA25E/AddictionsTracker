using System.Linq;
using Avalonia.Data.Converters;

namespace AddictionsTracker;

public class Converters
{
    public static readonly IMultiValueConverter Multiply =
        new FuncMultiValueConverter<int, int>(i => i.Aggregate((a, e) => a * e));
}
