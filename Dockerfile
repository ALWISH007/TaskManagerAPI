# Stage 1: Build the application using the SDK image
FROM mcr.microsoft.com/dotnet/sdk:9.0.304 AS build
WORKDIR /src

# Copy the project file and restore dependencies (this layer is cached if the .csproj doesn't change)
COPY ["TaskManagerAPI.csproj", "./"]
RUN dotnet restore "TaskManagerAPI.csproj"

# Copy the rest of the source code and build the application
COPY . .
RUN dotnet build "TaskManagerAPI.csproj" -c Release -o /app/build

# Install the 'ef' tool for running migrations (optional but useful)
RUN dotnet tool install --tool-path /tools --version 9.0.0 dotnet-ef

# Stage 2: Publish the application to get the optimized runtime files
FROM build AS publish
RUN dotnet publish "TaskManagerAPI.csproj" -c Release -o /app/publish

# Stage 3: Create the final runtime image (much smaller, as it doesn't contain the SDK)
FROM mcr.microsoft.com/dotnet/aspnet:9.0.3 AS final
WORKDIR /app

# Copy the published application from the 'publish' stage
COPY --from=publish /app/publish .

# Expose the port the app will run on (adjust if your app uses a different port)
EXPOSE 8080
EXPOSE 8081

# Set the environment to Production
ENV ASPNETCORE_ENVIRONMENT=Production
# Set the URLs the app should listen on
ENV ASPNETCORE_URLS=http://+:8080

# Add the tool path to the PATH environment variable
ENV PATH="/tools:${PATH}"

# Define the entry point for the container
ENTRYPOINT ["dotnet", "TaskManagerAPI.dll"]