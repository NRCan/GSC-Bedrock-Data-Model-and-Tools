using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ESRI.ArcGIS.Geodatabase;

namespace GSC_ProjectEditor
{
    public class Environments
    {
        #region GET Methods

        /// <summary>
        /// Will return the environment database path
        /// </summary>
        /// <returns></returns>
        public static string GetEnvDBPath()
        {
            //Variables
            string relativeEnvPath = WorkingEnvironmentFolderPath();
            string envName = GSC_ProjectEditor.Constants.Environment.containerName;
            string envDBPath = relativeEnvPath + "\\" + envName + ".gdb";
            return envDBPath;

        }

        /// <summary>
        /// Will return a list of values from a given field, within the working environment database
        /// </summary>
        /// <param name="field">The field to retrieve values from</param>
        /// <returns></returns>
        public static List<string> GetEnvironmentTableFieldValue(string field, bool onlyActiv)
        {

            //Get some variables
            //string tool4ProjectPathField = GSC_ProjectEditor.Constants.Environment.envTools4ProjectPath;
            string activeField = GSC_ProjectEditor.Constants.Environment.envActive;
            string activeFieldValueTrue = GSC_ProjectEditor.Constants.FieldValues.envActiveTrue.ToString();
            //string getTool4ProjectPath = GSC_ProjectEditor.Properties.Settings.Default.Tools4ProjectFolder;
            string getEnvTableName = GSC_ProjectEditor.Constants.Environment.envTable;

            //Build query to select current tool4project path
            //string pathQuery = tool4ProjectPathField + " = '" + getTool4ProjectPath + "'";
            string pathQuery = activeField + " <> 99";
            if (onlyActiv)
            {
                pathQuery = activeField + " = " + activeFieldValueTrue;
            }
            

            //Get the environment workspace object
            string envWorkspacePath = GetEnvDBPath();

            IWorkspace envWorkspace = GSC_ProjectEditor.Workspace.AccessWorkspace(envWorkspacePath);

            //Get if a row already exists within table
            return GSC_ProjectEditor.Tables.GetFieldValuesFromWorkspace(envWorkspace, getEnvTableName, field, pathQuery);

        }

        /// <summary>
        /// Will calculate row count from env. table
        /// </summary>
        /// <returns></returns>
        public static int GetRowCount(string queryFilter)
        {
            //Variables
            string envTable = Constants.Environment.envTable;

            //Get env working database path
            string getEnvDBPath = GetEnvDBPath();

            //Get workspace from this database
            IWorkspace environmentWorkspace = Workspace.AccessWorkspace(getEnvDBPath);

            return Tables.GetRowCountFromWorkspace(environmentWorkspace, envTable, queryFilter);
        }

        #endregion

        #region OTHER methods

        /// <summary>
        /// Getting a correct path to the working environment database
        /// </summary>
        /// <returns></returns>
        public static string WorkingEnvironmentFolderPath()
        {
            //Variable
            string arcGISFolder = Properties.Settings.Default.PROJECT_WORKSPACE_PATH;
            if (arcGISFolder == string.Empty || arcGISFolder == null)
            {
                arcGISFolder = System.IO.Path.Combine(Constants.ESRI.defaultArcGISFolderName, Constants.Namespaces.mainNamespace + " " + ThisAddIn.Version.ToString());
            }

            return System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + arcGISFolder;
        }


        #endregion

    }
}
