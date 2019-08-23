﻿using System;
using System.Text.RegularExpressions;

namespace PhilipDaubmeier.TimeseriesHostCommon.Parsers
{
    public static class TimeIntervalParser
    {
        public static TimeSpan ToTimeSpan(this string interval)
        {
            var match = Regex.Match(interval, @"^((?<h>\d+)h)?((?<m>\d+)m)?((?<s>\d+)s)?((?<ms>\d+)ms)?$",
                RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.RightToLeft);

            var h = match.Groups["h"].Success ? int.Parse(match.Groups["h"].Value) : 0;
            var m = match.Groups["m"].Success ? int.Parse(match.Groups["m"].Value) : 0;
            var s = match.Groups["s"].Success ? int.Parse(match.Groups["s"].Value) : 0;
            var ms = match.Groups["ms"].Success ? int.Parse(match.Groups["ms"].Value) : 0;

            return new TimeSpan(0, h, m, s, ms);
        }
    }
}