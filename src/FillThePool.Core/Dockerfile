FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY FillThePool.Core/FillThePool.Core.csproj src/FillThePool.Core/
RUN dotnet restore src/FillThePool.Core/FillThePool.Core.csproj
COPY . .
WORKDIR /src/FillThePool.Core
RUN dotnet build FillThePool.Core.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish FillThePool.Core.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "FillThePool.Core.dll"]
