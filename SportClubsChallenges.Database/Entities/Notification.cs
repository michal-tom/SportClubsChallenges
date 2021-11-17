namespace SportClubsChallenges.Database.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Notification
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public long AthleteId { get; set; }

        public DateTimeOffset CreationDate { get; set; }

        public bool IsRead { get; set; }

        #region References

        public virtual Athlete Athlete { get; set; }

        #endregion
    }
}
