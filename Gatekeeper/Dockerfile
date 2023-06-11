FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Gatekeeper/Gatekeeper.csproj", "Gatekeeper/"]
RUN dotnet restore "Gatekeeper/Gatekeeper.csproj"
COPY . .
WORKDIR "/src/Gatekeeper"
RUN dotnet build "Gatekeeper.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Gatekeeper.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gatekeeper.dll"]
