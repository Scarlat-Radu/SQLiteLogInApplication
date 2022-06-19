using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WPF_SQLite_Demo
{
    /// <summary>
    /// HelperClass
    /// </summary>
    public class Helper
    {
        /// <summary>
        /// 
        /// </summary>
        public static bool FirstStart = true;
        /// <summary>
        /// Keeps track of generated main windows while tabs are beeing puled from panel.
        /// Every time a tab is pulled from the main window adds it to the list.
        /// TODO: remove from list tabs that are merged again with the main window
        /// </summary>
        public static List<MainWindow> mainWindow = new List<MainWindow>();
        public class Get
        {
            public static string PortConfigDirectory()
            {
                string pathPortConfig = AppDomain.CurrentDomain.BaseDirectory + "\\Data";

                return pathPortConfig;
            }
            public static List<string> XMLFiles(DirectoryInfo directoryInfo)
            {
                List<string> XMLFiles = new List<string>();
                XMLFiles.AddRange(directoryInfo.GetFiles("*.xml")
                                    .Where(file => file.Name.EndsWith(".xml"))
                                    .Select(file => file.Name).ToList());

                return XMLFiles;
            }
            public static string StartUpDirectory()
            {
                string startupPath = AppDomain.CurrentDomain.BaseDirectory;

                return startupPath;
            }
        }
        public static ITheme theme;

        public static void setTheme(bool isDark)
        {
            var paletteHelper = new PaletteHelper();
            //Retrieve the app's existing theme
            theme = paletteHelper.GetTheme();

            //Change the base theme to Dark
            if (isDark)
                theme.SetBaseTheme(Theme.Dark);
            else
                theme.SetBaseTheme(Theme.Light);
            //or theme.SetBaseTheme(Theme.Light);

            //Change all of the primary colors to Red
            theme.SetPrimaryColor(Colors.Red);

            //Change all of the secondary colors to Blue
            theme.SetSecondaryColor(Colors.Blue);

            //You can also change a single color on the theme, and optionally set the corresponding foreground color
            theme.PrimaryMid = new ColorPair(Colors.Brown, Colors.White);

            var c = Color.FromRgb(64, 104, 130);
            var b = Color.FromRgb(0, 0, 0);
            Color primaryColor = Color.FromArgb(c.A, c.R, c.G, c.B);// SwatchHelper.Lookup[MaterialDesignColor.DeepPurple];
            Color accentColor = Color.FromArgb(b.A, b.R, b.G, b.B); //SwatchHelper.Lookup[MaterialDesignColor.Lime];
            if (isDark)
                theme = Theme.Create(new MaterialDesignDarkTheme(), primaryColor, accentColor);
            else
                theme = Theme.Create(new MaterialDesignLightTheme(), primaryColor, accentColor);
            //secondaryAccent = Color.FromArgb(theme.PrimaryDark.Color.A, theme.PrimaryDark.Color.R, theme.PrimaryDark.Color.G, theme.PrimaryDark.Color.B);



            //Change the app's current theme
            paletteHelper.SetTheme(theme);
        }
    }
}
