namespace SportClubsChallenges.Model.Dto
{
    public class AthleteDto
    {
        public string Name { get; set; }

        public string IconUrlLarge { get; set; }

        public string IconUrlMedium { get; set; }

        public string City { get; set; }

        public string Gender { get; set; }

        public string Country { get; set; }

        public string IconUrl => !string.IsNullOrEmpty(this.IconUrlMedium) ? this.IconUrlMedium : "/images/strava_user.png";

        public string AvatarUrl => !string.IsNullOrEmpty(this.IconUrlLarge) ? this.IconUrlLarge : "/images/strava_user.png";

        public string GenderDescription => this.Gender == "M" ? "Male" : this.Gender == "F" ? "Female" : "Other";
    }
}
