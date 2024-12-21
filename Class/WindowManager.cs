using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using BrightnessSimulator.Win;
using System.Diagnostics;

namespace BrightnessSimulator.Class
{
    public class WindowManager
    {
        public Window Window { get; set; }
        public Slider Slider { get; set; }

        public Label Label { get; set; }
        private Opaco windowOpaco;

        private readonly bool vis = false;

        public WindowManager(Window window, Slider slider, Label label, Opaco windowOpaco)
        {
            Window = window;
            Slider = slider;

            Label = label;

            this.windowOpaco = windowOpaco;

            Slider.ValueChanged += Slider_ValueChanged;
        }

        public void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            byte g = (byte)e.NewValue;
            SetBrightness(g);
            Label.Content = g.ToString();
        }

        private int CurrentBrightness; // Valor inicial

        public void SetBrightness(byte brightnessValue)
        {
            // Converte o valor de brilho para opacidade usando a mesma lógica do conversor
            double opacity = 0.90 - (brightnessValue / 111.0);

            // Define a opacidade do elemento (por exemplo, um painel ou uma janela)
            // Um elemento que cria um 'backlightElement'
            // que simula o brilho do monitor:
            windowOpaco.Opacity = Math.Max(0, Math.Min(1, opacity)); // Garante que a opacidade esteja entre 0 e 1
        }

        public static double GetWindowOpacity(Opaco windowOpaco)
        {
            // Verifica se a janela 'Opaco' é válida e retorna o valor da opacidade atual
            if (windowOpaco != null)
            {
                return windowOpaco.Opacity;
            }
            return 1.0; // Retorna o valor de opacidade padrão caso a janela seja nula ou não esteja configurada
        }
    }
}
