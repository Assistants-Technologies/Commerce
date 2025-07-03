var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var env = builder.Environment.EnvironmentName;

app.MapGet("/", () => new
{
    Environment = env,
    Message = $"Running in {env} environment."
});

await app.RunAsync();