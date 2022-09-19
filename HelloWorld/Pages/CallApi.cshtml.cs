using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IdentityModel.Client;
using System.Net.Http.Headers;

namespace HelloWorld.Pages
{
    public class CallApiModel : PageModel
    {
        internal string? ApiResponse { get; set; }
        internal string? AccessToken { get; set; }
        internal string? RefreshToken { get; set; }

        public async Task OnGetAsync()
        {
            // for HttpClientFactory usage check https://docs.duendesoftware.com/identityserver/v6/tokens/requesting/
            var tokenClient = new HttpClient();

            var tokenResponse = await tokenClient.RequestClientCredentialsTokenAsync(
                new ClientCredentialsTokenRequest
            {
                Address = "https://localhost:5001/connect/token",

                ClientId = "EDevletinBizeVerecegiClientId",
                ClientSecret = "EDevletinBizeVerecegiSecret",
                Scope = "HelloApi"
            });

            this.AccessToken = tokenResponse.AccessToken;
            this.RefreshToken = tokenResponse.RefreshToken;


            var apiClient = new HttpClient();
            
            apiClient.SetBearerToken(AccessToken);

            var apiResponse = await apiClient.GetStringAsync("https://localhost:7202/WeatherForecast");
            
            this.ApiResponse = apiResponse;

        }
    }
}
