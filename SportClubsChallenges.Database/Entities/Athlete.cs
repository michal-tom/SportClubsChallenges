namespace SportClubsChallenges.Database.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Athlete
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public long Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string IconUrlLarge { get; set; }

        public string IconUrlMedium { get; set; }

        public string City { get; set; }

        public string Gender { get; set; }

        public string Country { get; set; }

        public bool IsAdmin { get; set; }

        public DateTimeOffset FirstLoginDate { get; set; }

        public DateTimeOffset LastLoginDate { get; set; }

        public DateTimeOffset? LastSyncDate { get; set; }

        public long AthleteStravaTokenId { get; set; }

        #region References

        public virtual AthleteStravaToken AthleteStravaToken { get; set; }

        public virtual ICollection<ChallengeParticipant> ChallengeParticipants { get; set; }

        public virtual ICollection<ClubMember> ClubMembers { get; set; }

        #endregion
    }
}
