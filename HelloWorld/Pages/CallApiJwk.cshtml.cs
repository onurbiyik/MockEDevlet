using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IdentityModel.Client;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualBasic;
using Microsoft.IdentityModel.Tokens;
using IdentityModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HelloWorld.Pages
{
    public class CallApiJwkModel : PageModel
    {
        internal string? ApiResponse { get; set; }
        internal string? AccessToken { get; set; }
        internal string? RefreshToken { get; set; }

        // public+private keypair from mkjwk.org
        private static string rsaKey = "{\r\n    \"p\": \"3JpM1XvutaABnA5JY5XsbbvSHK4vy7Bu-8U1LwNkyvyikIfBis_YcxeZppEzuJTnS-pgWKPhIxY3Eu8lLsDtYw\",\r\n    \"kty\": \"RSA\",\r\n    \"q\": \"lktDYj1ezGB1tt3f4M9DLk3RQAUxHbcP7vCw0QFlDsH43rRbhDZzIa_Yti-KpCaZm3J5xCVyCylk5uGeMoXcUw\",\r\n    \"d\": \"Yb5k6wOWsNB3TRV_0lpBpwQkJxcQUHNMCeyu2V_jmTKd8J1p0MfDxAd4TXgbIJmOwNIKfiDgyazFtIyJmIVJT8zdFRFCEOJa6TCX6A8UZWtzw3Mc1gZSdWO9lmKEIj9eGGocv_7G43M7_7ALrjLTD0ftszwXit-jPgpJJ0BS7bU\",\r\n    \"e\": \"AQAB\",\r\n    \"kid\": \"EEEEE\",\r\n    \"qi\": \"c9fhtXwdw0sVgKG731WvDbNxIYqq2Mrns_s_QHz1oSMuLKOtjUi-SmtceSuSFhwFAPLLcl_XmR1zjMQTn29r3w\",\r\n    \"dp\": \"1n5I-sI3xe0_42aYwPWDHuciUrSi0GBKaQ7EuWOJWzztK65z5u5UvQNTaHuCWJSfmFbZOxaMzzrIbeaMtrrrlQ\",\r\n    \"alg\": \"RS256\",\r\n    \"dq\": \"HZ_cOmMKapKg4Vi-plt0TI4-LrZXRhotY9XBpJD30H7fhVZbq6Xn773vo4mfdFe_c2qPIeCXqCeSogfa3O6RzQ\",\r\n    \"n\": \"gYNESqM465kLLryEepgjKRw6ywWM8IcMo_PEt3xaf81F0prcrRuDHgqh_zR9priYuA4tmYFyOjxbXraPt3NE7_XxLBv01eMT9gJglW-gN-_Edo9OURMaXVUGxAWB-rkXnTtJRVwzRNZ2ViDq3ZCF1MG6uer9K7IJAML77p79Cxk\"\r\n}";


        public async Task OnGetAsync()
        {

            var jwk = new JsonWebKey(rsaKey);

            var signingCred = new SigningCredentials(jwk, "RS256");

            var tokenClient = new HttpClient();

            var disco = await tokenClient.GetDiscoveryDocumentAsync("https://localhost:5001");
            if (disco.IsError) throw new Exception(disco.Error);

            var clientToken = CreateClientToken(signingCred, "EDevletinBizeVerecegiClientId", "HelloApi");
            var tokenResponse = await tokenClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientAssertion =
                {
                    Type = OidcConstants.ClientAssertionTypes.JwtBearer,
                    Value = clientToken
                },
                Scope = "HelloApi"
            });

            if (!string.IsNullOrEmpty(tokenResponse.Error)) throw new Exception(tokenResponse.Raw);

            this.AccessToken = tokenResponse.AccessToken;
            this.RefreshToken = tokenResponse.RefreshToken;


            var apiClient = new HttpClient();
            apiClient.SetBearerToken(AccessToken);

            var apiResponse = await apiClient.GetStringAsync("https://localhost:7202/WeatherForecast");

            this.ApiResponse = apiResponse;
        }
        private static string CreateClientToken(SigningCredentials credential, string clientId, string audience)
        {
            var now = DateTime.UtcNow;

            var token = new JwtSecurityToken(
                issuer: clientId,
                audience: audience,
                claims: new Claim[]
                {
                    new Claim(JwtClaimTypes.JwtId, Guid.NewGuid().ToString()),
                    new Claim(JwtClaimTypes.Subject, clientId),
                    new Claim(JwtClaimTypes.IssuedAt, now.ToEpochTime().ToString(), ClaimValueTypes.Integer64)
                },
                notBefore: now,
                expires: now.AddMinutes(1),
                signingCredentials: credential
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
    }
}
