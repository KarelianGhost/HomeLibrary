FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["HomeLibrary.Web/HomeLibrary.Web.csproj", "HomeLibrary.Web/"]
COPY ["HomeLibrary.Application/HomeLibrary.Application.csproj", "HomeLibrary.Application/"]
COPY ["HomeLibrary.Domain/HomeLibrary.Domain.csproj", "HomeLibrary.Domain/"]
COPY ["HomeLibrary.Infrastructure/HomeLibrary.Infrastructure.csproj", "HomeLibrary.Infrastructure/"]

RUN dotnet restore "HomeLibrary.Web/HomeLibrary.Web.csproj"

COPY . .

WORKDIR "/src/HomeLibrary.Web"
RUN dotnet publish "HomeLibrary.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV LANG=C.UTF-8
ENV LC_ALL=C.UTF-8

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "HomeLibrary.Web.dll"]