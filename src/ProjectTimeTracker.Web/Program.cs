using ProjectTimeTracker.Web.Api;
using ProjectTimeTracker.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".ProjectTimeTracker.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromHours(8);
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient<IApiClient, ApiClient>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var baseUrl = configuration["ApiSettings:BaseUrl"]
        ?? throw new InvalidOperationException("Configurazione ApiSettings:BaseUrl mancante.");

    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddScoped<IUserSessionService, UserSessionService>();
builder.Services.AddScoped<IDashboardUiService, DashboardUiService>();
builder.Services.AddScoped<ILookupUiService, LookupUiService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapRazorPages();

app.Run();