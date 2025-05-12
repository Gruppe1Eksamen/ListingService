FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

RUN apt-get update && apt-get install -y curl

COPY --from=build /out .

EXPOSE 5136
ENV ASPNETCORE_URLS=http://+:5136

ENTRYPOINT ["dotnet", "ListingService.dll"]
