# Add your dotnet core project build stuff here
FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app
COPY *.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet publish -c Realase -o out
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app
COPY Resources/chromedriver.exe .
COPY Resources/chromedriver .
COPY tessdata /app/tessdata
EXPOSE 80
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "DockerApi.dll"]