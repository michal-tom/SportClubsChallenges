namespace SportClubsChallenges.Model.Dto
{
    public class ClubDto
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string SportType { get; set; }

        public string IconUrl { get; set; }

        // TODO: change to StravaUrl
        public string Url { get; set; }

        public string Owner { get; set; }

        public int MembersCount { get; set; }

        public string StravaUrl => $"https://www.strava.com/clubs/{this.Url}";
    }
}
