# Etapa 1: build do Angular
FROM node:18 AS build
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build --prod

# Etapa 2: servir via NGINX
FROM nginx:stable-alpine
COPY --from=build /app/dist/sisand-airlines-frontend /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
