FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["IdeaManagement.API/IdeaManagement.API.csproj", "IdeaManagement.API/"]
RUN dotnet restore "IdeaManagement.API/IdeaManagement.API.csproj"
COPY . .
WORKDIR "/src/IdeaManagement.API"
RUN dotnet build "IdeaManagement.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IdeaManagement.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdeaManagement.API.dll"]
