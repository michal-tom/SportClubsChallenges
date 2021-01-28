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

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public byte ChallengeType { get; set; }

        public bool PreventManualActivities { get; set; }

        public bool IncludeOnlyGpsActivities { get; set; }

        public long OwnerId { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime EditionDate { get; set; }

        #region References

        public virtual Club Club { get; set; }

        public virtual ICollection<ChallengeActivityType> ChallengeActivityTypes { get; set; }

        public virtual ICollection<ChallengeParticipant> ChallengeParticipants { get; set; }

        public virtual Athlete Owner { get; set; }

        #endregion
    }
}
