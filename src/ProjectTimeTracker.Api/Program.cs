using Microsoft.EntityFrameworkCore;
using ProjectTimeTracker.Api.Data;
using ProjectTimeTracker.Api.Middleware;
using ProjectTimeTracker.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUtentiService, UtentiService>();
builder.Services.AddScoped<IAreeService, AreeService>();
builder.Services.AddScoped<IStatiService, StatiService>();
builder.Services.AddScoped<IUrgenzeService, UrgenzeService>();
builder.Services.AddScoped<IProgettiService, ProgettiService>();
builder.Services.AddScoped<IProgettoDettagliService, ProgettoDettagliService>();
builder.Services.AddScoped<ICronologiaService, CronologiaService>();
builder.Services.AddScoped<ITempoLavoratoService, TempoLavoratoService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("WebClient", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    SeedData.Initialize(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors("WebClient");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }