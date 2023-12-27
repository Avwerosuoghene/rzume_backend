#Build Stage
# COPY . .
# RUN dotnet restore "./RzumeAPI/RzumeAPI.csproj" --disable-parallel
# RUN dotnet publish "./RzumeAPI/RzumeAPI.csproj" -c release -o /app --no-restore

# #Serve
# FROM mcr.microsoft.com/dotnet/sdk:6.0-focal
# WORKDIR /app
# COPY  --from=build /app ./

# EXPOSE 6000

# ENTRYPOINT ["dotnet", "RzumeAPI.dll"]



# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy and restore as distinct layers
COPY *.sln .
COPY RzumeAPI/RzumeAPI.csproj ./RzumeAPI/
RUN dotnet restore

# Copy everything else and build
COPY . .
WORKDIR /app/RzumeAPI
RUN dotnet build -c Release --no-restore

# Publish the application
FROM build AS publish
RUN dotnet publish -c Release -o out --no-restore

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=publish /app/RzumeAPI/out ./

# Expose port and set entry point
EXPOSE 80
ENTRYPOINT ["dotnet", "RzumeAPI.dll"]
