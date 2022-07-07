FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY Epam.Library.WebAPI.csproj Epam.Library.WebAPI.csproj
RUN dotnet restore Epam.Library.WebAPI.csproj
COPY . .
WORKDIR /src
RUN dotnet build Epam.Library.WebAPI.csproj -c Release -o /app/

FROM build AS publish
RUN dotnet publish Epam.Library.WebAPI.csproj -c Release -o /app/

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Epam.Library.WebAPI.dll"]
