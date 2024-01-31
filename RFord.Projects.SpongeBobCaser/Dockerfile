#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.


# The below is runnable in the following manner:
# 1. set the `test-server-pfx-password` secret, it needs to be the password for the PFX file
#  head -n1 | tr -d '\n' | podman secret create test-server-pfx-password -
# 2. run the container
#  podman \
#    run \
#    --detach \
#    --rm \
#    --volume testitem:/etc/pki/tls/private \
#    --publish 443:8081 \
#    --env ASPNETCORE_Kestrel__Endpoints__TestEndpoint__Url=https://services.babbypod.local:8081/ \
#    --env ASPNETCORE_Kestrel__Endpoints__TestEndpoint__Certificate__Path=/etc/pki/tls/private/server.pfx \
#    --secret test-server-pfx-password,type=env,target=ASPNETCORE_Kestrel__Endpoints__TestEndpoint__Certificate__Password \
#    rfordprojectsspongebobcaser:latest
#  


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# so for TLS certs, the private keypair needs to be in `/etc/pki/tls/private/`,
# and the public cert needs to be in `/etc/pki/tls/certs/`.  However, kestrel
# only supports loading from a PFX file, so we'll put the file in 
# `/etc/pki/tls/private/`.  We can put that config in the Kestrel JSON, and then
# supply the PFX password via environment variables injected by the docker host.

# --- experimental -------------------------------------------------------------

# make the directory
RUN mkdir -p /etc/pki/tls/private/

# well, TFTP is out because it uses multiple ports for control/data
# mDNS is not supported inside the aspnet container, so IP or proper DNS only

# download the certificate
#	1. requires python http server (to serve the https cert to the container build)
#	2. requires HOSTS mapping (to allow domain name -> IP)
#	3. dockerfile.env file to set the certificate parameters
RUN apt update && apt install -y curl
RUN curl --output-dir /etc/pki/tls/private --remote-name http://192.168.0.54/services-noir-local.pfx
RUN apt -y remove curl && apt -y autoremove
# can't copy from a local resource.  because why would you be able to.  it's on a VM/WSL instance.

# --------------------------------------------------------- end experimental ---

USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
#EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["RFord.Projects.SpongeBobCaser/RFord.Projects.SpongeBobCaser.csproj", "RFord.Projects.SpongeBobCaser/"]
RUN dotnet restore "./RFord.Projects.SpongeBobCaser/./RFord.Projects.SpongeBobCaser.csproj"
COPY . .
WORKDIR "/src/RFord.Projects.SpongeBobCaser"
RUN dotnet build "./RFord.Projects.SpongeBobCaser.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RFord.Projects.SpongeBobCaser.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RFord.Projects.SpongeBobCaser.dll"]

