namespace SportClubsChallenges.Utils.Attributes
{
    using System;
    using SportClubsChallenges.Utils.Enums;

    [AttributeUsage(AttributeTargets.Field)]
    public class UnitAttribute : Attribute
    {
        public ChallengeScoreUnit Unit { get; set; }

        public UnitAttribute(ChallengeScoreUnit unit)
        {
            this.Unit = unit;
        }
    }
}
