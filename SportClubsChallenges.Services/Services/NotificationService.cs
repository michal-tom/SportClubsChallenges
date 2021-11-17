namespace SportClubsChallenges.Domain.Services
{
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Domain.Interfaces;
    using SportClubsChallenges.Model.Dto;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class NotificationService : INotificationService
    {
        private readonly SportClubsChallengesDbContext db;

        private readonly IMapper mapper;

        public NotificationService(IMapper mapper, SportClubsChallengesDbContext db)
        {
            this.mapper = mapper;
            this.db = db;
        }

        public async Task<List<NotificationDto>> GetAthleteNotifications(long athleteId, bool showOnlyUnread)
        {
            var notifications = this.db.Notifications.Where(p => p.AthleteId == athleteId && (!showOnlyUnread || !p.IsRead));
            return await mapper.ProjectTo<NotificationDto>(notifications).ToListAsync();
        }

        public async Task<long> GetAthleteUnreadNotificationsCount(long athleteId)
        {
            return await this.db.Notifications.CountAsync(p => p.AthleteId == athleteId && !p.IsRead);
        }

        public async Task ChangeNotificationReadStatus(long notificationId, bool isRead)
        {
            var notification = db.Notifications.Find(notificationId);
            notification.IsRead = isRead;

            await db.SaveChangesAsync();
        }

        public async Task DeleteNotification(long notificationId)
        {
            var notification = db.Notifications.Find(notificationId);
            db.Notifications.Remove(notification);

            await db.SaveChangesAsync();
        }
    }
}
