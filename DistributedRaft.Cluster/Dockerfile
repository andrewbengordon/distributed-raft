FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY DistributedRaft.Cluster.csproj .
RUN dotnet restore "DistributedRaft.Cluster.csproj"

COPY . ./

WORKDIR "/app"
RUN dotnet publish "DistributedRaft.Cluster.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build-env /app/publish .

ENV ASPNETCORE_HTTP_PORTS=
ENTRYPOINT ["dotnet", "DistributedRaft.Cluster.dll"]

EXPOSE 80
EXPOSE 443
