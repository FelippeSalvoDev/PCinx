# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia os arquivos do projeto
COPY *.csproj ./
RUN dotnet restore

# Copia o restante e publica em Release
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Porta que o Render vai expor
EXPOSE 8080

# Comando de start
ENTRYPOINT ["dotnet", "pcinx-api.dll"]
