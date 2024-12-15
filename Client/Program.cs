using GrpcQuickstart;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;

if (args.Contains("--help") || args.Contains("-h"))
{
    Console.WriteLine("Usage:");
    Console.WriteLine("  --endpoint <url>      (Requited) Specify the gRPC server endpoint (default: http://localhost:80)");
    Console.WriteLine("  --grpc-web            (Optional) Use gRPC-Web with HTTP/1.1");
    Console.WriteLine("  --http-version-exact  (Optional) Use exact HTTP version: HTTP/2 for gRPC, HTTP/1.1 if --grpc-web is specified.");
    Console.WriteLine("                        If this flag is not specified, the client will try to negotiate the highest HTTP version supported by the server.");
    return;
}

var grpcWeb = args.Contains("--grpc-web");
var endpointIndex = Array.IndexOf(args, "--endpoint");
var endpoint = (endpointIndex >= 0 && endpointIndex < args.Length - 1) ? args[endpointIndex + 1] : "http://localhost:80";
var httpVersionExact = args.Contains("--http-version-exact");

var options = new GrpcChannelOptions
{
    HttpHandler = grpcWeb ? new GrpcWebHandler(new HttpClientHandler()) : new HttpClientHandler(),
    HttpVersion = grpcWeb ? new Version(1, 1) : new Version(2, 0),
    HttpVersionPolicy = httpVersionExact ? HttpVersionPolicy.RequestVersionExact : HttpVersionPolicy.RequestVersionOrHigher
};

var service = new Greeter.GreeterClient(GrpcChannel.ForAddress(endpoint, options));

var response = await service.SayHelloAsync(new HelloRequest { Name = "World" });

Console.WriteLine(response);