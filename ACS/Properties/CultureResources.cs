using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Forms;

namespace ACS.Properties
{
    public class CultureResources
    {
        private static bool _foundInstalledCultures = false;

        public static List<CultureInfo> SupportedCultures { get; } = new List<CultureInfo>();

        static CultureResources()
        {
            if (!_foundInstalledCultures)
            {
                foreach (var dir in Directory.GetDirectories(Application.StartupPath))
                {
                    try
                    {
                        //see if this directory corresponds to a valid culture name
                        var dirinfo = new DirectoryInfo(dir);
                        var tCulture = CultureInfo.GetCultureInfo(dirinfo.Name);

                        //determine if a resources dll exists in this directory that matches the executable name
                        if (dirinfo.GetFiles(Path.GetFileNameWithoutExtension(Application.ExecutablePath) + ".resources.dll").Length > 0)
                        {
                            SupportedCultures.Add(tCulture);
                        }
                    }
                    catch (ArgumentException) { }
                }
                _foundInstalledCultures = true;
            }
        }

        /// <summary>
        /// The Resources ObjectDataProvider uses this method to get an instance of the WPFLocalize.Properties.Resources class
        /// </summary>
        /// <returns></returns>
        public static Resources GetResourceInstance()
        {
            return new Resources();
        }

        private static ObjectDataProvider m_provider;
        public static ObjectDataProvider ResourceProvider
        {
            get
            {
                if (m_provider == null)
                    m_provider = (ObjectDataProvider)App.Current.FindResource("Resources");
                return m_provider;
            }
        }

        /// <summary>
        /// Change the current culture used in the application.
        /// If the desired culture is available all localized elements are updated.
        /// </summary>
        /// <param name="culture">Culture to change to</param>
        public static void ChangeCulture(CultureInfo culture)
        {
            if (SupportedCultures.Contains(culture))
            {
                Resources.Culture = culture;
                ResourceProvider.Refresh();
            }
        }
    }
}
