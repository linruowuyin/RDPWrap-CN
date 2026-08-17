@echo off
chcp 65001 >nul
title RDP Wrapper 配置更新（汉化引导）

echo ============================================
echo   RDP Wrapper - 在线更新 rdpwrap.ini 配置
echo   (下载最新补丁配置以支持新版 Windows)
echo ============================================
echo.

net session >nul 2>&1
if %errorlevel% neq 0 (
  echo   [错误] 请右键本脚本 → 以管理员身份运行！
  pause
  exit /b 1
)

if not exist "C:\Program Files\RDP Wrapper\RDPWInst.exe" goto :error

echo   请选择更新源:
echo   [1] 官方源 (raw.githubusercontent.com)
echo   [2] 国内镜像 (gh.ddlc.top，推荐)
echo.

choice /C 12 /N /M "请输入 1 或 2: "
if errorlevel 2 goto mirror
if errorlevel 1 goto official

:official
echo.
echo   正在从官方源下载最新配置...
"C:\Program Files\RDP Wrapper\RDPWInst.exe" -w
goto done

:mirror
echo.
echo   正在从国内镜像下载最新配置...
"C:\Program Files\RDP Wrapper\RDPWInst.exe" -w https://gh.ddlc.top/https://raw.githubusercontent.com/sebaxakerhtc/rdpwrap.ini/master/rdpwrap.ini
goto done

:done
ping -n 3 localhost >nul
echo.
echo   更新完成，正在打开中文配置面板...
if exist "C:\Program Files\RDP Wrapper\RDPCnC-CN.exe" (
  cmd.exe /C start "" "C:\Program Files\RDP Wrapper\RDPCnC-CN.exe"
)
goto :end

:error
echo ============================================
echo   错误: 未找到已安装的 RDPWInst.exe
echo   请先运行 install_cn.bat 完成安装。
echo ============================================

:end
pause