FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Atoms.Web/Atoms.Web.csproj", "Atoms.Web/"]
COPY ["src/Atoms.Core/Atoms.Core.csproj", "Atoms.Core/"]
COPY ["src/Atoms.Infrastructure/Atoms.Infrastructure.csproj", "Atoms.Infrastructure/"]
COPY ["src/Atoms.UseCases/Atoms.UseCases.csproj", "Atoms.UseCases/"]
RUN dotnet restore "Atoms.Web/Atoms.Web.csproj"
COPY . .
WORKDIR "/src/Atoms.Web"
RUN dotnet publish "Atoms.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Atoms.Web.dll"]
