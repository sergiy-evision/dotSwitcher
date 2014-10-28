using System;
using System.Windows.Forms;

namespace dotSwitcher
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var engine = new Switcher();
            Application.ApplicationExit += (s, a) => engine.Dispose();
            Application.Run(new SysTrayApp(engine));
        }
    }
}