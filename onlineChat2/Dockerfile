# ��������� �������� ������ ASP.NET Core
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# ���� ������ ����������
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# ����������� ������� � �������������� ������������
COPY ["onlineChat2/onlineChat2.csproj", "onlineChat2/"]
RUN dotnet restore "onlineChat2/onlineChat2.csproj"
COPY . .

# ������ ����������
WORKDIR "/src/onlineChat2"
RUN dotnet build "onlineChat2.csproj" -c Release -o /app/build

# ������ ����������
FROM build AS publish
RUN dotnet publish "onlineChat2.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ��������� �����
FROM base AS final
WORKDIR /app

# ����������� ���������� ����������
COPY --from=publish /app/publish .

# ����������� ����� Template �� ����� ������� � ����� app ������ ����������
COPY onlineChat2/TemplateFiles /app/TemplateFiles

ENTRYPOINT ["dotnet", "onlineChat2.dll"]
