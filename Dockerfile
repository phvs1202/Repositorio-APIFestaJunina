# Etapa base: imagem com .NET para rodar a aplicação
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["API-AdmFestaJunina/API-AdmFestaJunina.csproj", "API-AdmFestaJunina/"]
RUN dotnet restore "API-AdmFestaJunina/API-AdmFestaJunina.csproj"

# Copia o restante do código
COPY . .
WORKDIR /src/API-AdmFestaJunina
RUN dotnet publish -c Release -o /app/publish

# Etapa final
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "API-AdmFestaJunina.dll"]
