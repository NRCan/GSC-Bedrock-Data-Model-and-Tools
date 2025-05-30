using ArcGIS.Desktop.Framework.Contracts;
using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Layouts;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.KnowledgeGraph;
using System.Windows.Media;

namespace BedrockEditorPro.ProWindows
{
    class Show_Form_Environment_NewGeodatabase: Button
    {
        private Form_Environment_NewGeodatabase _form_environment_newgeodatabase = null;

        protected override void OnClick()
        {
            //already open?
            if (_form_environment_newgeodatabase != null)
                return;

            _form_environment_newgeodatabase = new Form_Environment_NewGeodatabase();
            _form_environment_newgeodatabase.Owner = FrameworkApplication.Current.MainWindow;
            _form_environment_newgeodatabase.Closed += (o, e) => { _form_environment_newgeodatabase = null; };

            _form_environment_newgeodatabase.Show();

        }

    }
}
