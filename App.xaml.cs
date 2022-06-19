using MaterialDesignColors.Recommended;
using MaterialDesignThemes.MahApps;
using MaterialDesignThemes.Wpf;
using System.Windows;
using MaterialDesignColors;
using System.Drawing;
using System.Windows.Media;

namespace WPF_SQLite_Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ITheme theme;
        public static Color secondaryAccent;
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Warning);
            e.Handled = true;
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            SetTheme(true);
            Resources["SecondaryAccentBrush"] = new SolidColorBrush(secondaryAccent);
        }
        public static void SetTheme(bool isDark)
        {
            var c = Color.FromRgb(64, 104, 130);
            var b = Color.FromRgb(0, 0, 0);
            Color primaryColor = Color.FromArgb(c.A, c.R, c.G, c.B);
            Color accentColor = Color.FromArgb(b.A, b.R, b.G, b.B);
            if (isDark)
                theme = Theme.Create(new MaterialDesignDarkTheme(), primaryColor, accentColor);
            else
                theme = Theme.Create(new MaterialDesignLightTheme(), primaryColor, accentColor);
            secondaryAccent = Color.FromArgb(theme.PrimaryDark.Color.A, theme.PrimaryDark.Color.R, theme.PrimaryDark.Color.G, theme.PrimaryDark.Color.B);
        }
    }
}
