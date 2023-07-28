# syntax=docker/dockerfile:1.5

FROM node:19.4-bullseye AS build

WORKDIR /usr/src/app

COPY package*.json ./
COPY pnpm-lock.yaml ./

RUN npm install -g pnpm

RUN --mount=type=cache,target=/usr/src/app/.pnpm \
    pnpm set cache /usr/src/app/.pnpm && \
    pnpm install

COPY . .

RUN pnpm run build

FROM nginxinc/nginx-unprivileged:1.23-alpine-perl

COPY --link nginx.conf /etc/nginx/conf.d/default.conf

COPY --link --from=build usr/src/app/dist/ /usr/share/nginx/html

EXPOSE 8080