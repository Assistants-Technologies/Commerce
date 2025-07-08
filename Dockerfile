# Wydajny obraz bazowy Node.js
FROM node:20-alpine

# Włącz zależności dla buildowania TS i native addons
RUN apk add --no-cache python3 make g++ libc6-compat

# Ustaw katalog roboczy
WORKDIR /app

# Skopiuj package.json i zainstaluj zależności
COPY package*.json ./
RUN npm install

# Skopiuj cały projekt
COPY . .

# Expose 4445
EXPOSE 4445

# Build TypeScript
RUN npm run build

# Domyślna komenda
CMD ["npm", "run", "start"]