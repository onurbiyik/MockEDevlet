using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
        private static string rsaKey = @"{
    ""p"": ""4S40kLM4cdQWBJ5rjC_zZ3BOsSlwIXhDgF4gEzXPG7mVboNOhV3QRfDVAFTNUuvHF3ttpwi7qRaLN_gO1Q605HCWVFjn7HexhbApkdRCYvWKqh0kY1hbDr9JYEo5uQLJ3ztj5cHNf-TCll3--yvEXyFc-tgu8inYI1YaAHqe4cc"",
    ""kty"": ""RSA"",
    ""q"": ""wl3dQUDpCe2INH6WUiWprmsShyjnPIxnKo7enyswEitz9YDhEF6oAIsIMZO2J_-SCm3NVxKxUW-wNPjn03WH29aNJL1cYKbBBHN4smNuqYfF217v0T9ZnKqttvPlqlocDR3wLCHj3-8wJ7Dx0PkCZx5gkR-vvrm0W_P5waG8PIU"",
    ""d"": ""VihscRIa-GqqDlma5wBNIhNcgRyjPVISFs_otZC3lTx5y6vjJ0eu57j14oUxijSmMOyQQkcXFKGKFUPJpK6ThYvgfskGhzY7EXXezNHxgexWy-TFXizyUioYtpi_xK_ws3Xw7jYn1eWBtuycjkKwJjv9SnTtKguviHmsuAfFXtJO7cLt1UFJFkeDnmCBZhQdoIWEUk6GihoQRZ9lAhxgMO9pRNpET831-co1wt708Yd-gex4ipis7B8cS4R6YGRSQbDB3UZQE5sSVhrhKJBAGYsHN_zaspicWe4bvgpCmlkV9MSEaYvu0hPBF7rGQz6pd2xOPAc6qZEeuzhheFGOSQ"",
    ""e"": ""AQAB"",
    ""use"": ""sig"",
    ""kid"": ""/43b2M7rVvKBshOg7TptUPpsFTo="",
    ""qi"": ""WDaErntTxFks5Fk6Wuem_u_uo49bHU0cTfxAzOFpTRpjFNuggaaw_SFLb0HxtOx2V_205PPOtrVxsFKPqruPQEIjEHTbm46SrwJ7ycRZBXMOYhhff5Z7uaynxQjaLxSrwvybxwXc_d7l4QLIcbToIc9USw3wG_oxIf1S0npq6sE"",
    ""dp"": ""VAy5FiGvOd2d8k9neUYUplFnLf13myaWm32Gn2UATyrwOfXNAz7HFLQV7S-6C-9xurQZc7VwxmZhxJxUfPYmUvYrRoSA3OS4YFwmyAXgWauo_GIwSNyb8F859jT-Yq5Fx4sVeJbkK46pixK-r-XgQFnlqMO0kjNIN7hVsEQUtdU"",
    ""alg"": ""RS256"",
    ""dq"": ""sqdyartOOLjjnFTKuQEzYUW7xw561XW8kiq2SBl7Wwgj0wI-XTyUMcOZqXA7W7KXOwzIVIOzBsahcV9kP0LluNUR8TY5BgHgosW8qEOrARrya3xs9KnFJbLgb6zpalbfN9NP4lDYUwpY-fKzosBvS_dpDZvRf37UYDz2Udvm2L0"",
    ""n"": ""qveUPQmWribiRDFWq9az1OA_aKSLwSHUXg2vWiJ4i_zcakxHTg28kz3MK9V2MaPvFWBBmPM2lvu8DxU439t6TsNsp3gtdH_J_KybQzd6ZhEqtA7Mk_FGyP4jhYLGHf65W72nCFNTfStpYaEa-SpRMY4BtW9QAg2Lv4zTb5Uy6C8y3DlZKOro3fgPIbyMJxJKHOrlbbeq2nNLtwU-ZLJ1vEsJl6412KPR7zIzAnJxBldjg1MmKITrHv5jR8SPrD8wo83_Jw6uUZtRgpKYdCGSInqT6OwdFitlE1854Fcnzp8wFflN6GCWCEn73tXenXB4TaD9Ea9h_sb3k12v5pnwYw""
}";


        public async Task OnGetAsync()
        {

            var jwk = new JsonWebKey(rsaKey);

            var signingCred = new SigningCredentials(jwk, "RS256");

            var tokenClient = new HttpClient();

            var disco = await tokenClient.GetDiscoveryDocumentAsync("https://localhost:5001");
            if (disco.IsError) throw new Exception(disco.Error);

            var clientToken = CreateClientToken(signingCred, "EDevletinBizeVerecegiClientId", disco.TokenEndpoint);
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
