# ─── Build stage ─────────────────────────────────────────────────────────────
FROM ghcr.io/cirruslabs/flutter:stable AS builder

WORKDIR /app

# Cache de dependências separado do código-fonte
COPY pubspec.yaml pubspec.lock ./
RUN flutter pub get

COPY . .

# URL do backend — sobrescreva com --build-arg API_BASE_URL=http://... se precisar
ARG API_BASE_URL=http://localhost:5197
RUN flutter build web --release --dart-define=API_BASE_URL=$API_BASE_URL

# ─── Serve stage ─────────────────────────────────────────────────────────────
FROM nginx:alpine

COPY --from=builder /app/build/web /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
