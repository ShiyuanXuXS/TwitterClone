using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TwitterClone.Data;
using TwitterClone.Models;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddRazorPages(/*options => {
    options.Conventions.AuthorizePage("main");
} */);
builder.Services.AddHttpClient();

builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddJsonFile("appsettings.json");
builder.Services.AddDbContext<TwitterCloneDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddIdentity<User, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
.AddEntityFrameworkStores<TwitterCloneDbContext>()
.AddDefaultTokenProviders();

//blob storage config
var blobStorageConn = builder.Configuration.GetConnectionString("BlobStorageConnection");
builder.Services.AddAzureClients(azureBuilder =>
{
    azureBuilder.AddBlobServiceClient(blobStorageConn);
});

builder.Services.Configure<IdentityOptions>(options =>
{
    //Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 5;
    //Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    //User settings
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";
    options.User.RequireUniqueEmail = true;
    //allow users with EmailConfirmed value 0/false to log in
    options.SignIn.RequireConfirmedAccount = false;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    //Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(25);
    options.LoginPath = "/Login";
    options.LogoutPath = "/Logout";
    options.AccessDeniedPath = "/AccessDenied";
    options.SlidingExpiration = true;
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

using (var scope = app.Services.CreateScope())
{
    void SeedUsersAndRoles(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        string[] roleNameList = new string[] { "User", "Admin" };
        foreach (string roleName in roleNameList)
        {
            if (!roleManager.RoleExistsAsync(roleName).Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = roleName;
                IdentityResult result = roleManager.CreateAsync(role).Result;
                foreach (IdentityError error in result.Errors)
                {
                    //TODO: Log it
                }
            }
        }
        //For testing only. Do not do it on a production system
        //Create an Administrator
        string adminEmail = "admin@admin.com";
        string adminPass = "Admin123!";
        if (userManager.FindByNameAsync(adminEmail).Result == null)
        {
            User user = new User();
            user.UserName = adminEmail;
            user.Email = adminEmail;
            user.EmailConfirmed = true;
            IdentityResult result = userManager.CreateAsync(user, adminPass).Result;
            if (result.Succeeded)
            {
                //Fixme: error my be returned by this call - log it
                var result2 = userManager.AddToRoleAsync(user, "Admin").Result;
                if (!result2.Succeeded)
                {
                    //fixme: log the error
                }
            }
            else
            {
                //fixme: log the error 
            }
        }
    }

    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    SeedUsersAndRoles(userManager, roleManager);
}


app.UseRouting();
app.UseStatusCodePagesWithRedirects("NotFound");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();
