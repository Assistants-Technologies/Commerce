$Root = (Resolve-Path "$PSScriptRoot\..").Path
$CertPath = Join-Path $Root "Platform\Nginx\certs"
$EncFiles = Get-ChildItem $CertPath -Include *.enc -File

foreach ($EncFile in $EncFiles) {
    $OriginalPath = $EncFile.FullName -replace '\.enc$', ''
    Write-Host "Decrypting $($EncFile.Name)..."
    sops -d $EncFile.FullName | Out-File -Encoding ascii -FilePath $OriginalPath
}