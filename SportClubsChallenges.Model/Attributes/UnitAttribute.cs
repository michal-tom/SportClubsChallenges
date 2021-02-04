namespace SportClubsChallenges.Model.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Field)]
    public class UnitAttribute : Attribute
    {
        public string Unit { get; set; }

        public UnitAttribute(string value)
        {
            this.Unit = value;
        }
    }
}
