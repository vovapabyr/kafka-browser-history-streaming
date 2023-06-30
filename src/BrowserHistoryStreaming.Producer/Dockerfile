FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5222

ENV ASPNETCORE_URLS=http://+:5222

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/BrowserHistoryStreaming.Producer/BrowserHistoryStreaming.Producer.csproj", "src/BrowserHistoryStreaming.Producer/"]
RUN dotnet restore "src/BrowserHistoryStreaming.Producer/BrowserHistoryStreaming.Producer.csproj"
COPY . .
WORKDIR "/src/src/BrowserHistoryStreaming.Producer"
RUN dotnet build "BrowserHistoryStreaming.Producer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BrowserHistoryStreaming.Producer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BrowserHistoryStreaming.Producer.dll"]
