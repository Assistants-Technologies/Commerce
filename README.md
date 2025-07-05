```
git remote add identityprovider https://github.com/TwojaOrg/IdentityProvider.git
git config alias.pull-idp "subtree pull --prefix Modules/IdentityProvider identityprovider main --squash"
git config alias.push-idp "subtree push --prefix Modules/IdentityProvider identityprovider main"
```