namespace SportClubsChallenges.Jobs.Clubs
{
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Database.Entities;
    using SportClubsChallenges.Domain.Interfaces;
    using SportClubsChallenges.Strava;
    using SportClubsChallenges.Strava.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class GetAthleteClubsJob
    {
        private readonly SportClubsChallengesDbContext db;

        private readonly IStravaApiWrapper stravaWrapper;

        private readonly ITokenService tokenService;

        private readonly IMapper mapper;

        public GetAthleteClubsJob(SportClubsChallengesDbContext db, IStravaApiWrapper stravaWrapper, ITokenService tokenService, IMapper mapper)
        {
            this.db = db;
            this.stravaWrapper = stravaWrapper;
            this.tokenService = tokenService;
            this.mapper = mapper;
        }

        public async Task Run()
        {
            var athleths = await this.db.Athletes
                .Include(p => p.AthleteStravaToken)
                .ToListAsync();

            foreach (var athlete in athleths)
            {
                await this.GetAthleteClubs(athlete);
            }
        }

        private async Task GetAthleteClubs(Athlete athlete)
        {
            var stravaToken = this.tokenService.GetStravaToken(athlete);

            var athleteClubsInStrava = await this.GetClubsFromStrava(stravaToken);
            if (athleteClubsInStrava == null || !athleteClubsInStrava.Any())
            {
                return;
            }

            var athleteClubMemebershipInDb = await this.GetAthleteClubMemebership(athlete.Id);

            this.UpdateClubsMambership(athlete.Id, athleteClubsInStrava, athleteClubMemebershipInDb);

            await this.db.SaveChangesAsync();
        }

        private async Task<List<Club>> GetClubsFromStrava(StravaToken stravaToken)
        {
            try
            {
                var stravaClubs = await this.stravaWrapper.GetAthleteClubs(stravaToken);
                return this.mapper.Map<List<Club>>(stravaClubs);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task<List<ClubMember>> GetAthleteClubMemebership(long athleteId)
        {
            return await this.db.ClubMembers.Where(p => p.AthleteId == athleteId).ToListAsync();
        }

        private void UpdateClubsMambership(long athleteId, List<Club> athleteClubsInStrava, List<ClubMember> athleteClubMemebershipInDb)
        {
            foreach (var clubInStrava in athleteClubsInStrava)
            {
                this.UpdateClubData(clubInStrava);

                var currentAthleteClubMemebershipInDb = athleteClubMemebershipInDb.FirstOrDefault(p => p.ClubId == clubInStrava.Id);
                if (currentAthleteClubMemebershipInDb == null)
                {
                    db.ClubMembers.Add(new ClubMember { AthleteId = athleteId, ClubId = clubInStrava.Id });
                }
            }
        }

        private void UpdateClubData(Club clubData)
        {
            var currentClubInDb = this.db.Clubs.FirstOrDefault(p => p.Id == clubData.Id);
            if (currentClubInDb == null)
            {
                this.db.Clubs.Add(clubData);
                return;
            }

            if (currentClubInDb.Name != clubData.Name
                || currentClubInDb.MembersCount != clubData.MembersCount
                || currentClubInDb.IconUrl != clubData.IconUrl)
            {
                currentClubInDb.Name = clubData.Name;
                currentClubInDb.MembersCount = clubData.MembersCount;
                currentClubInDb.IconUrl = clubData.IconUrl;
            }
        }
    }
}