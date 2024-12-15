using GrpcQuickstart;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;

if (args.Contains("--help") || args.Contains("-h"))
{
    Console.WriteLine("Usage:");
    Console.WriteLine("  --grpc-web       Use gRPC-Web with HTTP/1.1");
    Console.WriteLine("  --endpoint <url> Specify the gRPC server endpoint (default: http://localhost:80)");
    return;
}

var grpcWeb = args.Contains("--grpc-web");
var endpointIndex = Array.IndexOf(args, "--endpoint");
var endpoint = (endpointIndex >= 0 && endpointIndex < args.Length - 1) ? args[endpointIndex + 1] : "http://localhost:80";

var options = new GrpcChannelOptions
{
    HttpHandler = grpcWeb ? new GrpcWebHandler(new HttpClientHandler()) : new HttpClientHandler(),
    HttpVersion = grpcWeb ? new Version(1, 1) : new Version(2, 0)
};

var service = new Greeter.GreeterClient(GrpcChannel.ForAddress(endpoint, options));

var response = await service.SayHelloAsync(new HelloRequest { Name = "World" });

Console.WriteLine(response);