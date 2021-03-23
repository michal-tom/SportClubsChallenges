namespace SportClubsChallenges.Utils.Helpers
{
    using System;

    public static class TimeHelper
    {
        public static DateTime GetStartOfWeek(DayOfWeek? startOfWeek = DayOfWeek.Monday)
        {
            return DateTime.Today.AddDays(-(DateTime.Today.DayOfWeek - startOfWeek.Value));
        }
    }
}