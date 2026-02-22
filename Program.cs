using EventEase.Middleware;
using EventEase.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// 注册自定义服务
builder.Services.AddSingleton<EventService>();
builder.Services.AddScoped<AttendanceTrackerService>();
builder.Services.AddScoped<SessionStateService>();
builder.Services.AddScoped<ProtectedLocalStorage>();

// 添加日志服务
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();