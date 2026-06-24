FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY nuget.config .
COPY src/DigiDocumentConverter.Core/*.csproj src/DigiDocumentConverter.Core/
COPY src/DigiDocumentConverter.Api/*.csproj src/DigiDocumentConverter.Api/
RUN dotnet restore src/DigiDocumentConverter.Api/DigiDocumentConverter.Api.csproj
COPY src/ src/
RUN dotnet publish src/DigiDocumentConverter.Api/DigiDocumentConverter.Api.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
# QuestPDF needs libfontconfig/libfreetype for text layout
RUN apt-get update && apt-get install -y --no-install-recommends libfontconfig1 && rm -rf /var/lib/apt/lists/*
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080
HEALTHCHECK --interval=30s --timeout=5s CMD curl -f http://localhost:8080/health || exit 1
ENTRYPOINT ["dotnet", "DigiDocumentConverter.Api.dll"]
