    using EmployeeManagement.Models;
using EmployeeManagement.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//Add Services!! 
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<EmployeeDbContext>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();
builder.Services.AddSingleton<DataProtectionPurposeStrings>();



// Cliam Authorization 
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminRolePolicy", policy => policy.RequireRole("Admin"));
    options.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("Delete Role"));

    options.AddPolicy("EditRolePolicy", policy => policy.AddRequirements(new ManageAdminRolesClaimsRequirement()));
});


//Token Lifespan for Email Confirmation and Password Confirmation 
builder.Services.Configure<DataProtectionTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromMinutes(2));




//Google Authentication 
//builder.Services.AddAuthentication()
//    .AddGoogle(options =>
//    {
//        options.ClientId = "37447553159-q7ed7npbnmssod0pnragg0gach9ulqv8.apps.googleusercontent.com";
//        options.ClientSecret = "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";
//    });




//Identity for Authorization and Authentication
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 10;
    options.SignIn.RequireConfirmedEmail = true;
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);

}).AddEntityFrameworkStores<EmployeeDbContext>()
.AddDefaultTokenProviders();





var app = builder.Build();

// Adding Middleware
if(app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseStatusCodePagesWithReExecute("/Error/{0}");
}

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultControllerRoute();


app.Run();  
