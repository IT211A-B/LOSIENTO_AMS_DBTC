FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["AMS_DBTC_API.csproj", "./"]
RUN dotnet restore "AMS_DBTC_API.csproj"
COPY . .
RUN dotnet publish "AMS_DBTC_API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AMS_DBTC_API.dll"]
