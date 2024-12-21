using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

using BrightnessSimulator.Class.Command;
using BrightnessSimulator.Class.ViewModel;

namespace BrightnessSimulator.Class.Command
{
    public class NotifyIconViewModel
    {
        private bool IsVisible = false;
        private readonly MainViewModel _mainViewModel;
        
        public NotifyIconViewModel()
        {
            _mainViewModel = new MainViewModel(); // Referência ao MainViewModel

            brightSettings = new BrightSettings();

            // Inicialize o ToolTip com o valor atual do brilho
            UpdateBrightnessToolTip();
        }
        public ICommand ToggleWindowVisibilityCommand
        { 
            get
            {
                return new RelayCommand(
                    execute: (obj) =>
                    {
                        // Alterna a visibilidade da janela
                        if (IsVisible)
                        {
                            // Se a janela está visível, oculta
                            Application.Current.MainWindow.Hide();
                            IsVisible = false;
    }
                        else
                        {
                            // Se a janela está oculta, exibe e ativa
                            Application.Current.MainWindow.Show();
                            Application.Current.MainWindow.Activate(); // Garante que a janela receba o foco
                            Application.Current.MainWindow.Topmost = false;
                            IsVisible = true;
                        }
                    },
                    canExecute: (obj) => true // Sempre pode ser executado
                );
            }
        }

        /// <summary>
        /// Fecha a janela se estiver aberta.
        /// </summary>
        public ICommand CloseWindowCommand
        {
            get
            {
                return new RelayCommand(
                    execute: (obj) =>
                    {
                        if (Application.Current.MainWindow != null)
                        {
                            Application.Current.MainWindow.Close();
                            Application.Current.MainWindow = null; // Libera a referência após fechar
                        }
                    },
                    canExecute: (obj) => Application.Current.MainWindow != null
                );
            }
        }

        /// <summary>
        /// Fecha a aplicação.
        /// </summary>
        public ICommand ExitApplicationCommand
        {
            get
            {
                return new RelayCommand(
                    execute: (obj) => Application.Current.Shutdown()
                );
            }
        }

        /// <summary>
        /// Comando para abrir as configurações de Bluetooth.
        /// </summary>
        public readonly BrightSettings brightSettings;
        public ICommand OpenBrightSettingsCommand => brightSettings.OpenBrightSettingsCommand;

        private string _brightnessToolTip;

        public string BrightnessToolTip
        {
            get => _brightnessToolTip;
            set
            {
                if (_brightnessToolTip != value)
                {
                    _brightnessToolTip = value;
                    OnPropertyChanged(nameof(BrightnessToolTip));
                }
            }
        }

        private MainWindow main { get; set; }
        // Propriedade para armazenar o valor de opacidade
        public double WindowOpacity { get; set; }

        // Método para atualizar o ToolTip com o valor da opacidade
        public void UpdateBrightnessToolTip()
        {
            // Arredonda o valor da opacidade para exibir como número inteiro
            int roundedOpacity = (int)Math.Round(WindowOpacity * 100); // Multiplica por 100 para percentual

            // Atualiza o ToolTip com o valor da opacidade
            BrightnessToolTip = $"Brightness Level: {roundedOpacity}%";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

