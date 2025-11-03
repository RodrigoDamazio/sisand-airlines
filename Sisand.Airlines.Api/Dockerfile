# =============================
# 🚀 Etapa base: runtime leve
# =============================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

# ✅ Instala dependências básicas (caso use bibliotecas nativas)
RUN apt-get update && apt-get install -y libc6 libgcc-s1 && rm -rf /var/lib/apt/lists/*

# =============================
# 🧱 Etapa de build: SDK completo
# =============================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# ✅ Cache de pacotes NuGet
ENV NUGET_PACKAGES=/root/.nuget/packages

# Copia apenas os .csproj primeiro (melhor cache)
COPY ["Sisand.Airlines.Api/Sisand.Airlines.Api.csproj", "Sisand.Airlines.Api/"]
COPY ["Sisand.Airlines.Application/Sisand.Airlines.Application.csproj", "Sisand.Airlines.Application/"]
COPY ["Sisand.Airlines.Domain/Sisand.Airlines.Domain.csproj", "Sisand.Airlines.Domain/"]
COPY ["Sisand.Airlines.Infrastructure/Sisand.Airlines.Infrastructure.csproj", "Sisand.Airlines.Infrastructure/"]

# ✅ Restaura pacotes com cache otimizado
RUN dotnet restore "Sisand.Airlines.Api/Sisand.Airlines.Api.csproj" --disable-parallel

# Copia o restante do código
COPY . .
WORKDIR "/src/Sisand.Airlines.Api"

# 🔨 Compila o projeto (com restore já feito)
RUN dotnet build "Sisand.Airlines.Api.csproj" -c Release -o /app/build

# =============================
# 📦 Etapa de publicação
# =============================
FROM build AS publish
RUN dotnet publish "Sisand.Airlines.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# =============================
# 🏁 Etapa final: execução
# =============================
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# ✅ Exponha a porta padrão do container
EXPOSE 8080

# ✅ Entry point para execução da API
ENTRYPOINT ["dotnet", "Sisand.Airlines.Api.dll"]
