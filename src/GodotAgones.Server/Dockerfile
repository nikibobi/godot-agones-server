FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["GodotAgones.Server/GodotAgones.Server.csproj", "GodotAgones.Server/"]
RUN dotnet restore "GodotAgones.Server/GodotAgones.Server.csproj"
COPY . .
WORKDIR "/src/GodotAgones.Server"
RUN dotnet build "GodotAgones.Server.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "GodotAgones.Server.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
# true if container is intended to be ran in cluster
# false if you want to use kubectl proxy
ENV IN_CLUSTER=true
ENTRYPOINT ["dotnet", "GodotAgones.Server.dll"]