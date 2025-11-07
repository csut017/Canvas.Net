using Canvas.Core;
using Canvas.Core.Http;
using QuickTest;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

var settings = AppSettings.New<Program>("appsettings.json");

Log.Information("Canvas URL is {url}", settings.CanvasUrl);
var client = new ClientConfiguration()
    .ViaHttpWithResponseLogging(settings.CanvasUrl, settings.CanvasToken)
    .UseLogger(Log.Logger)
    .Build();
var user = await client.CurrentUser.Retrieve();
Log.Information("Current user is {user}", user);