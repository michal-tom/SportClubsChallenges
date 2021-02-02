namespace SportClubsChallenges.Database.Entities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Club
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public long Id { get; set; }

        public string Name { get; set; }

        public string SportType { get; set; }

        public string IconUrl { get; set; }

        public string Url { get; set; }

        public int MembersCount { get; set; }

        #region References

        public virtual ICollection<ClubMember> ClubMembers { get; set; }

        #endregion
    }
}
