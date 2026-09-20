FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["App.LoggingMicroservice/App.LoggingMicroservice.csproj", "App.LoggingMicroservice/"]

RUN dotnet restore "App.LoggingMicroservice/App.LoggingMicroservice.csproj"

COPY . .

WORKDIR "/src/App.LoggingMicroservice"
RUN dotnet build "App.LoggingMicroservice.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "App.LoggingMicroservice.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "App.LoggingMicroservice.dll"]