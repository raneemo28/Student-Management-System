FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

RUN mkdir -p /app/Storage

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["App.APIs/App.APIs.csproj", "App.APIs/"]
COPY ["App.Application/App.Application.csproj", "App.Application/"]
COPY ["App.domain/App.domain.csproj", "App.domain/"]
COPY ["App.infra/App.infra.csproj", "App.infra/"]

RUN dotnet restore "App.APIs/App.APIs.csproj"

COPY . .

WORKDIR "/src/App.APIs"
RUN dotnet build "App.APIs.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "App.APIs.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "App.APIs.dll"]