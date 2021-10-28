FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["XorusDiscordBot/XorusDiscordBot.csproj", "XorusDiscordBot/"]
RUN dotnet restore "XorusDiscordBot/XorusDiscordBot.csproj"
COPY . .
WORKDIR "/src/XorusDiscordBot"
RUN dotnet build "XorusDiscordBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "XorusDiscordBot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV TZ=Europe/Paris
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone
ENTRYPOINT ["dotnet", "XorusDiscordBot.dll"]
