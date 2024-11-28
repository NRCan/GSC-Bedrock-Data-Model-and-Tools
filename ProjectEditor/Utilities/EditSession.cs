using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.esriSystem;

namespace GSC_ProjectEditor.Utilities
{
    class EditSession
    {
        //Build a editor object, after detecting current editor ID
        //
        public static IEditor GetEditor()
        {
            UID editorUID = new UIDClass();
            editorUID.Value = "esriEditor.Editor";
            IEditor gsc_editor = GSC_ProjectEditor.ArcMap.Application.FindExtensionByCLSID(editorUID) as IEditor;

            return gsc_editor;
        }

        //Build a editor object 3, after detecting current editor ID
        //
        public static IEditor3 GetEditor3()
        {
            UID editorUID = new UIDClass();
            editorUID.Value = "esriEditor.Editor";
            IEditor3 gsc_editor = GSC_ProjectEditor.ArcMap.Application.FindExtensionByCLSID(editorUID) as IEditor3;

            return gsc_editor;
        }


    }
}
