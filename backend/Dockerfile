FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder

WORKDIR /home/backend/
COPY . .
WORKDIR /home/backend/Api
#RUN dotnet restore
#RUN dotnet publish -c Release -o out
EXPOSE 5001

CMD [ "dotnet", "run" ]
#FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
#WORKDIR /backend
#COPY --from=builder /home/backend/ .

#WORKDIR /backend/Api
#RUN ls ./out/*/ && ls ./out #&& sleep 5000

#CMD [ "dotnet", "./out/ZstdSharp.dll" ]
