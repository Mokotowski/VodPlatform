using Microsoft.EntityFrameworkCore;
using VodPlatform.Database;
using VodPlatform.Services.Interface;
using VodPlatform.Services;
using Microsoft.AspNetCore.Identity;
using VodPlatform.Models;
using VodPlatform.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// Configure Identity.
builder.Services.AddIdentity<UserModel, IdentityRole>(options =>
{
    // Password settings.
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 2;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;

    // Sign-in settings.
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedAccount = false;

    // User settings.
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;

}).AddEntityFrameworkStores<DatabaseContext>()
  .AddDefaultTokenProviders();




builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddScoped<ILogout, AccountAuthenticationServies>();
builder.Services.AddScoped<IRegister, AccountAuthenticationServies>();
builder.Services.AddScoped<ILogin, AccountAuthenticationServies>();

builder.Services.AddScoped<ISendEmail, EmailActionsServies>();
builder.Services.AddScoped<IFunctionsFromEmail, EmailActionsServies>();
builder.Services.AddScoped<ILibraryActions, LibraryServices>();
builder.Services.AddScoped<ILibraryData, LibraryServices>();

builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<RoleManager<IdentityRole>>();

builder.Services.AddScoped<IPlaylistPermissionServices, PlaylistPermissionServices>();


builder.Services.AddScoped<VideoServices>();



var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserModel>>();

    await SeedData.Initialize(scope.ServiceProvider, roleManager, userManager);
}


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAll");
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
