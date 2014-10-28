using System;
using System.Windows.Forms;
using dotSwitcher.Properties;

namespace dotSwitcher
{
    public class SysTrayApp : Form
    {
        private readonly Switcher _engine;
        private readonly NotifyIcon _trayIcon;
        private readonly MenuItem _power;

        public SysTrayApp(Switcher engine)
        {
            _engine = engine;
            var trayMenu = new ContextMenu();
            _power = new MenuItem("", OnPower);
            trayMenu.MenuItems.Add(_power);
            trayMenu.MenuItems.Add("Exit", OnExit);


            _trayIcon = new NotifyIcon
            {
                Text = Resources.SysTrayApp_SysTrayApp_dotSwitcher,
                Icon = Resources.icon,
                ContextMenu = trayMenu,
                Visible = true
            };
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false;
            ShowInTaskbar = false;

            _engine.Error += OnEngineError;
            _engine.Start();
            UpdateMenu();
            base.OnLoad(e);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            _engine.Error -= OnEngineError;
            base.OnClosing(e);
        }

        private void OnEngineError(object sender, SwitcherErrorArgs args)
        {
            var ex = args.Error;
            _trayIcon.ShowBalloonTip(2000, "dotSwitcher error", ex.ToString(), ToolTipIcon.None);
        }

        private static void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void UpdateMenu()
        {
            _power.Text = _engine.IsStarted() ? "Turn off" : "Turn on";
        }

        private void OnPower(object sender, EventArgs e)
        {
            if (_engine.IsStarted()) { _engine.Stop(); }
            else { _engine.Start(); }
            UpdateMenu();
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                _trayIcon.Dispose();
            }
            base.Dispose(isDisposing);
        }
    }
}
