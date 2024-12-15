using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Server.Services;

if (int.TryParse(Environment.GetEnvironmentVariable("PORT") ?? "80", out var port) && port is < 1 or > 65535)
{
    throw new Exception("PORT must be a valid integer between 1 and 65535");
}

if (!Enum.TryParse(Environment.GetEnvironmentVariable("PROTOCOL") ?? "http2", true, out HttpProtocols httpProtocols))
{
    throw new Exception("PROTOCOL must be a valid HttpProtocols value");
}

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel(options =>
{
    options.ListenAnyIP(port, listenOptions =>
    {
        listenOptions.Protocols = httpProtocols;
    });
});

var logger = LoggerFactory.Create(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Debug);
}).CreateLogger("Server");

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

builder.Services.AddCors(o => o.AddPolicy("AllowAll", policy => policy
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
    .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding")));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });
app.UseCors();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.MapGrpcService<GreeterService>()
    .EnableGrpcWeb()
    .RequireCors("AllowAll");
app.MapGrpcReflectionService();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

logger.LogInformation("Configured server to listen on port {Port} using {HttpProtocols} protocol", port, httpProtocols);

app.Run();
