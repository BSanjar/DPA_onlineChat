# Установка базового образа ASP.NET Core
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Этап сборки приложения
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Копирование проекта и восстановление зависимостей
COPY ["onlineChat2/onlineChat2.csproj", "onlineChat2/"]
RUN dotnet restore "onlineChat2/onlineChat2.csproj"
COPY . .

# Сборка приложения
WORKDIR "/src/onlineChat2"
RUN dotnet build "onlineChat2.csproj" -c Release -o /app/build

# Стадия публикации
FROM build AS publish
RUN dotnet publish "onlineChat2.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Финальный образ
FROM base AS final
WORKDIR /app

# Копирование собранного приложения
COPY --from=publish /app/publish .

# Копирование папки Template из корня проекта в папку app внутри контейнера
COPY onlineChat2/TemplateFiles /app/TemplateFiles

ENTRYPOINT ["dotnet", "onlineChat2.dll"]
