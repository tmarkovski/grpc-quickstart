FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the project files and restore any dependencies
COPY ["Client/Client.csproj", "Client/"]
RUN dotnet restore "Client/Client.csproj"

# Copy the rest of the application files and build the application
COPY ["Client/", "Client/"]
COPY ["./Protos", "./Protos"]
RUN dotnet publish "Client/Client.csproj" -c Release -o /app/publish

# Use the official ASP.NET Core runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:9.0-noble-chiseled
WORKDIR /app
COPY --from=build /app/publish .

# Run the application
ENTRYPOINT ["dotnet", "Client.dll"]
