namespace SportClubsChallenges.Database.Entities
{
    using System;

    public class Activity
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public long AthleteId { get; set; }

        public byte ActivityTypeId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int Duration { get; set; }

        public int Distance { get; set; }

        public int Elevation { get; set; }

        public decimal Pace { get; set; }

        public string Map { get; set; }

        #region References

        public virtual ActivityType ActivityType { get; set; }

        public virtual Athlete Athlete { get; set; }

        #endregion
    }
}
