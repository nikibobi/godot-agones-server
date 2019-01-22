FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY GodotAgones.Server/*.csproj ./GodotAgones.Server/
RUN dotnet restore

COPY GodotAgones.Server/. ./GodotAgones.Server/
WORKDIR /app/GodotAgones.Server
RUN dotnet publish -c Release -o out


FROM microsoft/dotnet:2.2-aspnetcore-runtime AS runtime
WORKDIR /app
COPY --from=build /app/GodotAgones.Server/out ./
# true if container is intended to be ran in cluster
# false if you want to use kubectl proxy
ENV IN_CLUSTER=true
ENTRYPOINT ["dotnet", "GodotAgones.Server.dll"]
