namespace SportClubsChallenges.Model.Enums
{
    using SportClubsChallenges.Model.Attributes;
    using System.ComponentModel;

    public enum ChallengeTypeEnum : byte
    {
        [Description("Activity distance sum")]
        [Unit("m")]
        Distance = 0,

        [Description("Activity time sum")]
        [Unit("s")]
        Time = 1,

        [Description("Activity elevation sum")]
        [Unit("m")]
        Elevation = 2
    }
}