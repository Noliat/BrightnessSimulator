using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;

namespace BrightnessSimulator.Class
{
    public class ToggleAction
    {
        private bool isWindowVisible = false;  // Renomeando para evitar conflitos com IsVisible do Window
        public void ToggleVisibility(Window window)
        {
            if (isWindowVisible)
            {
                window.Hide();
                isWindowVisible = false;
            }
            else
            {
                window.Show();
                window.Activate();
                isWindowVisible = true;
            }
        }
    }
}
