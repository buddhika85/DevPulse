# This is used to create a Docker Image on OrchestratorService API

# Use the LINUX based ASP.NET 8 runtime image as the base for running the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Set the working directory inside the container
WORKDIR /app

# Open port 80 so the app can receive HTTP traffic
EXPOSE 80

# Use the .NET SDK image to build the app (includes compilers and tools)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set working directory for build stage
WORKDIR /src

# Copy all files from your local project into the container
COPY . .

# Build and publish the OrchestratorService in Release mode to a folder named publish
RUN dotnet publish backend/OrchestratorService/OrchestratorService.csproj -c Release -o /app/publish


# Use the runtime image again for the final container
FROM base AS final

# Set working directory for the final image of this application
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /app/publish .

# Define the command to run your app when the container starts
ENTRYPOINT ["dotnet", "OrchestratorService.dll"]


# BUILD - docker build -t orchestrator-api .
