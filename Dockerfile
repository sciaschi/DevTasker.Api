# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build + publish
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files first (good cache behavior)
COPY ["DevTasker.Api/DevTasker.Api.csproj", "DevTasker.Api/"]
COPY ["DevTasker.Domain/DevTasker.Domain.csproj", "DevTasker.Domain/"]
COPY ["DevTasker.Infrastructure/DevTasker.Infrastructure.csproj", "DevTasker.Infrastructure/"]

RUN dotnet restore "DevTasker.Api/DevTasker.Api.csproj"

# Now copy the rest of the source
COPY . .

# Build + publish the API project
WORKDIR "/src/DevTasker.Api"
RUN dotnet publish "DevTasker.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DevTasker.Api.dll"]
