#!/bin/bash

cd "$(dirname "$0")"

for file in ../Platform/Nginx/certs/*.{pem,pfx}; do
  [ -f "$file" ] || continue
  echo "Encrypting $file..."
  sops -e "$file" > "$file.enc"
done