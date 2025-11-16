using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ardelean_Victor_Dan_Lab2.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
});

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Books");
    options.Conventions.AllowAnonymousToPage("/Books/Index");
    options.Conventions.AllowAnonymousToPage("/Books/Details");
    
    options.Conventions.AuthorizeFolder("/Members", "RequireAdminRole");
    options.Conventions.AuthorizeFolder("/Publishers", "RequireAdminRole");
    options.Conventions.AuthorizeFolder("/Categories", "RequireAdminRole");
});

builder.Services.AddDbContext<Ardelean_Victor_Dan_Lab2Context>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Ardelean_Victor_Dan_Lab2Context")
        ?? throw new InvalidOperationException("ConnStr 'Ardelean_Victor_Dan_Lab2Context' not found.")
    ));

builder.Services.AddDbContext<LibraryIdentity2Context>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("LibraryIdentity2ContextConnection")
        ?? throw new InvalidOperationException("ConnStr 'LibraryIdentity2ContextConnection' not found.")
    ));

builder.Services
    .AddDefaultIdentity<IdentityUser>(opts => { opts.SignIn.RequireConfirmedAccount = true; })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<LibraryIdentity2Context>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = { "User", "Admin" };

    foreach (var role in roles)
    {
        var roleExist = await roleManager.RoleExistsAsync(role);
        if (!roleExist)
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
