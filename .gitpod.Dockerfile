FROM mcr.microsoft.com/dotnet/sdk:7.0 as build
WORKDIR /build-dir
COPY [".",  "." ]
RUN dotnet build -c:Debug
RUN dotnet build -c:Release --no-restore




# Create Debug image
#FROM build as demo_debug
#WORKDIR /build-dir/Host
#ENTRYPOINT ["dotnet", "bin/Debug/net5.0/FusionDemo.HealthCentral.Host.dll"]

# Install custom tools, runtimes, etc.
# For example "bastet", a command-line tetris clone:
# RUN brew install bastet
#
# More information: https://www.gitpod.io/docs/config-docker/
