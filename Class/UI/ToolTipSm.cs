using BrightnessSimulator.Class.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hardcodet.Wpf.TaskbarNotification;
using System.Windows.Controls;

namespace BrightnessSimulator.Class.UI
{
    public class ToolTipSm
    {
        private MainWindow main { get; set; }
        private TaskbarIcon notifyIcon { get; set; }

        public void UpdateToolTip(double sliderValue)
        {
            // Arredonda o valor do slider para exibir como número inteiro
            int roundedValue = (int)Math.Round(sliderValue);

            // Atualiza o ToolTip com o valor do slider
            main.slider.ToolTip = $"Brilho: {roundedValue}%";
        }
    }
}
