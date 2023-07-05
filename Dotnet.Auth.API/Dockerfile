FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Dotnet.Auth.API.csproj", "./"]
RUN dotnet restore "./Dotnet.Auth.API.csproj"
COPY . .
RUN dotnet build "Dotnet.Auth.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Dotnet.Auth.API.csproj" -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=publish /app .

EXPOSE 5287

ENTRYPOINT ["dotnet", "Dotnet.Auth.API.dll"]


# ===== Build Script =====
# docker build -t mvdevops/dotnet-api:test-0.0.1 .

# ===== Push Script =====
# docker push mvdevops/dotnet-api:test-0.0.1
