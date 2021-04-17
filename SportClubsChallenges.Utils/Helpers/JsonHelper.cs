namespace SportClubsChallenges.Utils.Helpers
{
    using System;
    using Newtonsoft.Json;

    public static class JsonHelper
    {
        public static T Deserialize<T>(string json) where T : class
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch(Exception)
            {
                return null;
            }
        }
    }
}
