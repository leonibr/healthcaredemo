FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine as build
WORKDIR /build-dir
ADD Host/Host.csproj Host/Host.csproj
ADD UI/UI.csproj UI/UI.csproj
ADD Domain/Domain.csproj Domain/Domain.csproj
ADD Abstractions/Abstractions.csproj Abstractions/Abstractions.csproj
ADD Services/Services.csproj Services/Services.csproj
ADD FusionDemo.sln .
RUN dotnet restore
ADD . ./
RUN dotnet build -c:Debug --no-restore
RUN dotnet build -c:Release --no-restore




# Create Debug image
FROM build as demo_debug
WORKDIR /build-dir/Host
ENTRYPOINT ["dotnet", "bin/Debug/net7.0/FusionDemo.HealthCentral.Host.dll"]



FROM build as publish
WORKDIR /build-dir
RUN dotnet publish -c:Release -f:net7.0 --no-build --no-restore Host/Host.csproj


FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine as runtime
RUN apk add icu-libs libx11-dev
RUN apk add libgdiplus-dev \
    --update-cache --repository http://dl-3.alpinelinux.org/alpine/edge/testing/ --allow-untrusted
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
WORKDIR /dist
COPY --from=publish /build-dir/Host/bin/Release/net7.0/publish .


FROM runtime as healthcare_demo
WORKDIR /dist
ENV VIRTUAL_HOST healthcare.marques.top
ENV LETSENCRYPT_HOST healthcare.marques.top
ENTRYPOINT ["dotnet", "FusionDemo.HealthCentral.Host.dll"]