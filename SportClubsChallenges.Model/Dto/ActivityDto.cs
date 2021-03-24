namespace SportClubsChallenges.Model.Dto
{
    using System;

    public class ActivityDto
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string ActivityType { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int Duration { get; set; }

        public int Distance { get; set; }

        public int Elevation { get; set; }

        public float Pace { get; set; }

        public bool IsManual { get; set; }

        public bool IsGps { get; set; }

        public string Link => $"https://www.strava.com/activities/{this.Id}";
    }
}