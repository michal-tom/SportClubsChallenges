﻿namespace SportClubsChallenges.Domain.Interfaces
{
    using Microsoft.AspNetCore.Authentication;
    using SportClubsChallenges.Database.Entities;
    using SportClubsChallenges.Strava.Model;
    using System.Threading.Tasks;

    public interface ITokenService
    {
        StravaToken GetStravaToken(Athlete athlete);

        void UpdateStravaToken(Athlete athlete, AuthenticationProperties properties);

        void UpdateStravaToken(Athlete athlete, StravaToken token);
    }
}