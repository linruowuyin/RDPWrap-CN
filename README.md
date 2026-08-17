# RDPWrap-CN

> Windows 家庭版/标准版开启远程桌面 + 多用户同时登录的中文套件
>
> 一键安装 · 中文脚本 · 中文配置面板 · 配置每日自动同步 · 云端自动编译

## ✨ 特性

- ✅ 完整安装包开箱即用（所有二进制已收录，无需自行下载）
- ✅ 中文安装 / 卸载 / 更新脚本（含管理员权限检测、更新源国内镜像切换）
- ✅ **RDPCnC-CN.exe 中文配置面板**（自研，C# 编译，功能对齐官方英文面板：状态诊断 + 全部设置项 + 更新配置 + 测试连接）
- ✅ 中文使用说明（Markdown + HTML 双版本）
- ✅ 自动同步：每天定时检查上游 rdpwrap.ini，有更新自动提交并发布 release
- ✅ 自动编译：源码 `src/` 更新后，GitHub 云端自动编译出 exe 并提交回仓库

## 📦 快速安装

1. 从 [Releases](../../releases) 下载最新 zip 包并解压（或直接下载仓库文件）
2. 关闭杀毒软件（Windows Defender 等对注入类工具存在误报，安装后添加排除即可）
3. 右键 `install_cn.bat` → **以管理员身份运行**
4. 把 `C:\Program Files\RDP Wrapper` 加入杀毒软件排除列表：
   ```powershell
   Add-MpPreference -ExclusionPath "C:\Program Files\RDP Wrapper\"
   ```
5. 完成。详细说明见 `docs/使用说明.md`

## 📁 目录结构

```
RDPWrap-CN/
├── bin/
│   ├── RDPW_Installer.exe      # 安装器
│   ├── RDPW_Uninstaller.exe    # 卸载器
│   ├── RDPCnC-CN.exe           # 中文配置面板（自研）
│   ├── install_cn.bat          # 中文安装脚本
│   ├── uninstall_cn.bat        # 中文卸载脚本
│   └── update_cn.bat           # 中文更新配置脚本
├── src/                        # 中文配置面板源码（C#）
├── rdpwrap.ini                 # 补丁配置（自动同步）
├── docs/                       # 中文文档
├── scripts/build-package.ps1   # 发布包打包脚本
└── .github/workflows/          # 自动同步 + 自动编译工作流
```

## 🔄 自动化机制

| 工作流 | 触发条件 | 动作 |
|--------|---------|------|
| 同步配置 | 每天北京时间 00:30 / 手动 | 检查上游 rdpwrap.ini → 有更新则提交、编译面板、打包发布 release |
| 编译面板 | 推送 `src/` 源码 / 手动 | 云端编译 RDPCnC-CN.exe → 有变化自动提交回仓库 |

## ⚠️ 免责声明

- 本工具违反微软 Windows EULA（多会话为 Server 付费功能），仅供个人学习、家庭内部使用，请勿用于商业环境
- 杀毒软件（含 Windows Defender）会持续误报，属正常现象
- 商业场景请购买正版 Windows 专业版 / Server 授权
- 使用风险自负

## 📄 许可证

本项目基于 RDP Wrapper Library（[Stas'M](https://github.com/stascorp/rdpwrap) 原创，[sebaxakerhtc](https://github.com/sebaxakerhtc/rdpwrap) 维护 mod 版），沿用 [Apache License 2.0](LICENSE)，见 [NOTICE](NOTICE)。中文面板、中文脚本与文档为本项目原创内容。
