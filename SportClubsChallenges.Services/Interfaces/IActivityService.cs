namespace SportClubsChallenges.Domain.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using SportClubsChallenges.Database.Entities;
    using SportClubsChallenges.Model.Dto;

    public interface IActivityService
    {
        Task<List<ActivityDto>> GetAthleteActivities(long athleteId, int? maxCount = null);

        Task<List<ActivityDto>> GetAllActivities();

        Task AddActivity(Activity activity);

        Task SyncActivities(long athleteId, List<Activity> activities, DateTimeOffset startDate);

        Task UpdateActivity(long athleteId, long activityId, string activityTitle, string activityType, bool? isPrivate);

        Task DeleteActivity(long athleteId, long activityId);
    }
}