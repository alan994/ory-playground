FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
WORKDIR /opt/app

EXPOSE 80
EXPOSE 442

COPY api.csproj src/
RUN dotnet restore src/api.csproj


COPY . src/
RUN dotnet publish -c Release --no-restore -o publish src/

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /opt/app
COPY --from=build /opt/app/publish/ .
ENTRYPOINT [ "dotnet", "api.dll" ]