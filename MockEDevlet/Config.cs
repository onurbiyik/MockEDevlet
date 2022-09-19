using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Aydipi;

public static class Config
{
    public static List<TestUser> Users => new ()
        {
            new TestUser { Username = "onur", Password = "123", SubjectId = "AAAAAAA9-0000-434B-0000-A3932222DABE", Claims = new List<Claim> { new (JwtClaimTypes.Email, "onur@mockdevlet.com"), new (JwtClaimTypes.Role, "admin"), new ("tckn", "00000701"), new ("name", "ONUR BIYIK"), new("given_name", "Onur"), new("family_name", "Biyik") } },
            new TestUser { Username = "arzu", Password = "123", SubjectId = "BBBBBBB9-0000-434B-0000-A3932222DABE", Claims = new List<Claim> { new (JwtClaimTypes.Email, "arzu@mockdevlet.com"), new (JwtClaimTypes.Role, "admin"), new ("tckn", "00000702"), new ("name", "ARZU BATAK"), new("given_name", "Arzu"), new("family_name", "Batak") } },
            new TestUser { Username = "elif", Password = "123", SubjectId = "CCCCCCC9-0000-434B-0000-A3932222DABE", Claims = new List<Claim> { new (JwtClaimTypes.Email, "elif@mockdevlet.com"), new (JwtClaimTypes.Role, "admin"), new ("tckn", "00000703"), new ("name", "ELİF BUTİK"), new("given_name", "Elif"), new("family_name", "Butik") } },
            new TestUser { Username = "mert", Password = "123", SubjectId = "DDDDDDD9-0000-434B-0000-A3932222DABE", Claims = new List<Claim> { new (JwtClaimTypes.Email, "mert@mockdevlet.com"), new (JwtClaimTypes.Role, "admin"), new ("tckn", "00000704"), new ("name", "MERT BORAK"), new("given_name", "Mert"), new("family_name", "Borak") } },
        };    

    public static IEnumerable<IdentityResource> IdentityResources => new []
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new IdentityResources.Phone(),
            new IdentityResources.Address(),
            new IdentityResource
            {
                Name = "EDevletTemelBilgileri",
                UserClaims = new List<string> {"tckn"}
            },
            new IdentityResource
            {
                Name = "VergiBilgileri",
                UserClaims = new List<string> {"vergino", "vergimuafiyeti"}
            }
        };

    public static IEnumerable<ApiScope> ApiScopes => new ApiScope[]
    {
        new ApiScope("HelloApi", "Our Hello API", 
            userClaims: new [] { "WeatherAccess" })
    };


    public static IEnumerable<Client> Clients => new [] { 
        new Client
        {
            ClientId = "EDevletinBizeVerecegiClientId",
            ClientName = "Hello World Uygulamasi",
            ClientSecrets = new List<Secret> {
                new Secret("EDevletinBizeVerecegiSecret".Sha256()),
                new Secret
                    {
                        Type = IdentityServerConstants.SecretTypes.JsonWebKey,
                        Value = @"{
    ""kty"": ""RSA"",
    ""e"": ""AQAB"",
    ""use"": ""sig"",
    ""kid"": ""/43b2M7rVvKBshOg7TptUPpsFTo="",
    ""alg"": ""RS256"",
    ""n"": ""qveUPQmWribiRDFWq9az1OA_aKSLwSHUXg2vWiJ4i_zcakxHTg28kz3MK9V2MaPvFWBBmPM2lvu8DxU439t6TsNsp3gtdH_J_KybQzd6ZhEqtA7Mk_FGyP4jhYLGHf65W72nCFNTfStpYaEa-SpRMY4BtW9QAg2Lv4zTb5Uy6C8y3DlZKOro3fgPIbyMJxJKHOrlbbeq2nNLtwU-ZLJ1vEsJl6412KPR7zIzAnJxBldjg1MmKITrHv5jR8SPrD8wo83_Jw6uUZtRgpKYdCGSInqT6OwdFitlE1854Fcnzp8wFflN6GCWCEn73tXenXB4TaD9Ea9h_sb3k12v5pnwYw""
}"
                    }
            },

            // this app will be using code flow for authorizing end users
            // this app will also use client credentials flow for calling api (machine-to-machine)
            AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,

            RedirectUris = new string[] {"https://localhost:7214/signin-oidc"},            
            AllowedScopes = new string[]
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                IdentityServerConstants.StandardScopes.Email,
                "EDevletTemelBilgileri",
                "HelloApi" // this client is allowed to call hello api.
            },

            Claims = new ClientClaim[]{ new ClientClaim("WeatherAccess", "read") },

            // clients may have different sso lifes
            UserSsoLifetime = 100, // seconds

            RequirePkce = true,
            AllowPlainTextPkce = false,
            
            // TODO: learn what this is
            // PostLogoutRedirectUris = { "https://localhost:7214/signout-callback-oidc" },

            // Server-to-Server single sign out
            // This is Idp initiated sign-out scenario
            // call this url once the user signs out from MockEdevlet IdP,
            BackChannelLogoutUri = "https://localhost:7214/backchannel-logout",

            // Client-to-Server single sign out. 
            // This is Idp initiated sign-out scenario.
            // opens an iframe or smthng mimicking the user signing out.
            FrontChannelLogoutUri = "https://localhost:7214/frontchannel-logout",

        }};

    public static IEnumerable<OidcProvider> OidcProviders => new OidcProvider[]
    {
    };
}