FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
WORKDIR /opt/app

EXPOSE 80
EXPOSE 442

RUN apt update && \
    apt install unzip && \
    curl -sSL https://aka.ms/getvsdbgsh | /bin/sh /dev/stdin -v latest -l /vsdbg

COPY api.csproj src/
RUN dotnet restore src/api.csproj


COPY . src/
RUN dotnet publish -c Release --no-restore -o publish src/



ENTRYPOINT [ "dotnet", "publish/api.dll" ]

#FROM mcr.microsoft.com/dotnet/aspnet:6.0
#WORKDIR /opt/app
#COPY --from=build /opt/app/publish/ .
#ENTRYPOINT [ "dotnet", "api.dll" ]