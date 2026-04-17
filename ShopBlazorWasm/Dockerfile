# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["ShopBlazorWasm/ShopBlazorWasm.csproj", "ShopBlazorWasm/"]
COPY ["ShopShared/ShopShared.csproj", "ShopShared/"]
RUN dotnet restore "ShopBlazorWasm/ShopBlazorWasm.csproj"

COPY . .
WORKDIR "/src/ShopBlazorWasm"
RUN dotnet publish "ShopBlazorWasm.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Serve the WASM static files via nginx
FROM nginx:alpine AS final
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
COPY ShopBlazorWasm/nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
