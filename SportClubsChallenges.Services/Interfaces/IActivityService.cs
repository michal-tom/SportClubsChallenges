namespace SportClubsChallenges.Domain.Interfaces
{
    using SportClubsChallenges.Model.Dto;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IActivityService
    {
        Task<List<ActivityDto>> GetAthleteActivities(long athleteId, int? maxCount = null);
    }
}
