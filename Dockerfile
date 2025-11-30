FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /App

COPY ./Directory.Build.props ./
COPY ./Nuget.Docker.Config ./Nuget.Config
COPY ./.editorconfig ./
COPY ./src/ .

RUN dotnet --version
RUN ls -la
RUN dotnet restore ./Cocktails.Api/Cocktails.Api.csproj --configfile ./Nuget.Config
RUN dotnet publish ./Cocktails.Api/Cocktails.Api.csproj -c Release -o ./out

FROM mcr.microsoft.com/dotnet/aspnet:9.0
EXPOSE 8080
EXPOSE 443

ENV ASPNETCORE_URLS="https://+;http://+"
ENV ASPNETCORE_ENVIRONMENT="docker"
ENV ASPNETCORE_HTTPS_PORTS=443
ENV ASPNETCORE_Kestrel__Certificates__Default__Password="password"
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx

WORKDIR /App
COPY --from=build /App/out .
ENTRYPOINT ["dotnet", "Cocktails.Api.dll"]
