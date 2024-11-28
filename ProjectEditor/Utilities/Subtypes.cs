using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;

namespace GSC_ProjectEditor
{
    public class Subtypes
    {
        #region GET METHODS

        /// <summary>
        /// Will return a dictionnary containing subtype names and codes (key=name, value=code), for project database
        /// </summary>
        /// <param name="inputFeatureClass">Reference to a feature class name to get subtype dictionnary from</param>
        /// <returns></returns>
        public static SortedDictionary<string, int> GetSubtypeDico(string inputFeatureClass)
        {
            //Main output
            SortedDictionary<string, int> subDico = new SortedDictionary<string, int>();
            Dictionary<string, string> tempSubDico = GetSubtypeDicoByKey(inputFeatureClass, true);

            foreach (KeyValuePair<string, string> item in tempSubDico)
            {
                subDico[item.Key] = Convert.ToInt16(item.Value);
            }

            return subDico;
        }

        /// <summary>
        /// Will return a dictionnary containing subtype names and codes (key=name, value=code), for project database
        /// </summary>
        /// <param name="inputFeatureClass">Reference to a feature class name to get subtype dictionnary from</param>
        /// <param name="keyAsName">True if dict. key needs to be subtype description</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetSubtypeDicoByKey(string inputFeatureClass, bool keyAsName)
        {
            //Main output
            Dictionary<string, string> subDico = new Dictionary<string, string>();

            //Access subtype
            ISubtypes getSub = AccessSubtypes(inputFeatureClass);

            //Get list of subtypes
            IEnumSubtype subList = getSub.Subtypes;

            //Iterate through list
            int subCode = new int(); //Gives subtype code
            string subName = subList.Next(out subCode); //Gives subtype name

            while (subName != "" && subCode != 0)
            {
                if (keyAsName)
                {
                    subDico[subName] = subCode.ToString(); 
                }
                else
                {
                    subDico[subCode.ToString()] = subName;
                }
                subName = subList.Next(out subCode);

            }

            return subDico;
        }

        /// <summary>
        /// Will return a dictionnary containing subtype names and codes (key=name, value=code), for project database
        /// </summary>
        /// <param name="inputFeatureClass">Reference to a feature class name to get subtype dictionnary from</param>
        /// <param name="keyAsName">True if dict. key needs to be subtype description</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetSubtypeDicoByKeyFromWorkspace(IWorkspace inWork, string inputFeatureClass, bool keyAsName)
        {
            //Main output
            Dictionary<string, string> subDico = new Dictionary<string, string>();

            //Access subtype
            ISubtypes getSub = AccessSubtypesFromWorkspace(inWork, inputFeatureClass);

            //Get list of subtypes
            IEnumSubtype subList = getSub.Subtypes;

            //Iterate through list
            int subCode = new int(); //Gives subtype code
            string subName = subList.Next(out subCode); //Gives subtype name

            while (subName != null)
            {
                if (keyAsName)
                {
                    subDico[subName] = subCode.ToString();
                }
                else
                {
                    subDico[subCode.ToString()] = subName;
                }
                subName = subList.Next(out subCode);

            }

            return subDico;
        }

        /// <summary>
        /// Will return a dictionnary containing subtype names and codes (key=name, value=code), from a given feature layer (could be outside project DB)
        /// </summary>
        /// <param name="inputFL">The input feature layer to retrieve subtypes from</param>
        /// <returns></returns>
        public static Dictionary<string, int> GetSubtypeDicoFromLayer(IFeatureLayer inputFL)
        {
            //Main output
            Dictionary<string, int> subDico = new Dictionary<string, int>();

            //Get current workspace
            IFeatureClass inputFC = inputFL.FeatureClass;

            //Access subtype
            ISubtypes getSub = inputFC as ISubtypes;
            //Get list of subtypes
            IEnumSubtype subList = getSub.Subtypes;

            //Iterate through list
            int subCode = new int(); //Gives subtype code
            string subName = subList.Next(out subCode); //Gives subtype name

            while (subName != "" && subCode != 0)
            {

                subDico[subName] = subCode;
                subName = subList.Next(out subCode);

            }

            return subDico;
        }

        /// <summary>
        /// Will return a feature layer associated subtype field name.
        /// </summary>
        /// <param name="inputFL">The input feature layer to get subtype field name from.</param>
        /// <returns></returns>
        public static string GetSubtypeFieldFromFeatureLayer(IFeatureLayer inputFL)
        {
            //Cast current layer to feature class
            IFeatureClass inputFC = inputFL.FeatureClass;

            //Access sutbypes
            ISubtypes inputSub = inputFC as ISubtypes;

            return inputSub.SubtypeFieldName;

        }

        /// <summary>
        /// Will return a default field value, for a given subtype, inside a given field and feature class
        /// </summary>
        /// <param name="inFeatureClass">The feature class to search for default.</param>
        /// <param name="fieldName">The field name to get the default from</param>
        /// <param name="inSubCode">The subtype code to get the default value from</param>
        /// <returns></returns>
        public static string GetSubtypeFieldDefault(string inFeatureClass, string fieldName, int inSubCode)
        {
            //Variables
            string defaultValue = "";

            //Get subtype from given feature class name
            ISubtypes inSub = AccessSubtypes(inFeatureClass);

            //Get default value from given field name
            defaultValue = inSub.get_DefaultValue(inSubCode, fieldName).ToString();

            return defaultValue;

        }

        #endregion

        #region ACCESS METHODS

        /// <summary>
        /// Will access subtypes from a given layer, for project database
        /// </summary>
        /// <param name="inputFeatureForSub">Reference to a feature class name to get subtypes from</param>
        /// <returns></returns>
        public static ISubtypes AccessSubtypes(string inputFeatureForSub)
        {
            //Main output
            ISubtypes fcSub = null;

            //Get current workspace
            string getDatabasePath = GSC_ProjectEditor.Workspace.GetDBPath();
            IWorkspace getWorkspace = GSC_ProjectEditor.Workspace.AccessWorkspace(getDatabasePath);

            return AccessSubtypesFromWorkspace(getWorkspace, inputFeatureForSub);
        }

        /// <summary>
        /// Will access subtypes from a given layer, for project database
        /// </summary>
        /// <param name="inputFeatureForSub">Reference to a feature class name to get subtypes from</param>
        /// <returns></returns>
        public static ISubtypes AccessSubtypesFromWorkspace(IWorkspace inWork, string inputFeatureForSub)
        {
            //Main output
            ISubtypes fcSub = null;

            //Get wanted feature class object
            IFeatureClass wantedFC = GSC_ProjectEditor.FeatureClass.OpenFeatureClassFromWorkspace(inWork, inputFeatureForSub);

            //Get wanted subtype object from fc
            fcSub = wantedFC as ISubtypes;

            return fcSub;
        }

        #endregion

        #region DEL METHODS

        /// <summary>
        /// Will delete subtypes from a given layer, for project database
        /// </summary>
        /// <param name="inputFeatureForSub">Reference to a feature class name to get subtypes from</param>
        /// <returns></returns>
        public static void DeleteSubtypes(string inFC)
        {
            //Get current workspace
            string getDatabasePath = GSC_ProjectEditor.Workspace.GetDBPath();
            IWorkspace getWorkspace = GSC_ProjectEditor.Workspace.AccessWorkspace(getDatabasePath);

            DeleteSubtypesFromWorkspace(getWorkspace, inFC);

        }

        /// <summary>
        /// Will delete subtypes from a given layer, for project database
        /// </summary>
        /// <param name="inputFeatureForSub">Reference to a feature class name to get subtypes from</param>
        /// <returns></returns>
        public static void DeleteSubtypesFromWorkspace(IWorkspace inWork, string inFC)
        {

            //Get wanted feature class object
            IFeatureClass wantedFC = GSC_ProjectEditor.FeatureClass.OpenFeatureClassFromWorkspace(inWork, inFC);

            //Get wanted subtype object from fc
            ISubtypes fcSub = wantedFC as ISubtypes;
            fcSub.SubtypeFieldName = null;

        }

        #endregion

    }
}
