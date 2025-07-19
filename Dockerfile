# Use the official .NET 8 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy solution file and project files
COPY QuizPlatform.sln .
COPY src/Quiz.Api/Quiz.Api.csproj src/Quiz.Api/
COPY src/Quiz.CSharp.Api/Quiz.CSharp.Api.csproj src/Quiz.CSharp.Api/
COPY src/Quiz.CSharp.Data/Quiz.CSharp.Data.csproj src/Quiz.CSharp.Data/
COPY src/Quiz.Infrastructure/Quiz.Infrastructure.csproj src/Quiz.Infrastructure/
COPY src/Quiz.Shared/Quiz.Shared.csproj src/Quiz.Shared/

# Restore dependencies
RUN dotnet restore

# Copy the rest of the source code
COPY src/ src/

# Build the application
RUN dotnet publish src/Quiz.Api/Quiz.Api.csproj -c Release -o out

# Use the official .NET 8 runtime image for running
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Create a non-root user
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Copy the built application from the build stage
COPY --from=build-env /app/out .

# Expose the port the app runs on
EXPOSE 8080

# Set the entry point
ENTRYPOINT ["dotnet", "Quiz.Api.dll"] 