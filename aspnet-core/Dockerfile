# 请参阅 https://aka.ms/customizecontainer 以了解如何自定义调试容器，以及 Visual Studio 如何使用此 Dockerfile 生成映像以更快地进行调试。

# 此阶段用于在快速模式(默认为调试配置)下从 VS 运行时
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER root
RUN apt-get update && apt-get install -y \
    libfontconfig1 \
    libfreetype6 \
    libpng16-16 \
    libexpat1 \
    libuuid1 \
    && rm -rf /var/lib/apt/lists/*
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# 此阶段用于生成服务项目
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NuGet.Config", "."]
COPY ["src/HotelABP.HttpApi.Host/HotelABP.HttpApi.Host.csproj", "src/HotelABP.HttpApi.Host/"]
COPY ["SqlSugarSetup/SqlSugarSetup.csproj", "SqlSugarSetup/"]
COPY ["src/HotelABP.Application/HotelABP.Application.csproj", "src/HotelABP.Application/"]
COPY ["src/HotelABP.Domain/HotelABP.Domain.csproj", "src/HotelABP.Domain/"]
COPY ["src/HotelABP.Domain.Shared/HotelABP.Domain.Shared.csproj", "src/HotelABP.Domain.Shared/"]
COPY ["src/HotelABP.Application.Contracts/HotelABP.Application.Contracts.csproj", "src/HotelABP.Application.Contracts/"]
COPY ["src/HotelABP.EntityFrameworkCore/HotelABP.EntityFrameworkCore.csproj", "src/HotelABP.EntityFrameworkCore/"]
RUN dotnet restore "./src/HotelABP.HttpApi.Host/HotelABP.HttpApi.Host.csproj"
COPY . .
WORKDIR "/src/src/HotelABP.HttpApi.Host"
RUN dotnet build "./HotelABP.HttpApi.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

# 此阶段用于发布要复制到最终阶段的服务项目
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./HotelABP.HttpApi.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 此阶段在生产中使用，或在常规模式下从 VS 运行时使用(在不使用调试配置时为默认值)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HotelABP.HttpApi.Host.dll"]