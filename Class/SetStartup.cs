using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrightnessSimulator.Class
{
    public class SetStartup
    {
        public void SetStart(bool RunAtStartup)
        {
            string appName = "BrightnessSimulator"; // Nome do aplicativo
            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            RegistryKey rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (RunAtStartup)
            {
                rk.SetValue(appName, exePath);
            }
            else
            {
                rk.DeleteValue(appName, true);
            }
        }

        public bool IsRunAtStartup()
        {
            string appName = "BrightnessSimulator"; // Nome do aplicativo
            RegistryKey rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (rk.GetValue(appName) == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
