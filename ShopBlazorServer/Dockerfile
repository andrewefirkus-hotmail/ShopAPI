# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["ShopBlazorServer/ShopBlazorServer.csproj", "ShopBlazorServer/"]
COPY ["ShopShared/ShopShared.csproj", "ShopShared/"]
RUN dotnet restore "ShopBlazorServer/ShopBlazorServer.csproj"

COPY . .
WORKDIR "/src/ShopBlazorServer"
RUN dotnet publish "ShopBlazorServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ShopBlazorServer.dll"]
