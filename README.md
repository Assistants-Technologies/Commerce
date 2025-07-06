```
git remote add identityprovider https://github.com/Assistants-Technologies/IdentityProvider.git
git config alias.pull-idp "subtree pull --prefix Modules/IdentityProvider identityprovider main --squash"
git config alias.push-idp "subtree push --prefix Modules/IdentityProvider identityprovider main"

git remote add accountpanel https://github.com/Assistants-Technologies/AccountPanel.git
git config alias.pull-accp "subtree pull --prefix Modules/AccountPanel accountpanel main --squash"
git config alias.push-accp "subtree push --prefix Modules/AccountPanel accountpanel main"
```