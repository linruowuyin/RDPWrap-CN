@echo off
chcp 65001 >nul
title RDP Wrapper 安装程序（汉化引导）

echo ============================================
echo   RDP Wrapper 多用户远程桌面 - 安装程序
echo   版本: v1.8.9.9 (mod by sebaxakerhtc)
echo   配置: rdpwrap.ini 2026-08-15
echo ============================================
echo.
echo   [提示] 请确保已关闭杀毒软件，安装后再添加排除项。
echo.
echo   [提示] 本脚本需要管理员权限。
echo.

net session >nul 2>&1
if %errorlevel% neq 0 (
  echo   [错误] 请右键本脚本 → 以管理员身份运行！
  pause
  exit /b 1
)

if not exist "%~dp0RDPW_Installer.exe" goto :error

echo [1/3] 正在运行官方安装器...
"%~dp0RDPW_Installer.exe"
if %errorlevel% neq 0 (
  echo   [警告] 安装器返回异常，继续尝试后续步骤...
)

ping -n 3 localhost >nul

echo [2/3] 正在写入最新 rdpwrap.ini 配置 (2026-08-15)...
if exist "C:\Program Files\RDP Wrapper\" (
  copy /Y "%~dp0rdpwrap.ini" "C:\Program Files\RDP Wrapper\rdpwrap.ini" >nul
  echo        已更新配置文件。
) else (
  echo        [警告] 未找到安装目录，可能安装未成功。
)

echo [3/3] 完成收尾...
echo.
echo   正在将中文配置面板复制到安装目录...
if exist "C:\Program Files\RDP Wrapper\" (
  copy /Y "%~dp0RDPCnC-CN.exe" "C:\Program Files\RDP Wrapper\RDPCnC-CN.exe" >nul 2>&1
  echo       已添加中文配置面板。
)
echo.
echo ============================================
echo   安装完成！
echo.
echo   请将以下目录加入杀毒软件排除列表:
echo   C:\Program Files\RDP Wrapper
echo.
echo   正在打开中文配置面板...
echo ============================================
if exist "C:\Program Files\RDP Wrapper\RDPCnC-CN.exe" (
  cmd.exe /C start "" "C:\Program Files\RDP Wrapper\RDPCnC-CN.exe"
) else (
  echo   未找到中文面板，请检查安装是否成功。
)
pause
exit /b 0

:error
echo ============================================
echo   错误: 未找到安装程序 RDPW_Installer.exe
echo   请确保本目录包含完整安装包文件。
echo ============================================
pause
exit /b 1