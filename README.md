# OmidBankSepah

## Configure the GitHub NuGet feed

Before restoring packages or running/building this solution, add the GitHub Packages NuGet source by running this command in a terminal:

```bash
dotnet nuget add source --username samiyariceboy --password YOUR_GITHUB_TOKEN --store-password-in-clear-text --name github https://nuget.pkg.github.com/samiyariceboy/index.json
```

Replace `YOUR_GITHUB_TOKEN` with a GitHub Personal Access Token that has permission to read packages (`read:packages`). The source is then available to NuGet as `github`.

> **Security:** `--store-password-in-clear-text` stores the credential unencrypted in the NuGet configuration file. Never commit a real token or share it in source control, logs, or screenshots. Use a token with only the permissions required.
