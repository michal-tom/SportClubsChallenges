namespace SportClubsChallenges.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Database.Entities;
    using SportClubsChallenges.Domain.Interfaces;
    using SportClubsChallenges.Model.Dto;
    using SportClubsChallenges.Utils.Enums;
    using SportClubsChallenges.Utils.Helpers;

    public class ActivityService : IActivityService
    {
        private readonly SportClubsChallengesDbContext db;

        private readonly IMapper mapper;

        public ActivityService(IMapper mapper, SportClubsChallengesDbContext db)
        {
            this.mapper = mapper;
            this.db = db;
        }

        public async Task<List<ActivityDto>> GetAthleteActivities(long athleteId, int? maxCount = null)
        {
            var activities = db.Activities
                .Where(p => p.AthleteId == athleteId && !p.IsDeleted)
                .OrderByDescending(p => p.StartDate)
                .Take(maxCount == null ? int.MaxValue : maxCount.Value);

            return await mapper.ProjectTo<ActivityDto>(activities).ToListAsync();
        }

        public async Task<List<ActivityDto>> GetAllActivities()
        {
            return await mapper.ProjectTo<ActivityDto>(db.Activities.OrderByDescending(p => p.StartDate)).ToListAsync();
        }

        public async Task<long?> AddActivity(Activity activity)
        {
            var activityInDb = await this.db.Activities.FirstOrDefaultAsync(p => p.Id == activity.Id);
            if (activityInDb == null)
            {
                
                this.db.Activities.Add(activity);
                await this.db.SaveChangesAsync();
                return activity.Id; 
            }

            if (IsActivityChanged(activity, activityInDb))
            {
                UpdateActivity(activity, activityInDb);
                await this.db.SaveChangesAsync();
            }

            return null;
        }

        public async Task SyncActivities(long athleteId, List<Activity> activitesFromStrava, DateTimeOffset startDate)
        {
            var activitiesInDb = await this.db.Activities
                .Where(p => p.AthleteId == athleteId && p.StartDate >= startDate)
                .ToListAsync();

            await this.UpdateActivitiesInDatabase(activitesFromStrava, activitiesInDb);
        }

        public async Task UpdateActivity(long athleteId, long activityId, string activityTitle, string activityType, bool? isPrivate)
        {
            var activityInDb = await this.db.Activities.FirstOrDefaultAsync(p => p.Id == activityId && p.AthleteId == athleteId);
            if (activityInDb == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(activityTitle))
            {
                activityInDb.Name = activityTitle;
            }

            if (!string.IsNullOrEmpty(activityType))
            {
                var activityTypeId = EnumsHelper.GetEnumIdByName<ActivityTypeEnum>(activityType);
                activityInDb.ActivityTypeId = activityTypeId;
            }

            if (isPrivate.HasValue)
            {
                activityInDb.IsPrivate = isPrivate.Value;
            }

            await this.db.SaveChangesAsync();
        }

        public async Task DeleteActivity(long athleteId, long activityId)
        {
            var activityInDb = await this.db.Activities.FirstOrDefaultAsync(p => p.Id == activityId && p.AthleteId == athleteId);
            if (activityInDb == null || activityInDb.IsDeleted)
            {
                return;
            }

            activityInDb.IsDeleted = true;
            await this.db.SaveChangesAsync();
        }

        private async Task UpdateActivitiesInDatabase(IList<Activity> activitesFromStrava, IList<Activity> activitiesInDb)
        {
            foreach (var activityFromStrava in activitesFromStrava)
            {
                var currentActivityInDb = activitiesInDb.FirstOrDefault(p => p.Id == activityFromStrava.Id);
                if (currentActivityInDb == null)
                {
                    this.db.Activities.Add(activityFromStrava);
                    continue;
                }

                if (IsActivityChanged(activityFromStrava, currentActivityInDb))
                {
                    UpdateActivity(activityFromStrava, currentActivityInDb);
                }
            }

            CheckRemovedActivities(activitesFromStrava, activitiesInDb);

            await this.db.SaveChangesAsync();
        }

        private static bool IsActivityChanged(Activity activityFromStrava, Activity currentActivityInDb)
        {
            return currentActivityInDb.Name != activityFromStrava.Name
                || currentActivityInDb.ActivityTypeId != activityFromStrava.ActivityTypeId
                || currentActivityInDb.Duration != activityFromStrava.Duration
                || currentActivityInDb.IsDeleted;
        }

        private static void UpdateActivity(Activity activityFromStrava, Activity currentActivityInDb)
        {
            currentActivityInDb.Name = activityFromStrava.Name;
            currentActivityInDb.ActivityTypeId = activityFromStrava.ActivityTypeId;
            currentActivityInDb.Duration = activityFromStrava.Duration;
            currentActivityInDb.Distance = activityFromStrava.Distance;
            currentActivityInDb.Elevation = activityFromStrava.Elevation;
            currentActivityInDb.Pace = activityFromStrava.Pace;
            currentActivityInDb.StartDate = activityFromStrava.StartDate;
            currentActivityInDb.EndDate = activityFromStrava.EndDate;
            currentActivityInDb.IsDeleted = false;
        }

        private static void CheckRemovedActivities(IList<Activity> activitesFromStrava, IList<Activity> activitiesInDb)
        {
            foreach (var currentActivityInDb in activitiesInDb.Where(p => !p.IsDeleted && !activitesFromStrava.Any(a => a.Id == p.Id)))
            {
                currentActivityInDb.IsDeleted = true;
            }
        }
    }
}