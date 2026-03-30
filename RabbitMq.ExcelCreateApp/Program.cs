using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RabbitMq.ExcelCreateApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<AppDbContext>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var appDbContext = services.GetRequiredService<AppDbContext>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    appDbContext.Database.Migrate();

    if (!appDbContext.Users.Any())
    {
        var user = new IdentityUser
        {
            UserName = "admin",
            Email = "admin@gmail.com"   
        };
        var result1 = await userManager.CreateAsync(user, "Admin123*");

        if (!result1.Succeeded)
            throw new Exception(string.Join(", ", result1.Errors.Select(e => e.Description)));

        var user2 = new IdentityUser
        {
            UserName = "mgm",
            Email = "muratgumus05@gmail.com" 
        };
        var result2 = await userManager.CreateAsync(user2, "Admin123*");

        if (!result2.Succeeded)
            throw new Exception(string.Join(", ", result2.Errors.Select(e => e.Description)));
    }

    // Create default roles
    string[] roleNames = { "Admin", "User" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
    // Create a default admin user
    var adminEmail = "";
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
