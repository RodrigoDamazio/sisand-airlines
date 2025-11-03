# Etapa base: runtime leve
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

# ✅ Instala bibliotecas nativas necessárias para o BCrypt
RUN apt-get update && apt-get install -y libc6 libgcc-s1 && rm -rf /var/lib/apt/lists/*

# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Sisand.Airlines.Api/Sisand.Airlines.Api.csproj", "Sisand.Airlines.Api/"]
COPY ["Sisand.Airlines.Application/Sisand.Airlines.Application.csproj", "Sisand.Airlines.Application/"]
COPY ["Sisand.Airlines.Domain/Sisand.Airlines.Domain.csproj", "Sisand.Airlines.Domain/"]
COPY ["Sisand.Airlines.Infrastructure/Sisand.Airlines.Infrastructure.csproj", "Sisand.Airlines.Infrastructure/"]

RUN dotnet restore "Sisand.Airlines.Api/Sisand.Airlines.Api.csproj"

COPY . .
WORKDIR "/src/Sisand.Airlines.Api"
RUN dotnet build "Sisand.Airlines.Api.csproj" -c Release -o /app/build

# Etapa de publicação
FROM build AS publish
RUN dotnet publish "Sisand.Airlines.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Sisand.Airlines.Api.dll"]
