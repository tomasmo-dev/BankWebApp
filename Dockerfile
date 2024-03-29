﻿FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BankWebApp.csproj", "./"]
RUN dotnet restore "BankWebApp.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "BankWebApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "BankWebApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN apt-get update -y
RUN apt-get install -y curl
RUN apt-get install -y jq

ENTRYPOINT ["dotnet", "BankWebApp.dll"]
