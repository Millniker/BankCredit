FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["CreditService/CreditService.csproj", "CreditService/"]
COPY ["CreditService.BL/CreditService.BL.csproj", "CreditService.BL/"]
COPY ["CreditService.Common/CreditService.Common.csproj", "CreditService.Common/"]
COPY ["CreditService.DAL/CreditService.DAL.csproj", "CreditService.DAL/"]

RUN dotnet restore "CreditService/CreditService.csproj"
COPY . .
WORKDIR "/src/CreditService"

RUN dotnet build "CreditService.csproj" -c Release -o /app/build
FROM build AS publish

RUN dotnet publish "CreditService.csproj" -c Release -o /app/publish
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

CMD  ["dotnet", "CreditService.dll"]