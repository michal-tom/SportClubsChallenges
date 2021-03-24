namespace SportClubsChallenges.Utils.Enums
{
    using System.ComponentModel;
    using SportClubsChallenges.Utils.Attributes;

    public enum ChallengeCompetitionTypeEnum : byte
    {
        [Description("Distance")]
        [Unit(ChallengeScoreUnit.Meters)]
        Distance = 0,

        [Description("Time")]
        [Unit(ChallengeScoreUnit.Seconds)]
        Time = 1,

        [Description("Elevation")]
        [Unit(ChallengeScoreUnit.Meters)]
        Elevation = 2
    }
}