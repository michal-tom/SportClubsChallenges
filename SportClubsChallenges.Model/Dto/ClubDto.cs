namespace SportClubsChallenges.Model.Dto
{
    public class ClubDto
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string SportType { get; set; }

        public string IconUrl { get; set; }

        public string Url { get; set; }

        public string Owner { get; set; }

        public int MembersCount { get; set; }

        public int ActiveChallengesCount { get; set; }

        public int InactiveChallengesCount { get; set; }
    }
}