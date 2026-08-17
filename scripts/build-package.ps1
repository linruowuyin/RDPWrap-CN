# RDPWrap-CN 发布包打包脚本
# 用法: .\scripts\build-package.ps1 [-Version "2026-08-17"]
# 输出: dist\RDPWrap-CN-<Version>.zip
param(
    [string]$Version = (Get-Date -Format 'yyyy-MM-dd')
)

$ErrorActionPreference = 'Stop'
$root = Resolve-Path (Join-Path $PSScriptRoot '..')
$dist = Join-Path $root 'dist'
$pkgName = "RDPWrap-CN-$Version"
$pkgDir = Join-Path $dist $pkgName

Write-Host "==> 打包目录: $pkgDir"

if (Test-Path $dist) { Remove-Item $dist -Recurse -Force }
New-Item -ItemType Directory -Path $pkgDir -Force | Out-Null

# 1. 核心文件
Copy-Item (Join-Path $root 'rdpwrap.ini') $pkgDir
Copy-Item (Join-Path $root 'VERSION') $pkgDir
Copy-Item (Join-Path $root 'LICENSE') $pkgDir
Copy-Item (Join-Path $root 'NOTICE') $pkgDir

# 2. 二进制与脚本（bin 目录内容平铺到包根目录，与脚本内相对路径约定一致）
Copy-Item (Join-Path $root 'bin\*') $pkgDir -Force

# 3. 文档
Copy-Item (Join-Path $root 'docs') (Join-Path $pkgDir 'docs') -Recurse

# 4. 校验清单
$files = Get-ChildItem $pkgDir -Recurse -File | Sort-Object FullName
$manifest = $files | ForEach-Object {
    $rel = $_.FullName.Substring($pkgDir.Length + 1)
    $hash = (Get-FileHash $_.FullName -Algorithm SHA256).Hash.ToLower()
    "{0}  {1}" -f $hash, $rel
}
$manifest | Set-Content (Join-Path $pkgDir 'SHA256SUMS.txt') -Encoding UTF8
Write-Host "==> 已生成 SHA256SUMS.txt ($($manifest.Count) 个文件)"

# 5. 压缩
$zipPath = Join-Path $dist "$pkgName.zip"
Compress-Archive -Path $pkgDir -DestinationPath $zipPath -Force
$zipSize = [math]::Round((Get-Item $zipPath).Length / 1MB, 2)
Write-Host "==> 完成: $zipPath ($zipSize MB)"

# 输出路径供 CI 使用
Write-Output "ZIP_PATH=$zipPath"