// ============================================================
//  RDP Wrapper 中文配置面板 (RDPCnC-CN.exe)
//  C# WinForms 实现，功能对齐官方 RDP_CnC.exe，界面全中文
//  编译: csc /target:winexe /out:RDPCnC-CN.exe RDPCnC.cs
// ============================================================
using System;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.ServiceProcess;
using System.Windows.Forms;
using Microsoft.Win32;

namespace RDPWrapCN
{
    public class MainForm : Form
    {
        // 状态标签
        Label lsWrapper = new Label(), lsService = new Label(), lsListener = new Label(),
              lsTSVer = new Label(), lsSupp = new Label();
        // 设置控件
        CheckBox cbAllowTS = new CheckBox(), cbSingle = new CheckBox(), cbHideUsers = new CheckBox(), cbCustomPrg = new CheckBox();
        NumericUpDown numPort = new NumericUpDown();
        RadioButton[] rbNLA = new RadioButton[3];
        RadioButton[] rbShadow = new RadioButton[5];
        Label lblPath = new Label();
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        bool ready = false;

        public MainForm()
        {
            Text = "RDP Wrapper 配置与检测工具（中文版）";
            Font = new Font("Microsoft YaHei UI", 9F);
            ClientSize = new Size(660, 560);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;

            BuildUi();
            timer.Interval = 2000;
            timer.Tick += (s, e) => RefreshStatus();
            timer.Start();
            Load += (s, e) => { ReadSettings(); ready = true; RefreshStatus(); };
        }

        void BuildUi()
        {
            // ---------- 诊断信息 ----------
            GroupBox gbDiag = new GroupBox { Text = "诊断信息", Left = 12, Top = 8, Width = 300, Height = 190 };
            Controls.Add(gbDiag);
            AddRow(gbDiag, "Wrapper 状态：", ref lsWrapper, 25);
            AddRow(gbDiag, "服务状态：", ref lsService, 55);
            AddRow(gbDiag, "监听器状态：", ref lsListener, 85);
            AddRow(gbDiag, "termsrv 版本：", ref lsTSVer, 115);
            Label lSupp = new Label { Text = "支持级别：", Left = 12, Top = 145, AutoSize = true };
            lsSupp = new Label { Left = 95, Top = 145, AutoSize = true, Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold) };
            gbDiag.Controls.Add(lSupp); gbDiag.Controls.Add(lsSupp);
            lblPath = new Label { Left = 12, Top = 166, Width = 280, ForeColor = Color.Gray, AutoEllipsis = true };
            gbDiag.Controls.Add(lblPath);

            // ---------- 常规设置 ----------
            GroupBox gbGeneral = new GroupBox { Text = "常规设置", Left = 330, Top = 8, Width = 318, Height = 190 };
            Controls.Add(gbGeneral);
            Label lPort = new Label { Text = "RDP 端口：", Left = 15, Top = 28, AutoSize = true };
            numPort = new NumericUpDown { Left = 100, Top = 24, Width = 90, Minimum = 0, Maximum = 65535 };
            cbAllowTS = new CheckBox { Text = "启用远程桌面", Left = 15, Top = 58, Width = 200, AutoSize = true };
            cbSingle = new CheckBox { Text = "每用户限制单会话", Left = 15, Top = 88, Width = 220, AutoSize = true };
            cbHideUsers = new CheckBox { Text = "登录界面隐藏用户列表", Left = 15, Top = 118, Width = 220, AutoSize = true };
            cbCustomPrg = new CheckBox { Text = "允许未注册的 RemoteApps", Left = 15, Top = 148, Width = 240, AutoSize = true };
            gbGeneral.Controls.AddRange(new Control[] { lPort, numPort, cbAllowTS, cbSingle, cbHideUsers, cbCustomPrg });
            foreach (Control c in new Control[] { numPort, cbAllowTS, cbSingle, cbHideUsers, cbCustomPrg })
                c.Tag = 1;
            numPort.ValueChanged += MarkDirty; cbAllowTS.CheckedChanged += MarkDirty; cbSingle.CheckedChanged += MarkDirty;
            cbHideUsers.CheckedChanged += MarkDirty; cbCustomPrg.CheckedChanged += MarkDirty;

            // ---------- 身份验证模式 ----------
            GroupBox gbNLA = new GroupBox { Text = "身份验证模式", Left = 330, Top = 210, Width = 318, Height = 112 };
            Controls.Add(gbNLA);
            string[] nlaNames = { "仅图形界面验证", "默认 RDP 验证", "网络级身份验证 (NLA，推荐)" };
            for (int i = 0; i < 3; i++)
            {
                rbNLA[i] = new RadioButton { Text = nlaNames[i], Left = 15, Top = 26 + i * 27, Width = 280, AutoSize = true };
                gbNLA.Controls.Add(rbNLA[i]);
                rbNLA[i].CheckedChanged += MarkDirty;
            }

            // ---------- 会话影子模式 ----------
            GroupBox gbShadow = new GroupBox { Text = "会话影子模式", Left = 12, Top = 210, Width = 300, Height = 168 };
            Controls.Add(gbShadow);
            string[] shadowNames = { "禁用影子功能", "经用户同意完全控制", "无需同意完全控制", "经用户同意仅查看", "无需同意仅查看" };
            for (int i = 0; i < 5; i++)
            {
                rbShadow[i] = new RadioButton { Text = shadowNames[i], Left = 15, Top = 24 + i * 26, Width = 260, AutoSize = true };
                gbShadow.Controls.Add(rbShadow[i]);
                rbShadow[i].CheckedChanged += MarkDirty;
            }

            // ---------- mstsc 测试 ----------
            GroupBox gbTest = new GroupBox { Text = "用 mstsc 测试本机连接", Left = 12, Top = 392, Width = 636, Height = 60 };
            Controls.Add(gbTest);
            string[] resNames = { "全屏", "800x600", "1024x768", "1366x768", "1920x1080" };
            string[] resArgs = { "/v:127.0.0.2 /f /prompt", "/v:127.0.0.2 /w:800 /h:600 /prompt", "/v:127.0.0.2 /w:1024 /h:768 /prompt", "/v:127.0.0.2 /w:1366 /h:768 /prompt", "/v:127.0.0.2 /w:1920 /h:1080 /prompt" };
            for (int i = 0; i < 5; i++)
            {
                Button b = new Button { Text = resNames[i], Left = 12 + i * 124, Top = 24, Width = 112, Height = 26 };
                string args = resArgs[i];
                b.Click += (s, e) => Process.Start("mstsc", args);
                gbTest.Controls.Add(b);
            }

            // ---------- 底部操作 ----------
            Button bApply = new Button { Text = "应用设置", Left = 12, Top = 466, Width = 110, Height = 34, Enabled = false, Name = "bApply" };
            bApply.Click += (s, e) => { WriteSettings(); bApply.Enabled = false; MessageBox.Show("设置已应用。部分设置需重启远程桌面服务后生效。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information); };
            Button bUpdate = new Button { Text = "更新配置(官方源)", Left = 130, Top = 466, Width = 120, Height = 34 };
            bUpdate.Click += (s, e) => UpdateIni(null);
            Button bUpdateMir = new Button { Text = "更新配置(国内镜像)", Left = 258, Top = 466, Width = 130, Height = 34 };
            bUpdateMir.Click += (s, e) => UpdateIni("https://gh.ddlc.top/https://raw.githubusercontent.com/sebaxakerhtc/rdpwrap.ini/master/rdpwrap.ini");
            Button bRestart = new Button { Text = "重启远程桌面服务", Left = 396, Top = 466, Width = 120, Height = 34 };
            bRestart.Click += (s, e) => { RunElevated("net stop termservice /y & timeout /t 2 /nobreak >nul & net start termservice"); MessageBox.Show("已发起服务重启（UAC 弹窗请点“是”），约 5 秒后生效。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information); };
            Button bSettings = new Button { Text = "系统远程设置", Left = 524, Top = 466, Width = 124, Height = 34 };
            bSettings.Click += (s, e) => Process.Start("ms-settings:remotedesktop");
            Controls.AddRange(new Control[] { bApply, bUpdate, bUpdateMir, bRestart, bSettings });

            Button bLicense = new Button { Text = "查看许可证", Left = 12, Top = 508, Width = 110, Height = 32 };
            bLicense.Click += (s, e) => {
                MessageBox.Show("RDP Wrapper Library\nApache License 2.0\n\n原作者: Stas'M (stascorp/rdpwrap)\nmod: sebaxakerhtc\n本中文面板: RDPWrap-CN\n\n本工具违反微软 EULA，仅供个人学习使用。", "许可证", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            Controls.Add(bLicense);

            // 自动应用按钮状态管理
            void MarkDirty(object s, EventArgs e) { if (ready) { var btn = (Button)Controls.Find("bApply", true)[0]; btn.Enabled = true; } }
        }

        void AddRow(GroupBox g, string name, ref Label value, int top)
        {
            Label l = new Label { Text = name, Left = 12, Top = top, AutoSize = true };
            value = new Label { Left = 120, Top = top, AutoSize = true, Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold) };
            g.Controls.Add(l); g.Controls.Add(value);
        }

        // ============ 状态采集 ============
        void RefreshStatus()
        {
            // Wrapper 状态
            string img = RegStr(@"SYSTEM\CurrentControlSet\Services\TermService", "ImagePath");
            string dll = RegStr(@"SYSTEM\CurrentControlSet\Services\TermService\Parameters", "ServiceDll");
            if (img == null) SetVal(lsWrapper, "未知", Color.Gray);
            else if (!img.ToLower().Contains("svchost.exe")) SetVal(lsWrapper, "第三方服务", Color.Red);
            else if (dll != null && dll.ToLower().Contains("rdpwrap.dll")) SetVal(lsWrapper, "已安装", Color.Green);
            else if (dll != null && dll.ToLower().Contains("termsrv.dll")) SetVal(lsWrapper, "未安装", Color.Gray);
            else SetVal(lsWrapper, "未知", Color.Gray);

            // 服务状态
            try
            {
                using (ServiceController sc = new ServiceController("TermService"))
                {
                    switch (sc.Status)
                    {
                        case ServiceControllerStatus.Running: SetVal(lsService, "运行中", Color.Green); break;
                        case ServiceControllerStatus.Stopped: SetVal(lsService, "已停止", Color.Red); break;
                        default: SetVal(lsService, sc.Status.ToString(), Color.DarkGoldenrod); break;
                    }
                }
            }
            catch { SetVal(lsService, "未知", Color.Gray); }

            // 监听器状态
            int port = (int)RegVal(@"SYSTEM\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp", "PortNumber", 3389);
            bool listening = false;
            try { foreach (var ep in IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners()) if (ep.Port == port) { listening = true; break; } } catch { }
            SetVal(lsListener, listening ? "监听中 (端口 " + port + ")" : "未监听 (端口 " + port + ")", listening ? Color.Green : Color.Red);

            // termsrv 版本 + 支持级别
            string ver = "不适用";
            try { ver = FileVersionInfo.GetVersionInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32", "termsrv.dll")).FileVersion ?? "不适用"; } catch { }
            SetVal(lsTSVer, ver, Color.Black);
            string iniPath = @"C:\Program Files\RDP Wrapper\rdpwrap.ini";
            string ini = null;
            if (File.Exists(iniPath)) try { ini = File.ReadAllText(iniPath); } catch { }
            if (ini != null && ver != "不适用" && ini.Contains("[" + ver + "]")) SetVal(lsSupp, "[完全支持]", Color.Green);
            else if (ver.StartsWith("6.0.") || ver.StartsWith("6.1.")) SetVal(lsSupp, "[部分支持]", Color.DarkGoldenrod);
            else if (ver != "不适用") SetVal(lsSupp, "[不支持，请更新配置]", Color.Red);
            else SetVal(lsSupp, "", Color.Gray);

            lblPath.Text = Directory.Exists(@"C:\Program Files\RDP Wrapper") ? "安装目录: C:\\Program Files\\RDP Wrapper（已安装）" : "安装目录: C:\\Program Files\\RDP Wrapper（未找到）";
        }

        static void SetVal(Label l, string text, Color c) { if (l.Text != text) l.Text = text; l.ForeColor = c; }

        static string RegStr(string path, string name)
        {
            try { return Registry.LocalMachine.OpenSubKey(path)?.GetValue(name) as string; } catch { return null; }
        }
        static object RegVal(string path, string name, object def)
        {
            try { return Registry.LocalMachine.OpenSubKey(path)?.GetValue(name) ?? def; } catch { return def; }
        }

        // ============ 读写设置 ============
        void ReadSettings()
        {
            using (RegistryKey k = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Terminal Server"))
            {
                cbAllowTS.Checked = (int)(k.GetValue("fDenyTSConnections", 1) ?? 1) == 0;
                cbSingle.Checked = (int)(k.GetValue("fSingleSessionPerUser", 1) ?? 1) == 1;
                cbCustomPrg.Checked = (int)(k.GetValue("HonorLegacySettings", 0) ?? 0) == 1;
            }
            numPort.Value = (int)RegVal(@"SYSTEM\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp", "PortNumber", 3389);
            int sl = (int)RegVal(@"SYSTEM\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp", "SecurityLayer", 0);
            int ua = (int)RegVal(@"SYSTEM\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp", "UserAuthentication", 0);
            rbNLA[(sl == 0 && ua == 0) ? 0 : (sl == 1 && ua == 0 ? 1 : 2)].Checked = true;
            int shadow = (int)RegVal(@"SYSTEM\CurrentControlSet\Control\Terminal Server", "Shadow", 0);
            if (shadow >= 0 && shadow < 5) rbShadow[shadow].Checked = true;
            cbHideUsers.Checked = (int)RegVal(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "dontdisplaylastusername", 0) == 1;
        }

        void WriteSettings()
        {
            try
            {
                using (RegistryKey k = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Terminal Server"))
                {
                    k.SetValue("fDenyTSConnections", cbAllowTS.Checked ? 0 : 1, RegistryValueKind.DWord);
                    k.SetValue("fSingleSessionPerUser", cbSingle.Checked ? 1 : 0, RegistryValueKind.DWord);
                    k.SetValue("HonorLegacySettings", cbCustomPrg.Checked ? 1 : 0, RegistryValueKind.DWord);
                    int shadow = -1;
                    for (int i = 0; i < 5; i++) if (rbShadow[i].Checked) shadow = i;
                    if (shadow >= 0) k.SetValue("Shadow", shadow, RegistryValueKind.DWord);
                }
                using (RegistryKey k = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp"))
                {
                    k.SetValue("PortNumber", (int)numPort.Value, RegistryValueKind.DWord);
                    int sl = 0, ua = 0;
                    if (rbNLA[0].Checked) { sl = 0; ua = 0; }
                    if (rbNLA[1].Checked) { sl = 1; ua = 0; }
                    if (rbNLA[2].Checked) { sl = 2; ua = 1; }
                    k.SetValue("SecurityLayer", sl, RegistryValueKind.DWord);
                    k.SetValue("UserAuthentication", ua, RegistryValueKind.DWord);
                }
                int sh = -1;
                for (int i = 0; i < 5; i++) if (rbShadow[i].Checked) sh = i;
                if (sh >= 0)
                    using (RegistryKey k = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows NT\Terminal Services"))
                        k.SetValue("Shadow", sh, RegistryValueKind.DWord);
                using (RegistryKey k = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System"))
                    k.SetValue("dontdisplaylastusername", cbHideUsers.Checked ? 1 : 0, RegistryValueKind.DWord);
                // 端口变化时更新防火墙规则
                try
                {
                    Process.Start(new ProcessStartInfo("netsh", "advfirewall firewall set rule name=\"Remote Desktop\" new localport=" + numPort.Value) { WindowStyle = ProcessWindowStyle.Hidden, UseShellExecute = false, Verb = "runas" });
                }
                catch { }
            }
            catch (Exception ex)
            {
                MessageBox.Show("写入设置失败：" + ex.Message + "\n\n请以管理员身份运行本工具。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============ 操作 ============
        void UpdateIni(string mirrorUrl)
        {
            string exe = @"C:\Program Files\RDP Wrapper\RDPWInst.exe";
            if (!File.Exists(exe)) { MessageBox.Show("未找到 RDPWInst.exe，请先安装 RDP Wrapper。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo(exe) { UseShellExecute = true, Verb = "runas", WindowStyle = ProcessWindowStyle.Hidden };
                if (!string.IsNullOrEmpty(mirrorUrl)) psi.Arguments = "-w \"" + mirrorUrl + "\"";
                else psi.Arguments = "-w";
                Process p = Process.Start(psi);
                p.WaitForExit();
                MessageBox.Show("配置更新完成。建议点击「重启远程桌面服务」使新配置生效。", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshStatus();
            }
            catch (Exception ex) { MessageBox.Show("更新失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        static void RunElevated(string cmd)
        {
            try
            {
                Process.Start(new ProcessStartInfo("cmd.exe", "/C " + cmd) { Verb = "runas", WindowStyle = ProcessWindowStyle.Hidden, UseShellExecute = true });
            }
            catch { }
        }

        // ============ 入口 ============
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            bool isAdmin = false;
            try
            {
                using (System.Security.Principal.WindowsIdentity id = System.Security.Principal.WindowsIdentity.GetCurrent())
                    isAdmin = new System.Security.Principal.WindowsPrincipal(id).IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
            }
            catch { }
            if (!isAdmin)
            {
                try
                {
                    Process.Start(new ProcessStartInfo(Application.ExecutablePath) { Verb = "runas", UseShellExecute = true });
                    return;
                }
                catch { MessageBox.Show("需要管理员权限才能读写远程桌面设置。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            }
            Application.Run(new MainForm());
        }
    }
}
