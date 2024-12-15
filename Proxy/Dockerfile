# Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the project files and restore any dependencies
COPY ["Proxy/Proxy.csproj", "Proxy/"]
RUN dotnet restore "Proxy/Proxy.csproj"

# Copy the rest of the application files and build the application
COPY ["Proxy/", "Proxy/"]
RUN dotnet publish "Proxy/Proxy.csproj" -c Release -o /app/publish

# Use the official ASP.NET Core runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:9.0-noble-chiseled
WORKDIR /app
COPY --from=build /app/publish .

# Expose the port the application runs on
EXPOSE 8080

# Run the application
ENTRYPOINT ["dotnet", "Proxy.dll"]