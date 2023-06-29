using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Demos
{
    public static class NativeMethods
    {
        /// <summary>
        /// GetPrivateProfileString
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="def"></param>
        /// <param name="retVal"></param>
        /// <param name="size"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        /// <summary>
        /// WritePrivateProfileString
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        /// <summary>
        /// ReadIni
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="fileName">Ini filename</param>
        /// <param name="section">Section name</param>
        /// <param name="key">Key name</param>
        /// <param name="defaultValue">Default T value</param>
        /// <returns></returns>
        public static T ReadIni<T>(string fileName, string section, string key, T defaultValue = default(T))
        {
            const int size = 255;
            var sb = new StringBuilder(size);
            GetPrivateProfileString(section, key, string.Empty, sb, size, fileName);
            if (string.IsNullOrEmpty(sb.ToString()))
                return defaultValue;
            return (T)Convert.ChangeType(sb.ToString(), typeof(T));
        }

        /// <summary>
        /// WriteIni
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="fileName">Ini filename</param>
        /// <param name="section">Section name</param>
        /// <param name="key">Key name</param>
        /// <param name="value">T value</param>
        public static void WriteIni<T>(string fileName, string section, string key, T value)
        {
            WritePrivateProfileString(section, key, value?.ToString(), fileName);
        }
    }
}
