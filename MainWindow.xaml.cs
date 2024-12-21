using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Resources;
using System.ComponentModel;
using System.Diagnostics;

using BrightnessSimulator.Class.ViewModel;
using BrightnessSimulator.Class.Helpers;
using BrightnessSimulator.Class.UI;
using BrightnessSimulator.Class;
using BrightnessSimulator.Win;
using System.Runtime.Remoting.Channels;
using System.Threading;
using BrightnessSimulator.Class.Command;

namespace BrightnessSimulator
{
    public partial class MainWindow : Window
    {
        private MainViewModel ViewModel { get; set; }
        private Opaco windowOpaco;
        private WindowMonitor monitor { get; set; }
        private WindowManager windowManager;
        private NotifyIconViewModel viewModel;

        public ThemeManager ThemeManager { get; private set; }
        public ViewWindow ViewWindow { get; set; }
        private SetStartup SetStartup;

        public MainWindow()
        {
            InitializeComponent();
            KeyAction();

            ThemeManager = new ThemeManager();
            EnableAcrylic();

            slider.ValueChanged += Slider_ValueChanged;
            slider.PreviewMouseDown += Slider_PreviewMouseDown;

            this.PreviewMouseWheel += MainWindow_PreviewMouseWheel;

            ViewModel = new MainViewModel();
            this.DataContext = ViewModel;

            windowOpaco = new Opaco(ViewModel); // instância da janela `Opaco`
            double opacity = WindowManager.GetWindowOpacity(windowOpaco); // recupera a opacidade atual
            windowOpaco.Show(); // mostra a janela `Opaco`
            this.Topmost = true;

            ViewWindow = new ViewWindow(this);
            monitor = new WindowMonitor();
            windowManager = new WindowManager(this, slider, label, windowOpaco);
            //viewModel.UpdateBrightnessToolTip();  // Atualiza o ToolTip uma vez ao inicializar
            
            Deactivated += MainWindow_Deactivated;

            // Carrega o último valor de brilho salvo
            int lastBrightnessValue = Properties.Settings.Default.LastBrightnessValue;
            slider.Value = lastBrightnessValue; // Seta o último valor no slider
            windowManager.SetBrightness((byte)lastBrightnessValue); // Aplica o brilho inicial

            // Oculta da barra de tarefa
            ShowInTaskbar = false;

            // Inicializa com o sistema
            //SetStartup.SetStart(true);

            // Inicia oculto
            Visibility = Visibility.Hidden;
        }

        private void MainWindow_Deactivated(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void EnableAcrylic()
        {
            // Obter a cor do Background do Grid
            var gridBackground = ((SolidColorBrush)Grid.Background).Color;

            // Converter para formato ARGB
            int gradientColor = (gridBackground.A) |
                                (gridBackground.R) |
                                (gridBackground.G) |
                                (gridBackground.B);

            // Aplicar o efeito acrílico com a cor do Background
            AcrylicBlush.EnableAcrylic(this, gradientColor);
        }

        private void ThemeManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentThemeName")
            {
            }
        }

        public void Opaco_Loaded(object sender, RoutedEventArgs e)
        {
            Window_Loaded(sender, e);
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

        public ToolTipSm ToolTipSm;
        private Label Label{ get; set; }
        
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            byte g = (byte)e.NewValue;

            if (Label != null) // Verifica se Label não é null
            {
                Label.Content = g.ToString();
            }
            else
            {
                // Trate o caso em que Label é null
                Console.WriteLine("Label não inicializado!");
            }
            windowManager.SetBrightness(g);

            // Atualiza o ToolTip com o valor atual do slider
            if (viewModel.BrightnessToolTip != null)
            {
                viewModel.WindowOpacity = e.NewValue; // Atualiza o valor de opacidade
                viewModel.UpdateBrightnessToolTip();
            }
            else
            {
                Console.WriteLine("ToolTipSm não inicializado!");
            }

            // Salva o valor do brilho no Settings
            Properties.Settings.Default.LastBrightnessValue = g;
            Properties.Settings.Default.Save(); // Salva as configurações
        }

        private void Slider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Slider slider)
            {
                System.Windows.Point clickedPosition = e.GetPosition(slider);
                double percent = clickedPosition.X / slider.ActualWidth;

                // Calcule o novo valor do slider e arredonde para um inteiro
                double newValue = Math.Round(slider.Minimum + (percent * (slider.Maximum - slider.Minimum)));

                slider.Value = newValue; // Atualiza o slider com o valor arredondado
                ((MainViewModel)DataContext).SliderValue = newValue; // Atualiza o ViewModel
            }
        }


        private void MainWindow_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Verificando se o mouse está sobre a janela principal
            if (IsMouseOverMainWindow())
            {
                // Verificando se a roda do mouse foi rolada para cima
                if (e.Delta > 0)
                {
                    // Aumentando o valor do slider, mas não ultrapassando 100
                    if (slider.Value < slider.Maximum)
                    {
                        slider.Value = Math.Min(slider.Value + 2, slider.Maximum);
                    }
                }
                // Verificando se a roda do mouse foi rolada para baixo
                else if (e.Delta < 0)
                {
                    // Diminuindo o valor do slider, mas não abaixo de 0
                    if (slider.Value > slider.Minimum)
                    {
                        slider.Value = Math.Max(slider.Value - 2, slider.Minimum);
                    }
                }

                // Impedindo que o evento seja propagado para outros elementos
                e.Handled = true;
            }
        }

        private bool IsMouseOverMainWindow()
        {
            // Verificando se a posição do mouse está dentro dos limites da janela principal
            Point mousePos = Mouse.GetPosition(this);
            return (mousePos.X >= 0 && mousePos.X <= this.ActualWidth && mousePos.Y >= 0 && mousePos.Y <= this.ActualHeight);
        }

        private DispatcherTimer keyPressTimer;
        private bool isKeyHeldDown = false;
        private Key currentKey;

        // Método de inicialização da ação do teclado
        private void KeyAction()
        {
            // Inicializa o temporizador
            keyPressTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50) // Ajuste o intervalo conforme necessário
            };
            keyPressTimer.Tick += KeyPressTimer_Tick;

            // Eventos para detecção de teclas pressionadas
            this.PreviewKeyDown += Slider_KeyDown;
            this.PreviewKeyUp += Slider_KeyUp;
        }

        // Evento que é disparado continuamente enquanto a tecla estiver pressionada
        private void KeyPressTimer_Tick(object sender, EventArgs e)
        {
            if (isKeyHeldDown)
            {
                if (currentKey == Key.Right && slider.Value < slider.Maximum)
                {
                    slider.Value += 1;  // Aumenta o valor do slider
                }
                else if (currentKey == Key.Left && slider.Value > slider.Minimum)
                {
                    slider.Value -= 1;  // Diminui o valor do slider
                }
            }
        }

        // Evento que captura a tecla pressionada
        private void Slider_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Right || e.Key == Key.Left) && !isKeyHeldDown)
            {
                currentKey = e.Key;
                isKeyHeldDown = true;

                // Inicia o temporizador para aumentar/diminuir repetidamente
                keyPressTimer.Start();
            }
            else if (!isKeyHeldDown)
            {
                isKeyHeldDown = false;
                keyPressTimer.Stop();
            }

            // Previne que outros controles recebam esse evento, se necessário
            e.Handled = true;
        }

        // Evento que captura a liberação da tecla
        private void Slider_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right || e.Key == Key.Left)
            {
                isKeyHeldDown = false;

                // Para o temporizador quando a tecla é liberada
                keyPressTimer.Start();
            }
            else if (!isKeyHeldDown)
            {
                isKeyHeldDown = true;
                keyPressTimer.Stop();
            }

            // Previne que outros controles recebam esse evento, se necessário
            e.Handled = true;
        }

    //protected override void OnDeactivated(EventArgs e)
    //{
    //  base.OnDeactivated(e);
    // Hide();
    //WindowState = WindowState.Minimized; // Fecha a janela ao perder o foco
    //}
}
}
