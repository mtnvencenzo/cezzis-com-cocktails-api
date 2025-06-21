FROM mcr.microsoft.com/dotnet/sdk:9.0 as build
WORKDIR /App

COPY ./Directory.Build.props ./
COPY ./Nuget-Docker.Config ./Nuget.Config
COPY ./.editorconfig ./
COPY ./cocktails.api ./

RUN ls -la
RUN dotnet restore ./src/Cocktails.Api/Cocktails.Api.csproj --configfile ./Nuget.Config
RUN dotnet publish ./src/Cocktails.Api/Cocktails.Api.csproj -c Release -o ./out

FROM mcr.microsoft.com/dotnet/aspnet:9.0
EXPOSE 8080
EXPOSE 8081

ENV ASPNETCORE_ENVIRONMENT="local"

WORKDIR /App
COPY --from=build /App/out .
ENTRYPOINT ["dotnet", "Cocktails.Api.dll"]
