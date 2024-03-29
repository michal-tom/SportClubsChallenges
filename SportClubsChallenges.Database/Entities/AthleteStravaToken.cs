﻿namespace SportClubsChallenges.Database.Entities
{
    using System;

    public class AthleteStravaToken
    {
        public long Id { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public string TokenType { get; set; }

        public DateTimeOffset ExpirationDate { get; set; }

        public DateTimeOffset LastUpdateDate { get; set; }
    }
}
