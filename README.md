# SportClubsChallenges

SportClubsChallenges is web application to define sport challenges based on activities from Strava clubs.

Application build using Blazor with .NET Core 3.x and stored on Azure.

To set up application you'll need to edit configuration files as follows:

1. `commonsettings.json`
```json
{
	"SportClubsChallengeAzureFunctionsUrl": "APP_AZURE_FUNCTIONS_URL",
	"StravaClientId": "YOUR_STRAVA_CLIENT_ID", 
	"StravaClientSecret": "YOUR_STRAVA_CLIENT_SECRET",
}
```

2. `connectionstrings.json`
```json
{
  "ConnectionStrings": {
    "SportClubsChallengesDbConnString": "DB_CONNECTIONSTRING",
    "SportClubsChallengeStorage": "APP_AZURE_STORAGE_ADDRESS"
  }
}
```

Strava Client Id and Secret can be configured here: https://www.strava.com/settings/api

Frameworks and components used to build application:
* **AspNet.Security.OAuth.Providers** - ASP.NET Core Strava OAuth authentication - https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers
* **Strava-NET** - the C# library for the Strava API v3 https://github.com/timheuer/strava-net
* **Blazority** - Blazor component library based on Clarity UI - https://blazority.com
