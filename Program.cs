using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using static OpenIddict.Abstractions.OpenIddictConstants;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

IServiceCollection services = builder.Services;

//services.AddAuthentication(options =>
//{
//    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//})
    services.AddAuthentication(options =>
    {
        //options.DefaultScheme = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })

       .AddCookie(//"OpenIddict.Validation.AspNetCore",
       options =>
       {
           options.LoginPath =  "/login";//"https://localhost:7236/Identity/Account/Login";//
           options.ExpireTimeSpan = TimeSpan.FromMinutes(50);
           options.SlidingExpiration = false;

           ////https://github.com/openiddict/openiddict-core/issues/689
           //options.Events.OnRedirectToLogin = context =>
           //{
           //    // If a tenant identifier was attached to the challenge properties, update the login page URL.
           //    if (context.Properties.Items.TryGetValue("tenant_id", out string tenant))
           //    {
           //        context.RedirectUri = QueryHelpers.AddQueryString(context.RedirectUri, "tenant_id", tenant);
           //    }

           //    context.Response.Redirect(context.RedirectUri);

           //    return Task.CompletedTask;
           //};


       })

       .AddOpenIdConnect(options =>
       {
            // Note: these settings must match the application details
            // inserted in the database at the server level.
            options.ClientId = "mvc";
           options.ClientSecret = "901564A5-E7FE-42CB-B10D-61EF6A8F3654";

           options.RequireHttpsMetadata = false;
           options.GetClaimsFromUserInfoEndpoint = true;
           options.SaveTokens = true;

            // Use the authorization code flow.
            options.ResponseType = OpenIdConnectResponseType.Code;
           options.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;

            // Note: setting the Authority allows the OIDC client middleware to automatically
            // retrieve the identity provider's configuration and spare you from setting
            // the different endpoints URIs or the token validation parameters explicitly.
            options.Authority = "https://localhost:7236/";

           options.Scope.Add("email");
           options.Scope.Add("roles");
           //options.Scope.Add("api_scope");


           // Disable the built-in JWT claims mapping feature.
           options.MapInboundClaims = false;

           options.TokenValidationParameters.NameClaimType = "name";
           options.TokenValidationParameters.RoleClaimType = "role";


           
       });

services.AddControllersWithViews();

services.AddHttpClient();


////To be implemented in Client app side with policies
//services.AddSingleton<IAuthorizationHandler, AccessHandler>();
//services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>();
//services.AddSingleton<DataProtectionPurposeStrings>();
//services.AddMvc(options =>
//{
//    var policy = new AuthorizationPolicyBuilder()
//                    .RequireAuthenticatedUser()
//                    .Build();
//    options.Filters.Add(new AuthorizeFilter(policy));
//}).AddXmlSerializerFormatters();
//services.AddAuthorization(options =>
//{
//    options.AddPolicy("DeleteRolePolicy",
//        policy => policy.RequireClaim("Delete Role"));

//    options.AddPolicy("EditRolePolicy",
//        policy => policy.AddRequirements(new ManageRolesAndClaimsRequirement()));

//    options.AddPolicy("AdminRolePolicy",
//        policy => policy.RequireRole("Admin"));
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
