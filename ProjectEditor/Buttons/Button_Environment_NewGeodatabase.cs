using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace GSC_ProjectEditor
{
    public class Button_Environment_NewGeodatabase : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public Button_Environment_NewGeodatabase()
        {
        }

        protected override void OnClick()
        {
            Form_Environment_NewGeodatabase datasourceForm = new Form_Environment_NewGeodatabase();
            datasourceForm.ShowDialog();

        }

        protected override void OnUpdate()
        {
        }

    }
}
