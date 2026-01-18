using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using SubFlow.Application.Abstractions;
using SubFlow.Application.Services.Commands;
using SubFlow.Application.Services.Queries;
using SubFlow.Application.Subscriptions.Commands;
using SubFlow.Application.Subscriptions.Queries;
using SubFlow.Infrastructure.Persistence;
using SubFlow.Infrastructure.Persistence.Repositories;
using SubFlow.Web.Localization;
using SubFlow.Web.Pages.Services;

var builder = WebApplication.CreateBuilder(args);

// Localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Razor Pages + localization
builder.Services
    .AddRazorPages()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

// DB
builder.Services.AddDbContext<SubFlowDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SubFlowDb")));

// App abstractions (temporary demo user until Identity)
builder.Services.AddScoped<ICurrentUser, DemoCurrentUser>();

// Repositories
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();

// Use-cases (Subscriptions)
builder.Services.AddScoped<GetSubscriptionsQuery>();
builder.Services.AddScoped<CreateSubscriptionCommand>();

// Use-cases (Services)
builder.Services.AddScoped<GetServicesQuery>();
builder.Services.AddScoped<CreateServiceCommand>();

var app = builder.Build();

// Configure request localization (ru-RU default)
var supportedCultures = new[]
{
    new CultureInfo("ru-RU"),
    new CultureInfo("en-US")
};

var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("ru-RU"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};

// чтобы можно было переключать культуру query-string'ом: ?culture=en-US
localizationOptions.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());

app.UseRequestLocalization(localizationOptions);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
