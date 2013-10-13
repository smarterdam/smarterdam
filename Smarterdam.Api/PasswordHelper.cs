using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Smarterdam.Api.Helpers
{
    /// <summary>
    /// Poor man's security
    /// </summary>
    public class PasswordHelper
    {
        private static string folder;

        static PasswordHelper()
        {
            var curPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase).Replace("file:\\", "");
            var curDir = new DirectoryInfo(curPath);
            folder = Path.Combine(curDir.Parent.Parent.FullName, "passwords");
        }

        public static string WebPassword
        {
            get { return ReadPassword("web"); }
        }

        public static string EcoScadaServicePassword 
        {
            get { return ReadPassword("ecoscada"); }
        }

        static string ReadPassword(string name)
        {
            var filePath = Path.Combine(folder, name);
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }
            else
            {
                return null;
            }
        }
    }
}
