namespace SportClubsChallenges.Database.Entities
{
    using System;
    using System.Collections.Generic;

    public class Athlete
    {
        public long Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public byte[] Icon { get; set; }

        public string City { get; set; }

        public string Gender { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime LastLoginDate { get; set; }

        public long AthleteStravaTokenId { get; set; }

        #region References

        public virtual AthleteStravaToken AthleteStravaToken { get; set; }

        public virtual ICollection<ChallengeParticipant> ChallengeParticipants { get; set; }

        public virtual ICollection<Club> OwnedClubs { get; set; }

        #endregion
    }
}
