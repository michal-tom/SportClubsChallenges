namespace SportClubsChallenges.AzureFunctions.Consts
{
    public static class FunctionsConsts
    {
        public const string EventsRoute = "strava/events";

        public const string CreateSubscriptionRoute = "strava/subscription/create";

        public const string ViewSubscriptionRoute = "strava/subscription/view";

        public const string DeleteSubscriptionRoute = "strava/subscription/remove/{id}";

        public const string SubscriptionCallbackToken = "SportClubsChallengesSubscriptionToken";
    }
}
