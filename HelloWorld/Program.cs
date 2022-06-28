using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizePage("/ListMyClaims");
});

builder.Services.AddAuthentication(opt => { 
        opt.DefaultScheme = "cookie";
        opt.DefaultChallengeScheme = "oidc";
    })
    .AddCookie("cookie")
    .AddOpenIdConnect("oidc", opt =>
    {
        opt.Authority = "https://localhost:5001";
        opt.ClientId = "EDevletinBizeVerecegiClientId";
        opt.ClientSecret = "EDevletinBizeVerecegiSecret";

        opt.ResponseType = "code";
        opt.UsePkce = true;
        opt.ResponseMode = "query";

        opt.TokenValidationParameters.RoleClaimType = "role";
        opt.TokenValidationParameters.NameClaimType = "name";


        // opt.CallbackPath = "/signin-oidc"; // default redirect URI
        //opt.Scope.Add("oidc"); // default scope
        //opt.Scope.Add("profile"); // default scope
        //opt.Scope.Add("role");        
        

        // ek claim bilgilerini login sonrasi otomatik cek.
        opt.GetClaimsFromUserInfoEndpoint = true;

        // 1. TCKN bilgisini talep et.
        opt.Scope.Add("EDevletTemelBilgileri");

        // TCKN standart bir oidc alanı olmadığı için bir mapping gerekli.
        opt.ClaimActions.MapUniqueJsonKey("tckn", "tckn"); 

        // save claims to asp.net cookie
        opt.SaveTokens = true;

        
        // require users to log in every time, even when they are logged in. (optional)
        //opt.Events.OnRedirectToIdentityProvider = ctx =>
        //{
        //    ctx.ProtocolMessage.Prompt = "login";
        //    return Task.CompletedTask;
        //};
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
