using System;
using System.Globalization;
using System.Windows.Data;

namespace BrightnessSimulator.Class.Converter
{
    public class SliderToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double sliderValue = (double)value;
            return sliderValue / 100.0; // Transforma o valor do slider para opacidade
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double opacity = (double)value;
            return opacity * 100.0; // Transforma de volta para o valor do slider
        }
    }
}
