FROM node:lts-bookworm
WORKDIR /home/node/app
COPY ./package*json .
RUN npm ci --omit=dev
COPY . .
RUN npm run build

EXPOSE 3000

CMD ["npm", "run", "dev"]
