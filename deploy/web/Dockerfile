# NSBM ICU web app — build from the repository root:
#   docker build -f deploy/web/Dockerfile -t nsbm-icu-web .
# Set --build-arg VITE_DEMO_MODE=true only for a public demo deployment.

FROM node:22-alpine AS build
WORKDIR /web

COPY web/package.json web/package-lock.json ./
COPY web/scripts ./scripts
# The postinstall script copies the on-device camera-coaching runtime into public/.
RUN npm ci --no-audit --no-fund

COPY web/ ./
ARG VITE_DEMO_MODE=false
ARG VITE_FACE_MODEL_URL=
ENV VITE_DEMO_MODE=$VITE_DEMO_MODE VITE_FACE_MODEL_URL=$VITE_FACE_MODEL_URL
RUN npm run build

FROM nginx:1.27-alpine AS runtime
COPY deploy/web/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /web/dist /usr/share/nginx/html
EXPOSE 80
