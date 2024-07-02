FROM mcr.microsoft.com/dotnet/sdk:8.0
COPY . /home/backend
WORKDIR /home/backend/Api
RUN dotnet restore
RUN dotnet publish -c Release -o out

CMD [ "dotnet", "run" ]
