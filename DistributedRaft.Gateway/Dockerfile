FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY DistributedRaft.Gateway.csproj .
RUN dotnet restore "DistributedRaft.Gateway.csproj"

COPY . ./

WORKDIR "/app"
RUN dotnet publish "DistributedRaft.Gateway.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build-env /app/publish .

ENV ASPNETCORE_HTTP_PORTS= 
ENTRYPOINT ["dotnet", "DistributedRaft.Gateway.dll"]

EXPOSE 8080
EXPOSE 443
