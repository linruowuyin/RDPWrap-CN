# RDPWrap-CN

> Chinese kit for enabling RDP Host + concurrent RDP sessions on reduced-functionality Windows editions.

## Features

- Complete out-of-the-box package (all binaries included)
- Chinese installer / uninstaller / updater scripts (with China mirror update source)
- **RDPCnC-CN.exe** — self-developed Chinese configuration panel (C#, mirrors the official panel's full feature set)
- Chinese documentation (Markdown + HTML)
- Daily auto-sync of `rdpwrap.ini` via GitHub Actions
- Cloud auto-build of the panel exe on `src/` changes

## Quick Start

1. Download the latest zip from [Releases](../../releases) (or the repo files) and extract
2. Temporarily disable your antivirus (false positives are expected)
3. Right-click `install_cn.bat` → Run as administrator
4. Add an antivirus exclusion:
   ```powershell
   Add-MpPreference -ExclusionPath "C:\Program Files\RDP Wrapper\"
   ```

## Automation

| Workflow | Trigger | Action |
|----------|---------|--------|
| sync-ini | daily 00:30 (UTC+8) / manual | check upstream rdpwrap.ini → commit, build panel, package, release |
| build-cn-panel | push to `src/**` / manual | cloud-build RDPCnC-CN.exe → auto-commit if changed |

## Disclaimer

This tool violates Microsoft's Windows EULA. For personal/educational use only. Use at your own risk.

## License

Based on RDP Wrapper Library ([Stas'M](https://github.com/stascorp/rdpwrap), mod by [sebaxakerhtc](https://github.com/sebaxakerhtc/rdpwrap)), [Apache License 2.0](LICENSE). The Chinese panel, scripts and docs are original work of this project.
