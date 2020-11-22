using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Matrix_Elementary.Scripts
{
    public static class Cache
    {
        private static string appdata;

        public static void CheckFirstRun()
        {
            appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Matrix Elementary\\";
            if (!File.Exists(appdata + "prefs"))
            {
                Directory.CreateDirectory(appdata);
                WriteCache(false, false);
            }
        }

        public static void WriteCache(bool dark_mode, bool easter_found)
        {
            File.WriteAllText(appdata + "prefs",
                Convert.ToInt32(dark_mode).ToString() + Convert.ToInt32(easter_found));
        }

        public static bool[] ReadCache()
        {
            string data = File.ReadAllText(appdata + "prefs");
            return new bool[] { data[0] == '1', data[1] == '1' };
        }
    }
}
