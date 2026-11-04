#!/usr/bin/env bash
# Instala el binario standalone de Tailwind CSS v4 para Linux x64.
# Uso: ./tools/install-tailwind.sh
set -euo pipefail

cd "$(dirname "$0")/.."
mkdir -p tools

curl -sL --max-time 180 \
  -o tools/tailwindcss \
  "https://github.com/tailwindlabs/tailwindcss/releases/latest/download/tailwindcss-linux-x64"

chmod +x tools/tailwindcss
echo "Tailwind instalado en tools/tailwindcss"
