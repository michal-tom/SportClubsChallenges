namespace SportClubsChallenges.Strava.Model
{
    using System;

    public class StravaToken
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string TokenType { get; set; }
        public DateTimeOffset ExpirationDate { get; set; }
    }
}
