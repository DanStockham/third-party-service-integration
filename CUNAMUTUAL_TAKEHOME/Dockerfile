FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["CUNAMUTUAL_TAKEHOME/CUNAMUTUAL_TAKEHOME.csproj", "CUNAMUTUAL_TAKEHOME/"]
RUN dotnet restore "CUNAMUTUAL_TAKEHOME/CUNAMUTUAL_TAKEHOME.csproj"
COPY . .
WORKDIR "/src/CUNAMUTUAL_TAKEHOME"
RUN dotnet build "CUNAMUTUAL_TAKEHOME.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CUNAMUTUAL_TAKEHOME.csproj" -c Release -o /app/publish


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CUNAMUTUAL_TAKEHOME.dll"]
