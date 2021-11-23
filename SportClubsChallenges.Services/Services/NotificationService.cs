namespace SportClubsChallenges.Domain.Services
{
    using AutoMapper;
    using Microsoft.AspNetCore.SignalR.Client;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using SportClubsChallenges.Database.Data;
    using SportClubsChallenges.Database.Entities;
    using SportClubsChallenges.Domain.Interfaces;
    using SportClubsChallenges.Model.Dto;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class NotificationService : INotificationService
    {
        private readonly SportClubsChallengesDbContext db;

        private readonly IMapper mapper;

        private readonly string NotificationHubUrl;

        public NotificationService(IMapper mapper, SportClubsChallengesDbContext db, IConfiguration configuration)
        {
            this.mapper = mapper;
            this.db = db;
            this.NotificationHubUrl = configuration["SportClubsChallengeWebAppUrl"].TrimEnd('/') + "/notifications";
        }

        public async Task<List<NotificationDto>> GetAthleteNotifications(long athleteId, bool showOnlyUnread)
        {
            var notifications = this.db.Notifications
                .Where(p => p.AthleteId == athleteId && (!showOnlyUnread || !p.IsRead))
                .OrderByDescending(p => p.Id);

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

        public async Task CreateNewChallengesNotification(long challengeId, string challengeName, string clubName, List<long> athletesIds)
        {
            var notificationTitle = "New challenge was created!";
            var notificationText = $"New challenge <b>{challengeName}</b> for club <b>{clubName}</b> was already created!" +
                "<br />" +
                "<br />" +
                $"Please visit <a href='/challenges/details/{challengeId}'>{challengeName}</a> and join to this challenge!";
            var notificationDate = DateTimeOffset.Now;

            foreach (var athleteId in athletesIds)
            {
                var notification = new Notification
                {
                    Title = notificationTitle,
                    Text = notificationText,
                    AthleteId = athleteId,
                    CreationDate = notificationDate,
                    IsRead = false
                };
                db.Notifications.Add(notification);
            }

            await db.SaveChangesAsync();

            await this.NotifyCurrentlyLoggedUsers(athletesIds, notificationTitle);
        }

        public async Task CreateNewActivityNotification(long activityId, long athleteId, string activityName, string activityType)
        {
            var notification = new Notification
            {
                Title = "New activity uploaded!",
                Text = $"New <b>{activityType}</b> activity <b>{activityName}</b> was already uploaded to Strava!",
                AthleteId = athleteId,
                CreationDate = DateTimeOffset.Now,
                IsRead = false
            };

            db.Notifications.Add(notification);

            await db.SaveChangesAsync();

            await this.NotifyCurrentlyLoggedUsers(new List<long> { athleteId }, notification.Title);
        }

        private async Task NotifyCurrentlyLoggedUsers(List<long> athletesIds, string message)
        {
            var hubConnection = new HubConnectionBuilder().WithUrl(this.NotificationHubUrl).Build();

            await hubConnection.StartAsync();
            await hubConnection.InvokeAsync("NotifyLoggedUsers", athletesIds, message);
        }
    }
}
