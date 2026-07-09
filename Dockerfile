FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY CeyloneNature.sln .
COPY src/CeyloneNature.Domain/CeyloneNature.Domain.csproj src/CeyloneNature.Domain/
COPY src/CeyloneNature.Application/CeyloneNature.Application.csproj src/CeyloneNature.Application/
COPY src/CeyloneNature.Infrastructure/CeyloneNature.Infrastructure.csproj src/CeyloneNature.Infrastructure/
COPY src/CeyloneNature.Api/CeyloneNature.Api.csproj src/CeyloneNature.Api/
RUN dotnet restore CeyloneNature.sln

COPY . .
RUN dotnet publish src/CeyloneNature.Api/CeyloneNature.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "CeyloneNature.Api.dll"]
