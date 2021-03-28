namespace SportClubsChallenges.Jobs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Database.Entities;
    using SportClubsChallenges.Utils.Enums;

    public class UpdateChallengesClassificationsJob
    {
        private readonly SportClubsChallengesDbContext db;

        public UpdateChallengesClassificationsJob(SportClubsChallengesDbContext db)
        {
            this.db = db;
        }

        public async Task Run(long? challengeId = null)
        {
            if (challengeId.HasValue)
            {
                await this.UpdateSpecifiedChallengeClassification(challengeId.Value);
                return;
            }

            await this.UpdateAllChallengesClassifications();
        }

        private async Task UpdateSpecifiedChallengeClassification(long challengeId)
        {
            var athlete = this.db.Challenges.Find(challengeId);
            if (athlete == null)
            {
                return;
            }

            await UpdateClassification(athlete);
        }

        private async Task UpdateAllChallengesClassifications()
        {
            var activeChallenges = await this.db.Challenges
                .Include(p => p.ChallengeActivityTypes)
                .Where(p => p.IsActive && p.StartDate.Date <= DateTimeOffset.Now.Date)
                .ToListAsync();

            foreach (var challenge in activeChallenges)
            {
                await UpdateClassification(challenge);
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
                var activities = await GetAthleteActivitiesForChallenge(participant.Athlete, challenge);

                participant.Score = CalculateScore(activities, challenge);
            }

            UpdateRank(participants);

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
            var competitionType = (ChallengeCompetitionTypeEnum)challenge.CompetitionType;
            var score = 0;

            switch (competitionType)
            {
                case ChallengeCompetitionTypeEnum.Distance:
                    score = activities.Sum(p => p.Distance);
                    break;
                case ChallengeCompetitionTypeEnum.Time:
                    score = activities.Sum(p => p.Duration);
                    break;
                case ChallengeCompetitionTypeEnum.Elevation:
                    score = activities.Sum(p => p.Elevation);
                    break;
            }

            return score;
        }

        private void UpdateRank(List<ChallengeParticipant> participants)
        {
            var rank = 1;

            foreach (var participant in participants.OrderByDescending(p => p.Score).ThenBy(p => p.RegistrationDate))
            {
                participant.Rank = rank++;
                participant.LastUpdateDate = DateTimeOffset.Now;
            }
        }
    }
}