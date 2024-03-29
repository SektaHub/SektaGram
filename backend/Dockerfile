FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8081
EXPOSE 7294

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["backend.csproj", "."]
RUN dotnet restore "backend.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "backend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Ensure execute permissions for the entire wwwroot folder
RUN chmod -R +x /app/wwwroot

# Set the ASPNETCORE_URLS environment variable
ENV ASPNETCORE_URLS=http://+:8081

ENTRYPOINT ["dotnet", "backend.dll"]
