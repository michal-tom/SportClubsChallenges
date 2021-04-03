namespace SportClubsChallenges.Utils.Helpers
{
    using SportClubsChallenges.Utils.Enums;
    using System;

    public static class HtmlHelper
    {
        public static string GetIconForSportType(string sportType)
        {
            if (sportType != null && sportType.ToLower() == ChallengeSportCategoryEnum.Cycling.ToString().ToLower())
            {
                return "bicycle";
            }

            return "running";
        }

        public static string GetIconForCompetitionType(byte competitionType)
        {
            var competitionTypeEnumValue = (ChallengeCompetitionTypeEnum) competitionType;
            switch (competitionTypeEnumValue)
            {
                case ChallengeCompetitionTypeEnum.Distance:
                    return "map";
                case ChallengeCompetitionTypeEnum.Time:
                    return "clock";
                case ChallengeCompetitionTypeEnum.Elevation:
                    return "elevation";
                default:
                    throw new ArgumentException(message: "Invalid competition type enum value", paramName: nameof(competitionType));
            };
        }
    }
}
