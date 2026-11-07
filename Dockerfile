# ------------------------------------------------------------------ build ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Primero el csproj para aprovechar la caché de restore
COPY Coactivos_Predial.csproj ./
RUN dotnet restore Coactivos_Predial.csproj

# Después el resto del código
COPY . .
RUN dotnet publish Coactivos_Predial.csproj -c Release -o /app --no-restore

# ------------------------------------------------------------------ run ------
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /app .

# El runtime escucha en el 8080 (ASPNETCORE_HTTP_PORTS por defecto en la imagen)
EXPOSE 8080

ENTRYPOINT ["dotnet", "Coactivos_Predial.dll"]
