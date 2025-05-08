# Etapa base: imagem com .NET para rodar a aplicação
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["API-AdmFestaJunina.csproj", "./"]
RUN dotnet restore "API-AdmFestaJunina.csproj"

COPY . .
RUN dotnet build "API-AdmFestaJunina.csproj" -c Release -o /app/build

# Etapa de publish
FROM build AS publish
RUN dotnet publish "API-AdmFestaJunina.csproj" -c Release -o /app/publish

# Etapa final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API-AdmFestaJunina.dll"]
