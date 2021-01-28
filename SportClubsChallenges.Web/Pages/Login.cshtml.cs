namespace SportClubsChallenges.Web.Pages
{
    using System.Threading.Tasks;
    using AspNet.Security.OAuth.Strava;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    public class LoginModel : PageModel
    {
        public async Task OnGetAsync(string redirectUri)
        {
            if (string.IsNullOrEmpty(redirectUri))
            {
                redirectUri = Url.Content("~");
            }

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                Response.Redirect(redirectUri);
            }

            await HttpContext.ChallengeAsync(
               StravaAuthenticationDefaults.AuthenticationScheme,
               new AuthenticationProperties { RedirectUri = "/" });
        }
    }
}
