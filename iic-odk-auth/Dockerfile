#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["iic-odk-auth.csproj", "."]
RUN dotnet restore "./iic-odk-auth.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "iic-odk-auth.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "iic-odk-auth.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS="http://0.0.0.0:5000"
ENV ASPNETCORE_ENVIRONMENT="Production"
ENTRYPOINT ["dotnet", "iic-odk-auth.dll"]