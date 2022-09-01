using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using winforms = System.Windows.Forms;
public class LayoutManager
{
    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

    private const int SW_HIDE = 0;  
    private const int SW_RESTORE = 9;

    private static System.Windows.Forms.NotifyIcon _notifyIcon = new System.Windows.Forms.NotifyIcon();

    private static int _width = 100, _height = 100;
    private IntPtr window;

    public static bool Visible
    {
        get
        {
            return _notifyIcon.Visible;
        }
    }

    public void Init()
    {
        window = GetForegroundWindow();
        _notifyIcon.BalloonTipText = "Hidden to taskbar tray";
        _notifyIcon.Text = "XMR-Online";
        _notifyIcon.Icon = CustomTrayIcon(Application.streamingAssetsPath + "/icon.png", _width, _height);
        _notifyIcon.ShowBalloonTip(2000);
        _notifyIcon.MouseClick += notifyIcon_MouseClick;

        MenuItem OpenMenuItem = new MenuItem("Open");
        OpenMenuItem.Click += OnOpen;
        MenuItem StartWorkingMenuItem = new MenuItem("start working (mining)");
        StartWorkingMenuItem.Click += OnStartWorking;
        MenuItem CloseGraphMenuItem = new MenuItem("Close but not stop mining");
        CloseGraphMenuItem.Click += OnCloseGraphButton;
        MenuItem exitMenuItem = new MenuItem("quit");
        exitMenuItem.Click += OnExit;
        MenuItem[] childen = new MenuItem[] { OpenMenuItem, StartWorkingMenuItem, CloseGraphMenuItem, exitMenuItem };
        _notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(childen);
    }

    private void OnStartWorking(object sender, EventArgs e) 
    {
        Main.Instance.GetUIComponent<UILogin>().StartWork();
    }

    private void OnCloseGraphButton(object sender, EventArgs e) 
    {
        UIMining uiMining = Main.Instance.GetUIComponent<UIMining>();
        if (uiMining.AtWork)
        {
            uiMining.OnCloseGraphButton();
            return;
        }
        System.Windows.Forms.MessageBox.Show("Miners haven't started work yet", "Report", winforms.MessageBoxButtons.OK, winforms.MessageBoxIcon.Error);
    }

    private void OnExit(object sender, EventArgs e) 
    {
        _notifyIcon.Visible = false;
        ShowWindow(window, SW_RESTORE);
        if (!Main.Instance.Quit(false))
        {
            Main.Instance.GetUIComponent<UIMining>().Conceal();
            System.Diagnostics.Process cur = System.Diagnostics.Process.GetCurrentProcess();
            cur.Kill();
        }
    }

    private void OnOpen(object sender, EventArgs e)
    { 
        _notifyIcon.Visible = false;
        ShowWindow(window, SW_RESTORE);
    }

    public void HideTaskBar()
    {
        try
        {
            _notifyIcon.Visible = true;
            ShowWindow(window, SW_HIDE);
        }
        catch (Exception e)
        {
        }
    }

    private static System.Drawing.Icon CustomTrayIcon(string iconPath, int width, int height)
    {
        System.Drawing.Bitmap bt = new System.Drawing.Bitmap(iconPath);
        System.Drawing.Bitmap fitSizeBt = new System.Drawing.Bitmap(bt, width, height);
        return System.Drawing.Icon.FromHandle(fitSizeBt.GetHicon());
    }

    private void notifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e) 
    {
        if (e.Button == System.Windows.Forms.MouseButtons.Left)
        {
            _notifyIcon.Visible = false;
            ShowWindow(window, SW_RESTORE);
        }
    }
}
