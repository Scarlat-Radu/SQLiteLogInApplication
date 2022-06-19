using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_SQLite_Demo
{
    public class Helper
    {
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
    }
}
