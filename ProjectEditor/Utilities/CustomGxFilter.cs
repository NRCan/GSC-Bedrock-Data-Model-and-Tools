using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Catalog;

namespace GSC_ProjectEditor
{
    [Guid("53d66cb2-3703-4937-a146-f80f806fcafe")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("ProjectManager.CustomGxFilter")]
    public class CustomGxFilter : ESRI.ArcGIS.Catalog.IGxObjectFilter
    {
        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            GxObjectFilters.Register(regKey);

        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            GxObjectFilters.Unregister(regKey);

        }

        #endregion
        #endregion

        #region "IGxObjectFilter Implementations"

        private IGxObjectFilter m_pBasicFilter;

        public bool CanChooseObject(ESRI.ArcGIS.Catalog.IGxObject obj, ref ESRI.ArcGIS.Catalog.esriDoubleClickResult result)
        {
            //variables
            bool canChoose = false;

            //Set choosing option of wanted extension only
            if (obj is IGxFile)
            {
                string ext = null;
                ext = GetExtension(obj.Name);
                if (ext.ToLower() == ".pdf" | ext.ToLower() == ".doc" | ext.ToLower() == ".docx")
                {
                    canChoose = true;
                }

            }

            return canChoose;
        }

        public bool CanDisplayObject(ESRI.ArcGIS.Catalog.IGxObject obj)
        {
            //Variables
            bool canDisplay = false;

            if (obj!= null)
            {
                if (m_pBasicFilter.CanDisplayObject(obj))
	            {
                    canDisplay = true;
	            }
                
            }
            else if (obj is IGxFile)
            {
                string ext = null;
                ext = GetExtension(obj.Name);
                if (ext!=null)
                {
                    if (ext.ToLower() == ".pdf" | ext.ToLower() == ".doc" | ext.ToLower() == ".docx")
                    {
                        canDisplay = true;
                    }
                }

            }

            return canDisplay;
        }

        public bool CanSaveObject(ESRI.ArcGIS.Catalog.IGxObject Location, string newObjectName, ref bool objectAlreadyExists)
        {
            return false;
        }

        public string Description
        {
            get
            {
                return "Browses for .pdf, .doc, .docx";
            }
        }

        public string Name
        {
            get
            {
                return "Custom Filter";
            }
        }
        #endregion

        /// <summary>
        /// Will return current file extension from the file name
        /// </summary>
        /// <param name="fileName">The wanted file name to get extension from</param>
        /// <returns></returns>
        public string GetExtension(string fileName)
        {
            string tempGetExtension = null;
            int extPos = 0;
            extPos = (fileName.LastIndexOf(".") + 1);
            if (extPos > 0)
            {
                tempGetExtension = fileName.Substring(extPos - 1);
            }
            else
            {
                tempGetExtension = "";
            }

            return tempGetExtension;
        }


    }
}
