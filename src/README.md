# RDPCnC-CN 中文配置面板

## 这是什么

RDPWrap-CN 项目自研的**中文配置面板**（C# WinForms），功能对齐 RDP Wrapper 官方英文面板：
状态诊断、常规设置、身份验证模式、会话影子模式、mstsc 测试、在线更新配置
（官方源/国内镜像）、重启远程桌面服务。最终 exe 约 19KB，无外部依赖。

## 构建方法（三选一）

### Visual Studio（推荐）

用 VS 2019+ 打开 `RDPCnC.csproj` 直接生成（目标框架 .NET Framework 4.8）。

### MSBuild 命令行

```powershell
& "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe" RDPCnC.csproj /restore /p:Configuration=Release
```

### dotnet CLI

```powershell
dotnet build RDPCnC.csproj -c Release
```

输出：`src\bin\Release\net48\RDPCnC-CN.exe`

## 使用

- 将 `RDPCnC-CN.exe` 复制到 `C:\Program Files\RDP Wrapper\` 即可
- 双击运行（自动请求管理员权限，UAC 点“是”）
- 可与官方英文版 RDP_CnC.exe 共存，功能一致

## 说明

- 本工具读写远程桌面相关注册表项，杀毒软件可能提示，属正常行为
- 仅供个人学习使用，商业环境请购买正版授权
