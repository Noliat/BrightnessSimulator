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
using BrightnessSimulator.Class.ViewModel;
using BrightnessSimulator.Class.Helpers;
using BrightnessSimulator.Class.UI;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using BrightnessSimulator.Class;
using BrightnessSimulator.Win;

namespace BrightnessSimulator
{
    public partial class MainWindow : Window
    {
        private MainViewModel ViewModel { get; set; }
        private Opaco windowOpaco;

        private ThemeManager ThemeManager { get; set; }
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
            //slider.Minimum = 0;  // Valor mínimo
            //slider.Maximum = 100;  // Valor máximo

            ViewModel = new MainViewModel();
            this.DataContext = ViewModel;

            windowOpaco = new Opaco(ViewModel);
            windowOpaco.Show();

            ViewWindow = new ViewWindow(this);

            // Carrega o último valor de brilho salvo
            int lastBrightnessValue = Properties.Settings.Default.LastBrightnessValue;
            slider.Value = lastBrightnessValue; // Seta o último valor no slider
            // Inicializa com o sistema
            //SetStartup.SetStart(true);

            // Inicia oculto
            //Visibility = Visibility.Hidden;
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

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            byte g = (byte)e.NewValue;
            SetBrightness(g);
            Label.Content = g.ToString();

            // Salva o valor do brilho no Settings
            Properties.Settings.Default.LastBrightnessValue = g;
            Properties.Settings.Default.Save(); // Salva as configurações
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sender is Slider slider)
            {
                var intValue = (int)Math.Round(slider.Value);
                slider.Value = intValue;
                ((MainViewModel)DataContext).SliderValue = intValue;
            }
        }

        private void Slider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Slider slider)
            {
                System.Windows.Point clickedPosition = e.GetPosition(slider);
                double percent = clickedPosition.X / slider.ActualWidth;

                double newValue = slider.Minimum + (percent * (slider.Maximum - slider.Minimum));

                ((MainViewModel)DataContext).SliderClickedCommand.Execute(newValue);
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
                    // Aumentando o valor do slider
                    slider.Value += 2;
                }
                // Verificando se a roda do mouse foi rolada para baixo
                else if (e.Delta < 0)
                {
                    // Diminuindo o valor do slider
                    slider.Value -= 2;
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

        // Funções da API do Windows para ajustar as propriedades da janela
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        const int GWL_EXSTYLE = -20;
        const int WS_EX_TRANSPARENT = 0x00000020;
        const int WS_EX_TOOLWINDOW = 0x00000080;

        // Evento de carregamento da janela
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);

            // Define a janela como transparente para cliques e oculta da barra de tarefas
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW);
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
            //Hide();
            WindowState = WindowState.Minimized; // Fecha a janela ao perder o foco
        }
    }
}
