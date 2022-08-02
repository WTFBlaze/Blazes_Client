using System;
using System.Runtime.InteropServices;

namespace Blaze.Utils.Managers
{
    public static class ConsoleManager
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleOutputCP(uint wCodePageID);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleCP(uint wCodePageID);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("kernel32.dll")]
        public static extern int AllocConsole();

        public const int SW_HIDE = 0;
        public const int SW_SHOW = 5;
    }
}
