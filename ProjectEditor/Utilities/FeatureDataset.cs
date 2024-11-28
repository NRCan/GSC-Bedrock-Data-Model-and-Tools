using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace GSC_ProjectEditor
{
    public class FeatureDataset
    {
        /// <summary>
        /// To open a feature dataset
        /// </summary>
        /// <param name="inputWorkspace">Reference to a workspace object</param>
        /// <param name="inputFeature">Reference feature dataset name to open</param>
        /// <returns></returns>
        public static IFeatureDataset OpenFeatureDataSet(IWorkspace inputWorkspace, string inputFDName)
        {
            //Access workspace
            IFeatureWorkspace getWorkspace = (IFeatureWorkspace)inputWorkspace;

            //Return object
            return getWorkspace.OpenFeatureDataset(inputFDName);
        }

        /// <summary>
        /// Will return a list of all feature datasets inside a given workspace
        /// </summary>
        /// <param name="inputWorkspace">The input workspace to retrieve FC list from</param>
        /// <param name="inputFeatureDataset">Input feature dataset, if needed, to refine search, or else rooted fc will be returned.</param>
        public static List<IFeatureDataset> GetFeatureDatasetList(IWorkspace inputWorkspace)
        {
            //Variables
            List<IFeatureDataset> featureList = new List<IFeatureDataset>();
            IEnumDataset fcEnum;

            //Get a list of all feature classes from workspace
            fcEnum = inputWorkspace.get_Datasets(esriDatasetType.esriDTFeatureDataset);

            //Convert enum to list
            IDataset currentDS = fcEnum.Next();
            while (currentDS != null)
            {
                featureList.Add(currentDS as IFeatureDataset);

                currentDS = fcEnum.Next();
            }

            return featureList;
        }

    }
}
