using Canvas.Core;
using Canvas.Core.Http;
using Canvas.Core.Settings;
using QuickTest;

var settings = AppSettings.New<Program>("appsettings.json");

Console.WriteLine($"Canvas URL is {settings.CanvasUrl}");
var client = new ClientConfiguration()
    .ViaHttp(settings.CanvasUrl, settings.CanvasToken)
    .Build();
var user = await client.CurrentUser.Retrieve();
Console.WriteLine($"Current user is {user?.Name}");

var count = 0;
await foreach (var course in client.Courses.ListForCurrentUser(new CourseList { PageSize = List.MaxPageSize }))
{
    Console.WriteLine($"Course #{++count}: {course.Name} [{course.Term?.Name}]");
}