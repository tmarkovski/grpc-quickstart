# Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the project files and restore any dependencies
COPY ["Server/Server.csproj", "Server/"]
COPY ["./Protos", "./Protos"]
RUN dotnet restore "Server/Server.csproj"

# Copy the rest of the application files and build the application
COPY ["Server/", "Server/"]
RUN dotnet publish "Server/Server.csproj" -c Release -o /app/publish

# Use the official ASP.NET Core runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expose the port the application runs on
EXPOSE 80

# Run the application
ENTRYPOINT ["dotnet", "Server.dll"]