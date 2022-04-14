#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["BuxferToYNAB/BuxferToYNAB.csproj", "BuxferToYNAB/"]
RUN dotnet restore "BuxferToYNAB/BuxferToYNAB.csproj"
COPY . .
WORKDIR "/src/BuxferToYNAB"
RUN dotnet build "BuxferToYNAB.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BuxferToYNAB.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BuxferToYNAB.dll"]