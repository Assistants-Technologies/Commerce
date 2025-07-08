$Root = (Resolve-Path "$PSScriptRoot\..").Path
$CertPath = Join-Path $Root "Platform\Nginx\certs"
$Files = Get-ChildItem $CertPath -Include *.pem, *.pfx -File

foreach ($File in $Files) {
    $EncryptedPath = "$($File.FullName).enc"
    Write-Host "Encrypting $($File.Name)..."
    sops -e $File.FullName | Out-File -Encoding ascii -FilePath $EncryptedPath
}