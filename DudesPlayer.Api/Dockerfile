#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DudesPlayer.Api/DudesPlayer.Api.csproj", "DudesPlayer.Api/"]
COPY ["ClassLibrary/DudesPlayer.Library.csproj", "ClassLibrary/"]
RUN dotnet restore "DudesPlayer.Api/DudesPlayer.Api.csproj"
COPY . .
WORKDIR "/src/DudesPlayer.Api"
RUN dotnet build "DudesPlayer.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DudesPlayer.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DudesPlayer.Api.dll"]