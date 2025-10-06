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
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Layouts;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.KnowledgeGraph;

namespace BedrockEditorPro.ProWindows
{
    internal class ShowForm_RefreshSymbols : Button
    {

        private Form_RefreshSymbols _form_refreshsymbols = null;

        protected override void OnClick()
        {
            //already open?
            if (_form_refreshsymbols != null)
                return;
            _form_refreshsymbols = new Form_RefreshSymbols();
            _form_refreshsymbols.Owner = FrameworkApplication.Current.MainWindow;
            _form_refreshsymbols.Closed += (o, e) => { _form_refreshsymbols = null; };
            _form_refreshsymbols.Show();
             //uncomment for modal
             //_form_refreshsymbols.ShowDialog();
        }

    }
}
