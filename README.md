# SportClubsChallenges

Application to define sport challenges based on activities from Strava clubs.

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
