
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
# Copy csproj and restore as distinct layers
COPY Munientry.Client.csproj ./
RUN dotnet restore
# Copy all source files and folders
COPY . ./
RUN dotnet publish -c Release -o /app/publish

FROM nginx:stable-alpine
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
# Copy custom nginx config for SPA fallback
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
