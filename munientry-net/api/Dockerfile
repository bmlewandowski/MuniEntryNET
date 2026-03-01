FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
# Copy project file first for layer caching
COPY api/Munientry.Api.csproj api/
RUN dotnet restore api/Munientry.Api.csproj
# Copy source and publish
COPY api/ api/
RUN dotnet publish api/Munientry.Api.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "Munientry.Api.dll"]
