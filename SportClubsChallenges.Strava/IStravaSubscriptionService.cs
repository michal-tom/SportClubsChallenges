namespace SportClubsChallenges.Strava
{
    using System.Threading.Tasks;

    public interface IStravaSubscriptionService
    {
        Task<string> CreateSubscription(string callbackUrl, string token);

        Task<string> ViewSubscription();

        Task<string> DeleteSubscription(long id);
    }
}