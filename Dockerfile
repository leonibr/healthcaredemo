FROM mcr.microsoft.com/dotnet/sdk:5.0 as build
WORKDIR /build-dir
COPY [".",  "." ]
RUN dotnet build -c:Debug
RUN dotnet build -c:Release --no-restore




# Create Debug image
FROM build as demo_debug
WORKDIR /build-dir/Host
ENTRYPOINT ["dotnet", "bin/Debug/net5.0/FusionDemo.HealthCentral.Host.dll"]



FROM build as publish
WORKDIR /build-dir
RUN dotnet publish -c:Release -f:net5.0 --no-build --no-restore Host/Host.csproj


FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine as runtime
#RUN apk add icu-libs libx11-dev
#RUN apk add libgdiplus-dev \
#  --update-cache --repository http://dl-3.alpinelinux.org/alpine/edge/testing/ --allow-untrusted
# ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
WORKDIR /dist
COPY --from=publish /build-dir .


FROM runtime as healthcare_demo
WORKDIR /dist/Host
ENTRYPOINT ["dotnet", "bin/Release/net5.0/publish/FusionDemo.HealthCentral.Host.dll"]