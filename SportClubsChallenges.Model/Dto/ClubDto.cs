namespace SportClubsChallenges.Model.Dto
{
    public class ClubDto
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string SportType { get; set; }

        public string Owner { get; set; }

        public int MembersCount { get; set; }
    }
}
