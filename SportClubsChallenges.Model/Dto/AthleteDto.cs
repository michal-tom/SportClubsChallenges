namespace SportClubsChallenges.Model.Dto
{
    using System;

    public class AthleteDto
    {
        public long Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string IconUrlLarge { get; set; }

        public string IconUrlMedium { get; set; }

        public string City { get; set; }

        public string Gender { get; set; }

        public string Country { get; set; }

        public DateTime FirstLoginDate { get; set; }

        public DateTime LastLoginDate { get; set; }

        public DateTime? LastSyncDate { get; set; }

        public bool IsAdmin { get; set; }

        public string Name => this.FirstName + " " + this.LastName;

        public string IconUrl => !string.IsNullOrEmpty(this.IconUrlMedium) ? this.IconUrlMedium : "/images/strava_user.png";

        public string AvatarUrl => !string.IsNullOrEmpty(this.IconUrlLarge) ? this.IconUrlLarge : "/images/strava_user.png";

        public string GenderDescription => this.Gender == "M" ? "Male" : this.Gender == "F" ? "Female" : "Other";

        public string StravaUrl => $"https://www.strava.com/athletes/{this.Id}";
    }
}
