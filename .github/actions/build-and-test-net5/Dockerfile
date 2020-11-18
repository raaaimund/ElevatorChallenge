FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
COPY entrypoint.sh .
RUN chmod +x entrypoint.sh
ENTRYPOINT ["/entrypoint.sh"]