namespace SportClubsChallenges.Utils.Helpers
{
    using System;

    public static class TimeHelper
    {
        public static DateTime GetStartOfWeek(DayOfWeek? startOfWeek = DayOfWeek.Monday)
        {
            return DateTime.Today.AddDays(-(DateTime.Today.DayOfWeek - startOfWeek.Value));
        }

        public static int GetPercentageProgress(DateTime startDate, DateTime endDate)
        {
            if (startDate.Date >= DateTime.Today)
            {
                return 0;
            }

            if (endDate.Date < DateTime.Today)
            {
                return 100;
            }

            var totalDuration = endDate.Date.Subtract(startDate.Date);
            if (totalDuration.Days == 0)
            {
                return 0;
            }

            var currentDuration = DateTime.Today.Subtract(startDate.Date);

            return (int) ((100 * currentDuration) / totalDuration);
        }

        public static string GetChallengeDatePeriodDescription(DateTime startDate, DateTime endDate)
        {
            if (startDate.Date > DateTime.Today)
            {
                var daysToStart = startDate.Date.Subtract(DateTime.Today).TotalDays;
                if (daysToStart == 1)
                {
                    return $"Challenge will start tomorrow";
                }
                else
                {
                    return $"Challenge will start in {daysToStart} days";
                }
            }

            if (endDate.Date < DateTime.Today)
            {
                return $"Challenge has already ended";
            }

            var daysToEnd = endDate.Date.Subtract(DateTime.Today).TotalDays;
            if (daysToEnd == 0)
            {
                return $"Challenge ends today";
            }
            else
            {
                return $"Challenge will end in {daysToEnd} days";
            }
        }
    }
}