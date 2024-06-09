#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BackupR/BackupR.csproj", "BackupR/"]
RUN dotnet restore "BackupR/BackupR.csproj"
COPY . .
WORKDIR "/src/BackupR"
RUN dotnet build "BackupR.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BackupR.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=mysql:8.4.0 /usr/bin/mysqldump /usr/bin
ENTRYPOINT ["dotnet", "BackupR.dll"]