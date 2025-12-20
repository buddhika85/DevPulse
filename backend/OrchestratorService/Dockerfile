# This is used to create a Docker Image for the OrchestratorService API

# Use the LINUX based ASP.NET 8 runtime image as the base for running the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Set the working directory inside the container
WORKDIR /app

# Open port 8080 so the app can receive HTTP traffic
EXPOSE 8080

# Use the .NET SDK image to build the app (includes compilers and tools)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set working directory for build stage
WORKDIR /src

# Copy the entire repository into the container
# IMPORTANT: Because azure-pipelines-backend-ochestrator.yml is inside the root folder but the Dockerfile is inside backend/OrchestratorService,
# we must go UP TWO LEVELS to copy the whole repo.
#COPY ../../ ./
# This copies the entire repo into /src (because your WORKDIR is /src).
COPY . .


# Build and publish the OrchestratorService in Release mode
RUN dotnet publish backend/OrchestratorService/OrchestratorService.csproj -c Release -o /app/publish

# Use the runtime image again for the final container
FROM base AS final

# Set working directory for the final image
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /app/publish .

# Define the command to run your app when the container starts
ENTRYPOINT ["dotnet", "OrchestratorService.dll"]







# Pushing Docker image to AZURE CONTAINER REGISTRY Steps
# 0 From the root -  C:\BUDDHIKA\DevPulse-P4\DevPulse

# 1 BUILD - docker build -f backend/OrchestratorService/Dockerfile -t orchestrator-api .
# 2 LOCAL TEST - docker run -p 5000:8080 orchestrator-api					# maps host port 5000 → exposed container port 8080 - Test - http://localhost:5000/swagger

# 3 LOGIN TO ACR via Docker - docker login devpulseacr-cta8h8feeua4cchz.azurecr.io			

# 4 TAG / Label - docker tag orchestrator-api devpulseacr-cta8h8feeua4cchz.azurecr.io/orchestrator-api:v1.0.0
# 5 PUSH to ACR - docker push devpulseacr-cta8h8feeua4cchz.azurecr.io/orchestrator-api:v1.0.0
