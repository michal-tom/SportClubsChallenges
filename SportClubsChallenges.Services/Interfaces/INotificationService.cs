namespace SportClubsChallenges.Domain.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using SportClubsChallenges.Model.Dto;

    public interface INotificationService
    {
        Task<List<NotificationDto>> GetAthleteNotifications(long athleteId, bool showOnlyUnread);

        Task<long> GetAthleteUnreadNotificationsCount(long athleteId);

        Task ChangeNotificationReadStatus(long notificationId, bool isRead);

        Task DeleteNotification(long notificationId);

        Task CreateNewChallengesNotification(long challengeId, string challengeName, string clubName, List<long> athletesIds);
    }
}
