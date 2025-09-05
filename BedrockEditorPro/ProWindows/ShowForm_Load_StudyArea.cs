using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.KnowledgeGraph;
using ArcGIS.Desktop.Layouts;
using ArcGIS.Desktop.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BedrockEditorPro.ProWindows
{
    internal class ShowForm_Load_StudyArea : Button
    {

        private Form_Load_StudyArea _form_load_studyarea = null;

        protected override void OnClick()
        {
            //already open?
            if (_form_load_studyarea != null)
                return;
            _form_load_studyarea = new Form_Load_StudyArea();
            _form_load_studyarea.Owner = FrameworkApplication.Current.MainWindow;
            _form_load_studyarea.Closed += (o, e) => { _form_load_studyarea = null; };
            _form_load_studyarea.Show();
            //uncomment for modal
            //_form_load_studyarea.ShowDialog();
        }

    }
}
