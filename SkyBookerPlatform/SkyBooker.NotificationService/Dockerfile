FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SkyBooker.NotificationService.csproj", "./"]
RUN dotnet restore "./SkyBooker.NotificationService.csproj"
COPY . .
RUN dotnet build "SkyBooker.NotificationService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SkyBooker.NotificationService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SkyBooker.NotificationService.dll"]
