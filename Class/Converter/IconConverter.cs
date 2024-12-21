using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using BrightnessSimulator.Class.UI;

namespace BrightnessSimulator.Class.Converter
{
    public class IconConverter : MarkupExtension, IValueConverter
    {
        public ThemeManager ThemeManager { get; private set; }
        private static readonly bool isDarkTheme = true;
        // Caminhos dos ícones
        private static readonly  Uri brightWhtIconPath = new Uri ("/Assets/Icons/bright_wht.ico", UriKind.Relative); // Caminho do ícone claro
        private static readonly Uri brightDrkIconPath = new Uri("/Assets/Icons/bright_drk.ico", UriKind.Relative); // Caminho do ícone escuro

        private static Uri _currentIconUri;

        public static Uri CurrentIconUri
        {
            get => _currentIconUri;
            private set
            {
                if (_currentIconUri != value)
                {
                    _currentIconUri = value;
                    OnStaticPropertyChanged(nameof(CurrentIconUri));
                }
            }
        }

        // Método para fornecer o próprio objeto de conversão
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        // Método de conversão para converter um valor booleano em um caminho de imagem de ícone
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Retorna o caminho do ícone escuro ou claro com base no valor recebido

            bool isDarkTheme = (bool)value;
            return new BitmapImage(isDarkTheme ? brightWhtIconPath : brightDrkIconPath);
        
        }

        // Método de conversão de volta, não utilizado neste caso
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static event PropertyChangedEventHandler PropertyChanged;

        private static void OnStaticPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }

        private static void ThemeManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ThemeManager.IsDarkTheme))
            {
                UpdateIconUri();
            }
        }

        public static Uri GetIconUri()
        {
            if (isDarkTheme == true)
            {
                return brightWhtIconPath;
            }
            else if (isDarkTheme == false)
            {
                return brightDrkIconPath;
            }

            return null;
        }

        public static void UpdateIconUri()
        {
            if (isDarkTheme == false)
            {
                CurrentIconUri = brightDrkIconPath;
            }
            else if (isDarkTheme == true)
            {
                CurrentIconUri = brightWhtIconPath;
            }
        }

    }
}
