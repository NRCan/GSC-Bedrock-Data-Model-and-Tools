using ESRI.ArcGIS.Desktop.AddIns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace BedrockEditorPro.Utilities
{
    public class WorkingEnvironment
    {
        //The path were configuration and user visible settings are stored.
        public string WorkingEnvironmentPath
        {
            get { return Preferences.Get(nameof(WorkingEnvironmentPath), ""); }
            set { Preferences.Set(nameof(WorkingEnvironmentPath), value); }
        }

        public WorkingEnvironment()
        {
            SetWorkingEnvironment();
        }

        private void SetWorkingEnvironment()
        {
            if (WorkingEnvironmentPath == string.Empty)
            {
                // 1. get the filename of this .esriAddinX file
                var fileName = AddIn.GetAddInId();
                // 2. get the config.daml content from the esriAddinX file
                var versionTuple = AddIn.GetConfigDamlAddInInfo(fileName);
                // 3. set the WorkingEnvironmentPath to the default path
                string outputFolderName = System.IO.Path.Combine(Constants.ESRI.defaultArcGISFolderName, versionTuple.Name + " " + versionTuple.Version);
                WorkingEnvironmentPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), outputFolderName);
                
            }
        }
    }
}
