using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using BrightnessSimulator.Class.Converter;
using BrightnessSimulator.Class.Command;

namespace BrightnessSimulator.Class.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private bool _isDarkTheme;
        
        public MainViewModel()
        {
            SliderClickedCommand = new RelayCommand(ExecuteSliderClicked);
            IconConverter.UpdateIconUri();
        }

        private double _sliderValue;
        public double SliderValue
        {
            get => _sliderValue;
            set
            {
                // Arredondar para o inteiro mais próximo
                _sliderValue = Math.Round(value);
                OnPropertyChanged(nameof(SliderValue)); // Notifica que a propriedade mudou
            }
        }

        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set
            {
                if (_isDarkTheme != value)
                {
                    _isDarkTheme = value;
                    OnPropertyChanged(nameof(IsDarkTheme));
                }
            }
        }

        private int _currentBrightnessLevel;

        public int GetCurrentBrightnessLevel()
        {
            return _currentBrightnessLevel;
        }

        private double _updateTooTip;
        public double UpdateToolTip
        {
            get => _updateTooTip;
            set
            {
                // Arredondar para o inteiro mais próximo
                _updateTooTip = Math.Round(value);
                OnPropertyChanged(nameof(UpdateToolTip)); // Notifica que a propriedade mudou
            }
        }

        public ICommand SliderClickedCommand { get; set; }

    private void ExecuteSliderClicked(object parameter)
     {
            double newValue = Convert.ToDouble(parameter);
            SliderValue = newValue;
     }
        
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(field, newValue))
            {
                field = newValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }

            return false;
        }
    }
}
