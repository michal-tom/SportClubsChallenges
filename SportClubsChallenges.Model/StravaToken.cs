namespace SportClubsChallenges.Model
{
    using System;

    public class StravaToken
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string TokenType { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }
    }
}