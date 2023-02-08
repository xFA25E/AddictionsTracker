using System;
using AddictionsTracker.Models;

namespace AddictionsTracker;

public static class Globals
{
    public static readonly DateOnly Now = DateTime.Now.ToDateOnly();
    public static readonly DayWidth DayWidth = new DayWidth();
}
