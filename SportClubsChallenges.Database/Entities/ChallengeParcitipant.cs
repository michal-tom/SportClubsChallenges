﻿namespace SportClubsChallenges.Database.Entities
{
    using System;

    public class ChallengeParticipant
    {
        public long ChallengeId { get; set; }

        public long AthleteId { get; set; }

        public int Score { get; set; }

        public int Rank { get; set; }

        public DateTime RegistrationDate { get; set; }

        public DateTime LastUpdateDate { get; set; }

        #region References

        public virtual Challenge Challenge { get; set; }
        public virtual Athlete Athlete { get; set; }

        #endregion
    }
}
