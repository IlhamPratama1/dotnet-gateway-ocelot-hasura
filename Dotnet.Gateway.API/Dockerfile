FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Dotnet.Gateway.API.csproj", "./"]
RUN dotnet restore "./Dotnet.Gateway.API.csproj"
COPY . .
RUN dotnet build "Dotnet.Gateway.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Dotnet.Gateway.API.csproj" -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=publish /app .

EXPOSE 5145

ENTRYPOINT ["dotnet", "Dotnet.Gateway.API.dll"]


# ===== Build Script =====
# docker build -t mvdevops/dotnet-gateway:test-0.0.1 .

# ===== Push Script =====
# docker push mvdevops/dotnet-gateway:test-0.0.1
