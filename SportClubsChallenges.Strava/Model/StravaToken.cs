namespace SportClubsChallenges.Strava.Model
{
    using System;

    public class StravaToken
    {
        public long DatabaseId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string TokenType { get; set; }
        public DateTimeOffset ExpirationDate { get; set; }

        public event EventHandler OnTokenRefresh;

        public void TokenRefresh()
        {
            if (OnTokenRefresh != null)
            {
                OnTokenRefresh(this, EventArgs.Empty);
            }
        }
    }
}
