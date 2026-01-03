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
