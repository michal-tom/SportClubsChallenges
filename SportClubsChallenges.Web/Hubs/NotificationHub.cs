namespace SportClubsChallenges.Web.Hubs
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR;

    public class NotificationHub : Hub
    {
        public const string HubUrl = "/notifications";

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            var requestQuery = Context.GetHttpContext().Request.Query;
            if (requestQuery.ContainsKey("athleteId"))
            {
                var athleteId = requestQuery["athleteId"].ToString();
                await Groups.AddToGroupAsync(Context.ConnectionId, athleteId);
            }
        }

        public async Task NotifyLoggedUsers(long[] athleteIds, string message)
        {
            foreach (var athleteId in athleteIds)
            {
                await Clients.Group(athleteId.ToString()).SendAsync("Notify", message);
            }
        }
    }
}