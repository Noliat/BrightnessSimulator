using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;
using System.Windows.Data;
using System.Collections.Generic;
using BrightnessSimulator.Class;

namespace BrightnessSimulator.Class.UI
{
    public class ThemeManager
    {
        private static Dictionary<string, ResourceDictionary> themes = new Dictionary<string, ResourceDictionary>();
        private static string currentThemeName;
        private static bool isDarkTheme = true;
        internal static Action<object, EventArgs> ThemeChanged;

        public static string CurrentThemeName
        {
            get { return currentThemeName; }
            private set
            {
                if (currentThemeName != value)
                {
                    currentThemeName = value;
                    OnPropertyChanged(nameof(CurrentThemeName));

                }
            }
        }

        public static bool IsDarkTheme
        {
            get { return isDarkTheme; }
            private set
            {
                if (isDarkTheme != value)
                {
                    isDarkTheme = value;
                    OnPropertyChanged(nameof(IsDarkTheme));
                }
            }
        }

        public static event PropertyChangedEventHandler PropertyChanged;

        static ThemeManager()
        {
            LoadThemes();
            ApplyTheme("Light"); // Aplicando o tema padrão ao inicializar
            StartThemeUpdateTimer();
        }

        public static void LoadThemes()
        {
            // Adicione seus temas ao dicionário de temas
            AddTheme("Light", "/BrightnessSimulator;component/Themes/LightTheme.xaml");
            AddTheme("Dark", "/BrightnessSimulator;component/Themes/DarkTheme.xaml");
            // Adicione mais temas conforme necessário
        }

        public static void AddTheme(string themeName, string themePath)
        {
            ResourceDictionary themeDictionary = new ResourceDictionary()
            {
                Source = new Uri(themePath, UriKind.Relative)
            };

            themes[themeName] = themeDictionary;
        }

        public static void ApplyTheme(string themeName)
        {
            if (themes.ContainsKey(themeName))
            {
                Application.Current.Resources.Clear();
                Application.Current.Resources.MergedDictionaries.Add(themes[themeName]);
                CurrentThemeName = themeName;
            }
            else
            {
                throw new ArgumentException("Theme not found.", nameof(themeName));
            }
        }

        public static bool IsSystemDarkTheme()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                {
                    if (key != null && int.TryParse(key.GetValue("SystemUsesLightTheme").ToString(), out int lightThemeValue))
                    {
                        return lightThemeValue == 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // Tratar exceções ao acessar o registro
                Console.WriteLine($"Erro ao acessar o registro: {ex.Message}");
            }
            return false;
        }

        public static void StartThemeUpdateTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(10); // Verificar a cada 10 segundos
            timer.Tick += (sender, e) => UpdateThemeBasedOnSystem();
            timer.Start();
        }

        public static void UpdateThemeBasedOnSystem()
        {
            // Lógica para atualizar o tema com base no sistema operacional
            // Por exemplo, verificar se o sistema usa tema claro ou escuro
            bool systemDarkTheme = IsSystemDarkTheme();
            string themeToApply = systemDarkTheme ? "Dark" : "Light";
            if (CurrentThemeName != themeToApply)
            {
                ApplyTheme(themeToApply);
            }
        }

        public static void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }

        public static void ToggleTheme()
        {
            UpdateThemeBasedOnSystem();
        }
    }
}
