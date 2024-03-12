FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS development
WORKDIR /app
COPY . ./
CMD dotnet watch run --no-launch-profile --non-interactive --no-hot-reload

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build-env
WORKDIR /app

COPY DistributedRaft.Gateway.csproj .
RUN dotnet restore "DistributedRaft.Gateway.csproj"

COPY . ./

WORKDIR "/app"
RUN dotnet publish "DistributedRaft.Gateway.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app

COPY --from=build-env /app/publish .

ENTRYPOINT ["dotnet", "DistributedRaft.Gateway.dll"]
