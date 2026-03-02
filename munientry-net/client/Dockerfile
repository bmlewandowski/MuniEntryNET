
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
# Copy project files first for layer caching
COPY client/Munientry.Client.csproj client/
COPY shared/Munientry.Shared.csproj shared/
RUN dotnet restore client/Munientry.Client.csproj
# Copy all source files
COPY client/ client/
COPY shared/ shared/
RUN dotnet publish client/Munientry.Client.csproj -c Release -o /app/publish

FROM nginx:stable-alpine
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
# Copy custom nginx config for SPA fallback
COPY --from=build /src/client/nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
