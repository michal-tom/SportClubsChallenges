namespace SportClubsChallenges.Database.Entities
{
    using System.Collections.Generic;

    public class Club
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string SportType { get; set; }

        public byte[] Icon { get; set; }

        public long OwnerId { get; set; }

        public int MembersCount { get; set; }

        #region References

        public virtual Athlete Owner { get; set; }

        #endregion
    }
}
