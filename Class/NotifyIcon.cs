using System;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Forms;

using BrightnessSimulator.Class.UI;
using Hardcodet.Wpf.TaskbarNotification;

namespace BrightnessSimulator.Class
{
    public class NotifyIcon : IDisposable
    {
        private readonly NotifyIcon notifyIcon;
        private readonly Action toggleVisibilityAction;

        public ThemeManager ThemeManager { get; private set; }
        private static readonly bool isDarkTheme;

        public NotifyIconManager(Action toggleVisibility, bool IsDarkTheme)
        {
            toggleVisibilityAction = toggleVisibility;
            notifyIcon = new NotifyIcon
            {
                Visible = true
            };
            notifyIcon.MouseClick += NotifyIcon_MouseClick;
            notifyIcon.Text = "Brightness Control";
            UpdateToolTip();
            UpdateIcon();

            // Inscreva-se no evento ThemeChanged
            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;

            // Configurar um temporizador para atualizar a dica do balão a cada intervalo
            Timer timer = new Timer()
            {
                Interval = 500 // Define o intervalo em milissegundos (0,5 segundo)
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Atualiza a dica do balão com o nível de brilho atual
            UpdateToolTip();
        }

        private void ThemeManager_ThemeChanged(object sender, EventArgs e)
        {
            // Atualizar o ícone com base no novo tema
            UpdateIcon();
        }

        private MainWindow main { get; set; }

        public void UpdateToolTip(double sliderValue)
        {
            // Arredonda o valor do slider para exibir como número inteiro
            int roundedValue = (int)Math.Round(sliderValue);

            // Atualiza o ToolTip com o valor do slider
            main.slider.ToolTip = $"Brilho: {roundedValue}%";
        }

        public void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Chame a ação de alternância de visibilidade
                toggleVisibilityAction();
            }
        }

        public void Dispose()
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
        }
    }
}
