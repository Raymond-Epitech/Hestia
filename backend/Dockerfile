FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

WORKDIR /app
EXPOSE 8080
EXPOSE 8081


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ["../", "Api/"]
RUN dotnet restore "Api/Api/Api.csproj"
COPY . .

WORKDIR /src/Api/Api

RUN dotnet build "Api.csproj" -c Release -o /app/build

FROM build AS publish

RUN dotnet publish "Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Api.dll"]
