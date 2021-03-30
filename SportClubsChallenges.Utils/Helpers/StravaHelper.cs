namespace SportClubsChallenges.Utils.Helpers
{
    using SportClubsChallenges.Utils.Consts;

    public static class StravaHelper
    {
        public static string GetMainPageUrl()
        {
            return StravaConsts.UrlAddress;
        }

        public static string GetClubUrl(string clubSuffix)
        {
            return $"{StravaConsts.UrlAddress}/clubs/{clubSuffix}";
        }

        public static string GetActivityUrl(long activityId)
        {
            return $"{StravaConsts.UrlAddress}/activities/{activityId}";
        }

        public static string GetAthleteUrl(long athleteId)
        {
            return $"{StravaConsts.UrlAddress}/athletes/{athleteId}";
        }
    }
}
