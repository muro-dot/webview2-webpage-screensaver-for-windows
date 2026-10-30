param([switch]$SkipInstall)
$ErrorActionPreference = 'Stop'
Set-Location -LiteralPath $PSScriptRoot
$python = Join-Path $PSScriptRoot '.venv\Scripts\python.exe'
if (-not (Test-Path -LiteralPath $python)) {
    python -m venv .venv
    if ($LASTEXITCODE -ne 0) { throw 'Could not create virtual environment.' }
}
if (-not $SkipInstall) {
    & $python -m pip install -r requirements.txt
    if ($LASTEXITCODE -ne 0) { throw 'Dependency installation failed.' }
}
& $python -m unittest discover -s tests -v
if ($LASTEXITCODE -ne 0) { throw 'Tests failed.' }
& $python main.py --capture-assets assets
if ($LASTEXITCODE -ne 0) { throw 'Screenshot layout verification failed.' }
& $python -m PyInstaller --noconfirm --clean WebScreensaver.spec
if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }
Copy-Item -LiteralPath dist\WebScreensaver.exe -Destination dist\WebScreensaver.scr -Force
$size = (Get-Item -LiteralPath dist\WebScreensaver.scr).Length
Write-Host ('Built WebScreensaver.scr: {0:N2} MiB' -f ($size / 1MB))
