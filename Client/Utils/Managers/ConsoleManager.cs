using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Blaze.Utils.Managers
{
    internal static class ConsoleManager
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetConsoleOutputCP(uint wCodePageID);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetConsoleCP(uint wCodePageID);

        [DllImport("kernel32.dll")]
        internal static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("kernel32.dll")]
        internal static extern int AllocConsole();

        public const int SW_HIDE = 0;
        public const int SW_SHOW = 5;
    }
}
