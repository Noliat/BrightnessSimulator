using BrightnessSimulator.Class.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BrightnessSimulator.Win
{
    /// <summary>
    /// Lógica interna para Opaco.xaml
    /// </summary>
    public partial class Opaco : Window
    {
        public Opaco(MainViewModel ViewModel)
        {
            this.InitializeComponent();

            this.DataContext = ViewModel;
            this.Loaded += Opaco_Loaded;

            this.WindowStyle = WindowStyle.None;
            this.ResizeMode = ResizeMode.NoResize;
            this.ShowInTaskbar = false;
            this.Topmost = true;

            // Chama método para ajustar a janela ao tamanho da tela ao carregar
            AjustarParaTela();

            // Registra o evento para capturar alterações de resolução
            SystemEvents.DisplaySettingsChanged += OnDisplaySettingsChanged;

        }

        public void Opaco_Loaded(object sender, RoutedEventArgs e)
        {
            Window_Loaded(sender, e);
            Window_LostFocus(sender, e);
        }

        // Importar função SetWindowPos da User32.dll
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);

        // Constantes para SetWindowPos
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const uint SWP_SHOWWINDOW = 0x0040;

        // Funções da API do Windows para ajustar as propriedades da janela
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        // Constantes para a barra de tarefas
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        const int GWL_EXSTYLE = -20;
        const int WS_EX_TRANSPARENT = 0x00000020;
        const int WS_EX_TOOLWINDOW = 0x00000080;

        // Evento de carregamento da janela
        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);

            // Manter a janela sempre no topo
            SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);

            // A barra de tarefas tem uma classe chamada "Shell_TrayWnd"
            IntPtr taskbarHandle = FindWindow("Shell_TrayWnd", null);


            // Trazer a Taskbar para frente, logo abaixo da janela Opaco
            SetWindowPos(taskbarHandle, hwnd, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);

            // Define a janela como transparente para cliques e oculta da barra de tarefas
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            // Força a janela a permanecer no topo
            ForceTopmost();
        }

        // Sempre que a janela perder o foco, forçar para o topo
        public void Window_Deactivated(object sender, EventArgs e)
        {
            ForceTopmost();
        }

        public void Window_LostFocus(object sender, RoutedEventArgs e)
        {
            ForceTopmost();
        }

        public void ForceTopmost()
        {
            var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);
        }

        private void AjustarParaTela()
        {
            // Ajusta o tamanho e posição para ocupar toda a área visível
            Left = 0;
            Top = 0;
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;
        }

        private void OnDisplaySettingsChanged(object sender, EventArgs e)
        {
            // Redimensiona a janela sempre que houver mudanças na configuração da tela
            AjustarParaTela();
        }

        // Não esqueça de cancelar o registro do evento ao fechar para evitar vazamentos de memória
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            SystemEvents.DisplaySettingsChanged -= OnDisplaySettingsChanged;
        }
    }
}
