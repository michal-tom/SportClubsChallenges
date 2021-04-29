namespace SportClubsChallenges.Utils.Enums
{
    using SportClubsChallenges.Utils.Attributes;

    public enum ActivityTypeEnum : byte
    {
        Other = 0,
        AlpineSki = 1,
        BackcountrySki = 2,
        Canoeing = 3,
        Crossfit = 4,
        [BikeActivity]
        EBikeRide = 5,
        Elliptical = 6,
        Golf = 7,
        Handcycle = 8,
        [RunActivity]
        Hike = 9,
        IceSkate = 10,
        InlineSkate = 11,
        Kayaking = 12,
        Kitesurf = 13,
        NordicSki = 14,
        [BikeActivity]
        Ride = 15,
        RockClimbing = 16,
        RollerSki = 17,
        Rowing = 18,
        [RunActivity]
        Run = 19,
        Sail = 20,
        Skateboard = 21,
        Snowboard = 22,
        Snowshoe = 23,
        Soccer = 24,
        StairStepper = 25,
        StandUpPaddling = 26,
        Surfing = 27,
        Swim = 28,
        Velomobile = 29,
        [BikeActivity]
        VirtualRide = 30,
        [RunActivity]
        VirtualRun = 31,
        [RunActivity]
        Walk = 32,
        WeightTraining = 33,
        Wheelchair = 34,
        Windsurf = 35,
        Workout = 36,
        Yoga = 37
    }
}