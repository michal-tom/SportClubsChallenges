namespace SportClubsChallenges.Model.Enums
{
    using System.ComponentModel;

    public enum ChallengeTypeEnum : byte
    {
        [Description("Suma pokonanych kilometrów")]
        Distance = 0,

        [Description("Suma czasu aktywności")]
        Time = 1,

        [Description("Suma wzniesień")]
        Elevation = 2
    }
}