using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace GSC_ProjectEditor.Utilities
{
    class Culture
    {
        /// <summary>
        /// Will set culture for current addin.
        /// </summary>
        public static void SetCulture()
        {
            try
            {
                //Apply current culture to current thread
                Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.CurrentUICulture;

                // Manage if Culture is already set within project
                if (GSC_ProjectEditor.Properties.Settings.Default.Culture != System.Globalization.CultureInfo.CurrentUICulture)
                {
                    //Add culture to current setting
                    GSC_ProjectEditor.Properties.Settings.Default.Culture = System.Globalization.CultureInfo.CurrentUICulture;
                    GSC_ProjectEditor.Properties.Settings.Default.LANGUAGE = System.Globalization.CultureInfo.CurrentUICulture;
                    GSC_ProjectEditor.Properties.Settings.Default.Save();
                }

            }
            catch (Exception setCultureError)
            {
                MessageBox.Show("Culture.cs error: " + setCultureError.Message);
            }

        }

    }
}
