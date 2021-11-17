namespace SportClubsChallenges.Model.Dto
{
    using System;

    public class NotificationDto
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public DateTime CreationDate { get; set; }

        public bool IsRead { get; set; }
    }
}
