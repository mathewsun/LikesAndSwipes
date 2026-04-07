#!/usr/bin/env bash
set -euo pipefail

if [[ -z "${DOMAIN:-}" || -z "${CERTBOT_EMAIL:-}" ]]; then
  echo "DOMAIN and CERTBOT_EMAIL must be set."
  echo "Example: DOMAIN=likesandswipes.site CERTBOT_EMAIL=admin@likesandswipes.site ./scripts/init-letsencrypt.sh"
  exit 1
fi

docker compose up -d nginx

docker compose run --rm certbot certonly \
  --webroot -w /var/www/certbot \
  --email "$CERTBOT_EMAIL" \
  -d "$DOMAIN" \
  --rsa-key-size 4096 \
  --agree-tos \
  --no-eff-email

docker compose restart nginx

echo "Initial certificate created for $DOMAIN"
