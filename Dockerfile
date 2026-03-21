# ===== ビルド用 =====
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /app
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o out

# ===== 実行用 =====
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview
WORKDIR /app
COPY --from=build /app/out .
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000
ENTRYPOINT ["dotnet", "WebApplication5.dll"]

# SDKイメージでビルド
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ["WebApplication5.csproj", "./"]
RUN dotnet restore "./WebApplication5.csproj"

COPY . .
RUN dotnet publish "WebApplication5.csproj" -c Release -o /app/publish

# ランタイムイメージ
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 80
ENTRYPOINT ["dotnet", "WebApplication5.dll"]