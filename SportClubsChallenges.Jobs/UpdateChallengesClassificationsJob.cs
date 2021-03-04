namespace SportClubsChallenges.Jobs
{
    using Microsoft.EntityFrameworkCore;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Database.Entities;
    using SportClubsChallenges.Model.Enums;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class UpdateChallengesClassificationsJob
    {
        private readonly SportClubsChallengesDbContext db;

        public UpdateChallengesClassificationsJob(SportClubsChallengesDbContext db)
        {
            this.db = db;
        }

        public async Task Run()
        {
            var activeChallenges = await this.db.Challenges
                .Include(p => p.ChallengeActivityTypes)
                .Where(p => p.IsActive && p.StartDate.Date <= DateTimeOffset.Now.Date)
                .ToListAsync();

            foreach (var challenge in activeChallenges)
            {
                await this.UpdateClassification(challenge);
            }
        }

        private async Task UpdateClassification(Challenge challenge)
        {
            var participants = await this.db.ChallengeParticipants
                .Include(p => p.Athlete)
                .Where(p => p.ChallengeId == challenge.Id)
                .ToListAsync();

            if (!participants.Any())
            {
                return;
            }

            foreach (var participant in participants)
            {
                var activities = await this.GetAthleteActivitiesForChallenge(participant.Athlete, challenge);

                participant.Score = this.CalculateScore(activities, challenge);
            }

            this.UpdateRank(participants);
            
            await this.db.SaveChangesAsync();
        }

        private async Task<List<Activity>> GetAthleteActivitiesForChallenge(Athlete athlete, Challenge challenge)
        {
            var challengeActivityTypes = challenge.ChallengeActivityTypes.Select(p => p.ActivityType);

            return await this.db.Activities
                .Where(p => p.AthleteId == athlete.Id 
                    && !p.IsDeleted 
                    && p.StartDate >= challenge.StartDate
                    && p.EndDate <= challenge.EndDate
                    && !(p.IsManual && challenge.PreventManualActivities)
                    && !(!p.IsGps && challenge.IncludeOnlyGpsActivities)
                    && (!challengeActivityTypes.Any() || challengeActivityTypes.Contains(p.ActivityType)))
                .ToListAsync();
        }

        private int CalculateScore(List<Activity> activities, Challenge challenge)
        {
            var challengeType = (ChallengeTypeEnum) challenge.ChallengeType;
            var score = 0;

            switch (challengeType)
            {
                case ChallengeTypeEnum.Distance:
                    score = activities.Sum(p => p.Distance);
                    break;
                case ChallengeTypeEnum.Time:
                    score = activities.Sum(p => p.Duration);
                    break;
                case ChallengeTypeEnum.Elevation:
                    score = activities.Sum(p => p.Elevation);
                    break;
            }

            return score;
        }

        private void UpdateRank(List<ChallengeParticipant> participants)
        {
            var rank = 1;

            foreach(var participant in participants.OrderByDescending(p => p.Score).ThenBy(p => p.RegistrationDate))
            {
                participant.Rank = rank++;
                participant.LastUpdateDate = DateTimeOffset.Now;
            }
        }
    }
}