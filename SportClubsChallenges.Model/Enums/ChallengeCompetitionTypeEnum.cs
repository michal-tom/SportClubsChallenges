namespace SportClubsChallenges.Model.Enums
{
    using SportClubsChallenges.Model.Attributes;
    using System.ComponentModel;

    public enum ChallengeCompetitionTypeEnum : byte
    {
        [Description("Distance")]
        [Unit("m")]
        Distance = 0,

        [Description("Time")]
        [Unit("s")]
        Time = 1,

        [Description("Elevation")]
        [Unit("m")]
        Elevation = 2
    }
}