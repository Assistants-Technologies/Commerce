#!/bin/bash

for enc_file in Proxy/Nginx/certs/*.enc; do
  original="${enc_file%.enc}"
  echo "Decrypting $enc_file to $original..."
  sops -d "$enc_file" > "$original"
done