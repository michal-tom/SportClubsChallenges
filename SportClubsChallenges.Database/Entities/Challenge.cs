namespace SportClubsChallenges.Database.Entities
{
    using System;
    using System.Collections.Generic;

    public class Challenge
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public long ClubId { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public byte SportCategory { get; set; }

        public byte RivalryType { get; set; }

        public bool PreventManualActivities { get; set; }

        public bool IncludeOnlyGpsActivities { get; set; }

        public long AuthorId { get; set; }

        public DateTimeOffset CreationDate { get; set; }

        public DateTimeOffset EditionDate { get; set; }

        #region References

        public virtual Club Club { get; set; }

        public virtual ICollection<ChallengeActivityType> ChallengeActivityTypes { get; set; }

        public virtual ICollection<ChallengeParticipant> ChallengeParticipants { get; set; }

        public virtual Athlete Author { get; set; }

        #endregion
    }
}
