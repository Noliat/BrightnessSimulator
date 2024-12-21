using System;
using System.Globalization;
using System.Windows.Data;

namespace BrightnessSimulator.Class.Converter
{
    public class SliderToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Considerando que o slider vai de 0 a 100
            double sliderValue = (double)value;

            // Convertendo o valor do slider para um valor de opacidade entre 0 e 0.25
            double opacity = 0.90 - (sliderValue / 111); // Subtraímos para inverter o efeito.

            return opacity;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
