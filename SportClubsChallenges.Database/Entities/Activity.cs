namespace SportClubsChallenges.Database.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Activity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public long Id { get; set; }

        public string Name { get; set; }

        public long AthleteId { get; set; }

        public byte ActivityTypeId { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public int Duration { get; set; }

        public int Distance { get; set; }

        public int Elevation { get; set; }

        public float Pace { get; set; }

        public bool IsManual { get; set; }

        public bool IsGps { get; set; }

        #region References

        public virtual ActivityType ActivityType { get; set; }

        public virtual Athlete Athlete { get; set; }

        #endregion
    }
}
