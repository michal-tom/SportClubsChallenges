namespace SportClubsChallenges.Model.Strava
{
    using Newtonsoft.Json;

    public class StravaEvent
    {
        [JsonProperty("object_type")]
        public ObjectType ObjectType { get; set; }

        [JsonProperty("object_id")]
        public long ObjectId { get; set; }

        [JsonProperty("aspect_type")]
        public ActionType ActionType { get; set; }

        [JsonProperty("updates")]
        public UpdateData Updates { get; set; }

        [JsonProperty("owner_id")]
        public long AthleteId { get; set; }

        [JsonProperty("subscription_id")]
        public int SubscriptionId { get; set; }

        [JsonProperty("event_time")]
        public long EventTime { get; set; }

    }

    public enum ActionType
    {
        Create,
        Update,
        Delete
    }

    public enum ObjectType
    {
        Activity,
        Athlete
    }

    public class UpdateData
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("private")]
        public bool? Private { get; set; }

        [JsonProperty("authorized")]
        public bool Authorized { get; set; }
    }
}