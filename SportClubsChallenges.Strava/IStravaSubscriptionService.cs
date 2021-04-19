namespace SportClubsChallenges.Strava
{
    using System.Threading.Tasks;

    public interface IStravaSubscriptionService
    {
        Task<string> CreateSubscription(string callbackUrl, string token, string clientId, string clientSecret);

        Task<string> ViewSubscription(string clientId, string clientSecret);

        Task<string> DeleteSubscription(long id, string clientId, string clientSecret);
    }
}