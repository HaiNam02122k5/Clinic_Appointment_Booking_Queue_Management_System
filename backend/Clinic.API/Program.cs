var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Keep the first backend smoke test tiny: prove the API host runs before adding layers back.
app.MapGet("/", () => "Hello world!");

app.Run();
