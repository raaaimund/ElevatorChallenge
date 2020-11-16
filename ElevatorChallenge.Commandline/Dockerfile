FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["ElevatorChallenge.Commandline/ElevatorChallenge.Commandline.csproj", "ElevatorChallenge.Commandline/"]
COPY ["ElevatorChallenge.Application/ElevatorChallenge.Application.csproj", "ElevatorChallenge.Application/"]
COPY ["ElevatorChallenge.Domain/ElevatorChallenge.Domain.csproj", "ElevatorChallenge.Domain/"]
COPY ["ElevatorChallenge.Infrastructure/ElevatorChallenge.Infrastructure.csproj", "ElevatorChallenge.Infrastructure/"]
RUN dotnet restore "ElevatorChallenge.Commandline/ElevatorChallenge.Commandline.csproj"
COPY . .
WORKDIR "/src/ElevatorChallenge.Commandline"
RUN dotnet build "ElevatorChallenge.Commandline.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ElevatorChallenge.Commandline.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ElevatorChallenge.Commandline.dll"]