namespace SportClubsChallenges.Utils.Helpers
{
    using System;
    using SportClubsChallenges.Utils.Enums;

    public static class UnitHelper
    {
        public static string GetScoreDescription(int score, ChallengeScoreUnit scoreUnit)
        {
            switch (scoreUnit)
            {
                case ChallengeScoreUnit.Meters:
                    return GetDistanceDescription(score);
                case ChallengeScoreUnit.Seconds:
                    return GetTimeDescription(score);
                default:
                    throw new ArgumentException(message: "Invalid score unit enum value", paramName: nameof(scoreUnit));
            };
        }

        public static string GetTimeDescription(int seconds)
        {
            var timespanDuration = TimeSpan.FromSeconds(seconds).Duration();
            if (timespanDuration.Minutes == 0)
            {
                return "-";
            }

            return string.Format("{0}{1}{2}",
                timespanDuration.Days > 0 ? string.Format("{0:0}d ", timespanDuration.Days) : string.Empty,
                timespanDuration.Hours > 0 ? string.Format("{0:0}h ", timespanDuration.Hours) : string.Empty,
                timespanDuration.Minutes > 0 ? string.Format("{0:0}m", timespanDuration.Minutes) : string.Empty);
        }

        public static string GetDistanceDescription(int meters)
        {
            if (meters == 0)
            {
                return "-";
            }
            else if (meters < 1000)
            {
                return GetDistanceMetersDescription(meters);
            }
            
            return Decimal.Round(Decimal.Divide(meters, 1000), 2).ToString() + " km";
        }

        public static string GetDistanceMetersDescription(int meters)
        {
            if (meters == 0)
            {
                return "-";
            }

            return meters.ToString() + " m";
        }
    }
}