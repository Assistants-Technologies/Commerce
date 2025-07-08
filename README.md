## 🐳 Docker on Windows

To make Portainer and other containers work properly on Windows, you may need to enable **WSL 2 integration** in Docker Desktop.

### ⚙️ How to enable WSL 2:

1. Open Docker Desktop.
2. Go to **Settings → Resources → WSL Integration**.
3. Enable integration for your installed WSL 2 distributions.

> ⚠️ Required if you're using Docker with WSL (e.g., Ubuntu on Windows).

## 🌳 Git subtree setup

To manage modules like IdentityProvider and AccountPanel as separate repositories via Git subtrees, use the following:

```bash
git remote add identityprovider https://github.com/Assistants-Technologies/IdentityProvider.git
git config alias.pull-idp "subtree pull --prefix Modules/IdentityProvider identityprovider main --squash"
git config alias.push-idp "subtree push --prefix Modules/IdentityProvider identityprovider main"

git remote add accountpanel https://github.com/Assistants-Technologies/AccountPanel.git
git config alias.pull-accp "subtree pull --prefix Modules/AccountPanel accountpanel main --squash"
git config alias.push-accp "subtree push --prefix Modules/AccountPanel accountpanel main"
```

## 🌐 Custom DNS setup for local development

This lets you map local services (like Portainer, IdentityProvider, etc.) to pretty domain names like `identity.assts.tech`.

### 🍎 macOS & 🐧 Linux (Debian, Ubuntu, etc.)

```bash
sudo nano /etc/hosts
```

### 🪟 Windows

```powershell
notepad.exe C:\Windows\System32\drivers\etc\hosts
```

> Make sure to **run as administrator**.

### ➕ Add these lines:

```plaintext
127.0.0.1   portainer.assts.tech
127.0.0.1   assts.tech
127.0.0.1   identity.assts.tech
127.0.0.1   account.assts.tech
127.0.0.1   commerce.assts.tech
```

## 🔄 Flush DNS cache

### 🍎 macOS

```bash
sudo dscacheutil -flushcache; sudo killall -HUP mDNSResponder
```

### 🐧 Linux (Debian-based)

```bash
sudo systemd-resolve --flush-caches
```

### 🪟 Windows

```powershell
ipconfig /flushdns
```

## 🔐 Secrets encryption (SOPS + GPG)

This project uses [SOPS](https://github.com/mozilla/sops) with **GPG keys** to encrypt secrets (e.g. `.yaml`, `.pem`, `.pfx` files).

The config file `Sops/.sops.yaml` is already present.  
**To decrypt/encrypt, you must add your GPG key fingerprint to it.**

### 📦 Installation

#### macOS

```bash
brew install sops gnupg
```

#### Linux (Debian-based)

```bash
sudo apt install sops gnupg
```

#### Windows

Download SOPS from GitHub releases: https://github.com/mozilla/sops/releases  
Install [GPG4Win](https://www.gpg4win.org/) to manage your GPG keys.

### 🛠️ Generate a GPG key

```bash
gpg --full-generate-key
```

Steps:

1. Choose option `9) ECC (sign and encrypt)`
2. Confirm all defaults
3. Enter your name and email
4. Choose a strong passphrase

Then check your key:

```bash
gpg --list-secret-keys
```

Copy your fingerprint (e.g. `36EC0F83FA5E36F0E87A9E0E436F52BFF4F88902`)

### 🧩 Add your key to `Sops/.sops.yaml`

```yaml
creation_rules:
  - path_regex: '.*\.(ya?ml|pem|pfx)$'
    encrypted_regex: '^(data|stringData)$'
    pgp: >-
      36EC0F83FA5E36F0E87A9E0E436F52BFF4F88902#, ← ADD COMMA AFTER THE LAST FINGERPRINT
      # ← ADD YOUR FINGERPRINT HERE
```

> 🔁 Each developer should add their key here to decrypt/encrypt.

### 🔒 Encrypt a file

```bash
./Sops/encrypt.sh
```

alternatively, for PowerShell, you can use:

```bash
./Sops/encrypt.ps1
```

### 🔓 Decrypt a file

```bash
./Sops/decrypt.sh
```

alternatively, for PowerShell, you can use:

```bash
./Sops/decrypt.ps1
```

> ✅ The original `.pem` and `.pfx` files should be gitignored — only `.enc` files are tracked.