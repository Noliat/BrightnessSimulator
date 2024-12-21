using System;
using System.Globalization;
using System.Windows.Data;

namespace BrightnessSimulator.Class.Converter
{
    public class SliderToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Converte o valor do slider (0 a 100) para opacidade (0 a 0.25)
            double sliderValue = (double)value;
            return sliderValue / 400;  // 100 / 400 = 0.25
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Converte a opacidade de volta para o valor do slider (0 a 100)
            double opacityValue = (double)value;
            return opacityValue * 400;  // 0.25 * 400 = 100
        }
    }
}
