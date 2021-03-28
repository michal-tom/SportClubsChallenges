namespace SportClubsChallenges.Model.Dto
{
    public class ChallengeRankPositionDto
    {
        public long AthleteId { get; set; }

        public string AthleteName { get; set; }

        public string AthleteIconUrlMedium { get; set; }

        public int Rank { get; set; }

        public int Score { get; set; }

        public string AthleteIconUrl => !string.IsNullOrEmpty(this.AthleteIconUrlMedium) ? this.AthleteIconUrlMedium : "/images/strava_user.png";
    }
}
