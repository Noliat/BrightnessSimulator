using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Threading;

namespace BrightnessSimulator.Class
{
    public class WindowMonitor
    {
        // Função para enumerar todas as janelas
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        // Delegate que define o callback de enumeração
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        // Função para obter o estilo de uma janela
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        // Função para definir o estilo de uma janela
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        // Constantes para modificar o estado da janela
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOPMOST = 0x00000008;

        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOACTIVATE = 0x0010;

        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);

        public void StartMonitoring()
        {
            EnumWindows(new EnumWindowsProc(CheckTopmostWindow), IntPtr.Zero);
        }

        private bool CheckTopmostWindow(IntPtr hWnd, IntPtr lParam)
        {
            // Obter o estilo da janela
            int style = GetWindowLong(hWnd, GWL_EXSTYLE);

            // Verifica se a janela tem o estilo TOPMOST
            if ((style & WS_EX_TOPMOST) != 0)
            {
                // Redefine o Topmost da janela
                ResetTopmost(hWnd);
            }

            return true; // Continue a enumeração
        }

        private void ResetTopmost(IntPtr hWnd)
        {
            // Remove o estilo TOPMOST da janela
            SetWindowPos(hWnd, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);
        }

        public class WindowMonitorService
        {
            private WindowMonitor monitor;
            private DispatcherTimer timer;

            public WindowMonitorService()
            {
                monitor = new WindowMonitor();
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1); // Verifica a cada 1 segundo
                timer.Tick += Timer_Tick;
                timer.Start();
            }

            private void Timer_Tick(object sender, EventArgs e)
            {
                monitor.StartMonitoring();
            }
        }
    }
}
