namespace SportClubsChallenges.Model.Dto
{
    public class AthleteDto
    {
        public string Name { get; set; }

        public string IconUrlMedium { get; set; }

        public string IconUrl {
            get => !string.IsNullOrEmpty(this.IconUrlMedium) ? this.IconUrlMedium : "/images/strava_user.png";
        }
    }
}
