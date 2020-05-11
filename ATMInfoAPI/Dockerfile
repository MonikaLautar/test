#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
ADD bankart-ca.crt /usr/local/share/ca-certificates/bankart-ca.crt
RUN chmod 644 /usr/local/share/ca-certificates/bankart-ca.crt
ADD bankart-ca2.crt /usr/local/share/ca-certificates/bankart-ca2.crt
RUN chmod 644 /usr/local/share/ca-certificates/bankart-ca2.crt && update-ca-certificates
WORKDIR /src
COPY ["ATMInfoAPI/ATMInfoAPI.csproj", "ATMInfoAPI/"]
RUN dotnet restore "ATMInfoAPI/ATMInfoAPI.csproj"
COPY . .
WORKDIR "/src/ATMInfoAPI"
RUN dotnet build "ATMInfoAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ATMInfoAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ATMInfoAPI.dll"]