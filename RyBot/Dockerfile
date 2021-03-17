FROM mcr.microsoft.com/dotnet/aspnet:3.1

#ENV PUPPETEER_EXECUTABLE_PATH "/usr/bin/google-chrome-unstable"
COPY bin/Release/netcoreapp3.1/ RyBot/
WORKDIR /RyBot
ENTRYPOINT ["dotnet", "RyBot.dll"]