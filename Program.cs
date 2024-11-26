using System.Collections.Concurrent;
using WebTvs.Services;
using WebTvs.Hubs;
using WebTvs.Models;

var builder = WebApplication.CreateBuilder(args);

// SignalR'ý ekleyin
builder.Services.AddSignalR();

// UDPListenerService ve ConcurrentBag<TvsCheck>'i ekleyin
builder.Services.AddSingleton<ConcurrentBag<TvsCheck>>();
builder.Services.AddHostedService<UDPListenerService>();

// MVC'yi ekleyin
builder.Services.AddControllersWithViews();

var app = builder.Build();

// SignalR route'unu ekleyin
app.MapHub<UdpDataHub>("/udpDataHub");
app.UseStaticFiles();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
