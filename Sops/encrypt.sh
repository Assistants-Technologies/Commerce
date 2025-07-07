#!/bin/bash

for file in Proxy/Nginx/certs/*.pem Proxy/Nginx/certs/*.pfx; do
  [ -f "$file" ] || continue
  echo "Encrypting $file..."
  sops -e "$file" > "$file.enc"
done