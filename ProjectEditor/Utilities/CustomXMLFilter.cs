using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Catalog;

namespace GSC_ProjectEditor.Utilities
{
    [Guid("26396405-80e2-442e-bdc0-6f58be362f64")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("ProjectManager.Utilities.CustomXMLFilter")]
    public class CustomXMLFilter : ESRI.ArcGIS.Catalog.IGxObjectFilter
    {
        //#region COM Registration Function(s)
        //[ComRegisterFunction()]
        //[ComVisible(false)]
        //static void RegisterFunction(Type registerType)
        //{
        //    // Required for ArcGIS Component Category Registrar support
        //    ArcGISCategoryRegistration(registerType);

        //    //
        //    // TODO: Add any COM registration code here
        //    //
        //}

        //[ComUnregisterFunction()]
        //[ComVisible(false)]
        //static void UnregisterFunction(Type registerType)
        //{
        //    // Required for ArcGIS Component Category Registrar support
        //    ArcGISCategoryUnregistration(registerType);

        //    //
        //    // TODO: Add any COM unregistration code here
        //    //
        //}

        //#region ArcGIS Component Category Registrar generated code
        ///// <summary>
        ///// Required method for ArcGIS Component Category registration -
        ///// Do not modify the contents of this method with the code editor.
        ///// </summary>
        //private static void ArcGISCategoryRegistration(Type registerType)
        //{
        //    string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
        //    GxObjectFilters.Register(regKey);

        //}
        ///// <summary>
        ///// Required method for ArcGIS Component Category unregistration -
        ///// Do not modify the contents of this method with the code editor.
        ///// </summary>
        //private static void ArcGISCategoryUnregistration(Type registerType)
        //{
        //    string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
        //    GxObjectFilters.Unregister(regKey);

        //}

        //#endregion
        //#endregion

        #region Member Variables
        private IGxObjectFilter m_pBasicFilter;
        #endregion

        public CustomXMLFilter()
        {
            m_pBasicFilter = new GxFilterBasicTypesClass();
        }

        #region "IGxObjectFilter Implementations"
        public bool CanChooseObject(ESRI.ArcGIS.Catalog.IGxObject @object, ref ESRI.ArcGIS.Catalog.esriDoubleClickResult result)
        {
            //Set whether the selected object can be chosen
            bool bCanChoose = false;
            bCanChoose = false;
            if (@object is IGxFile)
            {
                string sExt = null;
                sExt = GetExtension(@object.Name);
                if (sExt.ToLower() == ".xml")
                    bCanChoose = true;
            }
            return bCanChoose;
        }

        public bool CanDisplayObject(ESRI.ArcGIS.Catalog.IGxObject @object)
        {
            //Check objects can be displayed
            try
            {
                //Check objects can be displayed
                if (m_pBasicFilter.CanDisplayObject(@object))
                    return true;
                else if (@object is IGxFile)
                {
                    string sExt = null;
                    sExt = GetExtension(@object.Name);
                    if (sExt.ToLower() == ".xml")
                        return true;
                }

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
            return false;
        }

        public bool CanSaveObject(ESRI.ArcGIS.Catalog.IGxObject Location, string newObjectName, ref bool objectAlreadyExists)
        {
            return false;
        }

        public string Description
        {
            get
            {
                //Set filter description
                return "XML file (.xml)";
            }
        }

        public string Name
        {
            get
            {
                //Set filter name
                return "XML filter";
            }
        }
        #endregion

        private string GetExtension(string sFileName)
        {
            string tempGetExtension = null;
            //Get extension
            long pExtPos = 0;
            pExtPos = (sFileName.LastIndexOf(".") + 1);
            if (pExtPos > 0)
                tempGetExtension = sFileName.Substring((Int32)pExtPos - 1);
            else
                tempGetExtension = "";
            return tempGetExtension;
        }

    }
}
