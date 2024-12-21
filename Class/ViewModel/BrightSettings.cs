using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using BrightnessSimulator.Class.Command;

namespace BrightnessSimulator.Class.ViewModel
{
    public class BrightSettings
    {
        public ICommand OpenBrightSettingsCommand { get; }

        public BrightSettings()
        {
            OpenBrightSettingsCommand = new RelayCommand(OpenBrightSettings);
        }

        private void OpenBrightSettings(object parameter)
        {
            try
            {
                var uri = new Uri("ms-settings:display");
                Process.Start(new ProcessStartInfo(uri.ToString()) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                // Lidar com a exceção (log ou exibir uma mensagem)
                Console.WriteLine(ex.Message);
            }
        }
    }
}
