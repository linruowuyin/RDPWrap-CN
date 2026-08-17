@echo off
chcp 65001 >nul
title RDP Wrapper 卸载程序（汉化引导）

echo ============================================
echo   RDP Wrapper - 卸载程序
echo ============================================
echo.

net session >nul 2>&1
if %errorlevel% neq 0 (
  echo   [错误] 请右键本脚本 → 以管理员身份运行！
  pause
  exit /b 1
)

if not exist "%~dp0RDPW_Uninstaller.exe" goto :error

echo [1/2] 正在运行官方卸载器...
"%~dp0RDPW_Uninstaller.exe"

echo [2/2] 正在清理残留...
SCHTASKS /DELETE /TN "RDPWUpdater" /F >nul 2>&1
rmdir /Q /S "C:\Program Files\RDP Wrapper" 2>nul

echo.
echo ============================================
echo   卸载完成！远程桌面服务已恢复系统默认状态。
echo ============================================
pause
exit /b 0

:error
echo ============================================
echo   错误: 未找到卸载程序 RDPW_Uninstaller.exe
echo   请确保本目录包含完整安装包文件。
echo ============================================
pause
exit /b 1