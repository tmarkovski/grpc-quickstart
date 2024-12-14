# Overview

This project sets up a gRPC-enabled Docker image with server reflection. The intention is to provide a quick-start template for creating and deploying gRPC services using ASP.NET Core.

## Project Structure

- **Dockerfile**: Defines the steps to build and run the application in a Docker container.
- **greet.proto**: Defines the gRPC service and messages.
- **appsettings.json**: Configures application settings, including logging and Kestrel server settings.
- **Program.cs**: Configures the gRPC services and HTTP request pipeline.
- **GreeterService.cs**: Implements the gRPC service defined in `greet.proto`.

## Dockerfile

The Dockerfile consists of two stages:

1. **Build Stage**:
    - Uses the official .NET SDK image to build the application.
    - Copies the project files and restores dependencies.
    - Publishes the application to the `/app/publish` directory.

2. **Runtime Stage**:
    - Uses the official ASP.NET Core runtime image to run the application.
    - Copies the published application from the build stage.
    - Exposes port 80.
    - Sets the entry point to run the application.

## gRPC Service

- **greet.proto**: Defines a `Greeter` service with a `SayHello` RPC method.
- **GreeterService.cs**: Implements the `SayHello` method to return a greeting message.

## Configuration

- **appsettings.json**: Configures logging and Kestrel server to use HTTP/2.

## Running the Application

To build and run the Docker image:

```sh
docker build -t grpc-quickstart .
docker run -p 80:80 grpc-quickstart
```

## Testing Connectivity

You can use tools like `grpcurl` to test the connectivity and functionality of the gRPC service:

```sh
grpcurl -plaintext localhost:80 list
grpcurl -plaintext -d '{"name": "World"}' localhost:80 greet.Greeter/SayHello
```

This project provides a quick and easy way to set up a gRPC service with server reflection, making it easier to test and debug using tools like `grpcurl`.
