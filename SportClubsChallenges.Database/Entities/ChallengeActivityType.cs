namespace SportClubsChallenges.Database.Entities
{
    public class ChallengeActivityType
    {
        public long ChallengeId { get; set; }

        public byte ActivityTypeId { get; set; }

        #region References

        public virtual Challenge Challenge { get; set; }
        public virtual ActivityType ActivityType { get; set; }

        #endregion
    }
}
