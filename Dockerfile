# Add your dotnet core project build stuff here
FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env

ADD https://dl.google.com/linux/direct/google-talkplugin_current_amd64.deb /src/google-talkplugin_current_amd64.deb

# Install Chrome
RUN apt-get update && apt-get install -y \
    apt-transport-https \
    ca-certificates \
    curl \
    gnupg \
    hicolor-icon-theme \
    libcanberra-gtk* \
    libgl1-mesa-dri \
    libgl1-mesa-glx \
    libpango1.0-0 \
    libpulse0 \
    libv4l-0 \
    fonts-symbola \
    --no-install-recommends \
    && curl -sSL https://dl.google.com/linux/linux_signing_key.pub | apt-key add - \
    && echo "deb [arch=amd64] https://dl.google.com/linux/chrome/deb/ stable main" > /etc/apt/sources.list.d/google.list \
    && apt-get update && apt-get install -y \
    google-chrome-stable \
    --no-install-recommends \
    && apt-get purge --auto-remove -y curl \
    && rm -rf /var/lib/apt/lists/*

# Download ChromeDriver
RUN set -x \
    && apt-get update \
    && apt-get install -y --no-install-recommends \
    ca-certificates \
    curl \
    unzip \
    && rm -rf /var/lib/apt/lists/* \
    && curl -sSL "https://dl.google.com/linux/direct/google-talkplugin_current_amd64.deb" -o /tmp/google-talkplugin-amd64.deb \
    && dpkg -i /tmp/google-talkplugin-amd64.deb \
    && mkdir \opt\selenium \
    && curl -sSL "https://chromedriver.storage.googleapis.com/2.40/chromedriver_linux64.zip" -o /tmp/chromedriver.zip \
    && unzip -o /tmp/chromedriver -d /opt/selenium/ \
    && rm -rf /tmp/*.deb \
    && apt-get purge -y --auto-remove curl unzip

# Add chrome user
RUN groupadd -r chrome && useradd -r -g chrome -G audio,video chrome \
    && mkdir -p /home/chrome/Downloads && chown -R chrome:chrome /home/chrome
WORKDIR /app
COPY *.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet publish -c Realase -o out
COPY Resources/chromedriver.exe /out
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app
EXPOSE 80
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "DockerApi.dll"]