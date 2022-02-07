using System;
using ExcelTest.Mt940;
using System.IO;
using System.Runtime.InteropServices;
using System.Configuration;

namespace ExcelTest
{
    class Program
    {
        private static readonly string filePath = ConfigurationManager.AppSettings.Get("filePath");
        private static void Main()
        {
            AllocConsole();
            ShowWindow(GetConsoleWindow(), 0);

            using var watcher = new FileSystemWatcher(filePath);

            watcher.Created += OnCreated;
            watcher.Filter = "*.txt";
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;

            new System.Threading.AutoResetEvent(false).WaitOne();
        }

        private static void OnCreated(object sender, FileSystemEventArgs e) => _ = new Mt940Added(e, filePath);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
    }
}