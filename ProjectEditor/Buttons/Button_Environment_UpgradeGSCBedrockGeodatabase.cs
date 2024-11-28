using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Globalization;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Desktop.AddIns;

namespace GSC_ProjectEditor
{
    public class Button_Environment_UpgradeGSCBedrockGeodatabase : ESRI.ArcGIS.Desktop.AddIns.Button
    {

        public Button_Environment_UpgradeGSCBedrockGeodatabase()
        {
            //Set culture before init.
            Utilities.Culture.SetCulture();

        }

        protected override void OnClick()
        {
            //Start creation of a new database
            Form_Environment_UpgradeGSCBedrockGeodatabase upgradeForm = new Form_Environment_UpgradeGSCBedrockGeodatabase();
            upgradeForm.ShowDialog();


        }

    }
}
