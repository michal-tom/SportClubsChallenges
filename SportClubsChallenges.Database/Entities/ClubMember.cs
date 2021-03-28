namespace SportClubsChallenges.Database.Entities
{
    public class ClubMember
    {
        public long ClubId { get; set; }

        public long AthleteId { get; set; }

        public bool IsAdmin { get; set; }

        #region References

        public virtual Club Club { get; set; }
        public virtual Athlete Athlete { get; set; }

        #endregion
    }
}
