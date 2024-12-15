using Microsoft.AspNetCore.Server.Kestrel.Core;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Forwarder;

if (int.TryParse(Environment.GetEnvironmentVariable("PORT") ?? "8080", out var port) && port is < 1 or > 65535)
{
  throw new Exception("PORT must be a valid integer between 1 and 65535");
}

if (!Enum.TryParse(Environment.GetEnvironmentVariable("PROTOCOL") ?? "http1", true, out HttpProtocols httpProtocols))
{
  throw new Exception("PROTOCOL must be a valid HttpProtocols value");
}

if (Environment.GetEnvironmentVariable("TARGET") is not { } target || Uri.TryCreate(target, UriKind.Absolute, out _) is false)
{
  throw new ArgumentException("TARGET environment variable is required and must be a valid URL. " +
                              "Example: http://localhost:5000");
}

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel(options =>
{
  options.ListenAnyIP(port, listenOptions =>
  {
    listenOptions.Protocols = httpProtocols;
  });
});

builder.Services.AddReverseProxy()
  .LoadFromMemory([
    new RouteConfig
    {
      RouteId = "route1",
      Match = new RouteMatch
      {
        Path = "{**catch-all}"
      },
      ClusterId = "cluster1"
    }
  ], [
    new ClusterConfig
    {
      ClusterId = "cluster1",
      HttpRequest = new ForwarderRequestConfig
      {
        Version = new Version(2, 0),
        VersionPolicy = HttpVersionPolicy.RequestVersionExact
      },
      Destinations = new Dictionary<string, DestinationConfig>
      {
        {
          "destination1", new DestinationConfig
          {
            Address = target
          }
        }
      }
    }
  ]);

var logger = LoggerFactory.Create(logging =>
{
  logging.AddConsole();
  logging.SetMinimumLevel(LogLevel.Debug);
}).CreateLogger("Server");

logger.LogInformation("Listening on port {Port} using {HttpProtocols} protocol", port, httpProtocols);
logger.LogInformation("Proxying requests to {Target} using HTTP2 protocol", target);

var app = builder.Build();
app.MapReverseProxy();
app.Run();