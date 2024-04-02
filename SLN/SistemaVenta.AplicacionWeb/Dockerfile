#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SistemaVenta.AplicacionWeb/nextadvisordotnet.AppWeb.csproj", "SistemaVenta.AplicacionWeb/"]
COPY ["SistemaVenta.BLL/nextadvisordotnet.BLL.csproj", "SistemaVenta.BLL/"]
COPY ["SistemaVenta.DAL/nextadvisordotnet.DAL.csproj", "SistemaVenta.DAL/"]
COPY ["SistemaVenta.Entity/nextadvisordotnet.Entity.csproj", "SistemaVenta.Entity/"]
COPY ["SistemaVenta.IOC/nextadvisordotnet.IOC.csproj", "SistemaVenta.IOC/"]
RUN dotnet restore "SistemaVenta.AplicacionWeb/nextadvisordotnet.AppWeb.csproj"
COPY . .
WORKDIR "/src/SistemaVenta.AplicacionWeb"
RUN dotnet build "nextadvisordotnet.AppWeb.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "nextadvisordotnet.AppWeb.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "nextadvisordotnet.AppWeb.dll"]