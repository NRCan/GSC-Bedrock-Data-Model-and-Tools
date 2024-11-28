using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Editor;

namespace GSC_ProjectEditor.Utilities
{
    class MapDocumentFeatureLayers
    {
        /// <summary>
        /// Changes datasource within a layer within Arc Map Table of Content. 
        /// Needs to get some protected info before refering to another method
        /// </summary>
        /// <param name="featureClassName">Feature class name to set datasource of</param>
        /// <param name="featureDatasetName">Feature dataset that contains feature class to set datasoure from</param>
        /// <param name="layerToResetSource">The laye object that reference the feature class to set datasource from</param>
        public static void SetDataSourcePreProcessing(string featureClassName, string featureDatasetName, ILayer layerToResetSource)
        {

            //Build list of arc map addin only objects.
            List<object> arcMapObjects = new List<object>();
            arcMapObjects.Add(GetCurrentMapObject());
            arcMapObjects.Add(GetCurrentActiveViewObject());
            arcMapObjects.Add(GetCurrentContentsViewObject());

            //Send object to method in main repo.
            GSC_ProjectEditor.FeatureLayers.SetDataSource(featureClassName, featureDatasetName, layerToResetSource, arcMapObjects);


        }

        /// <summary>
        /// Return current focused map
        /// </summary>
        /// <returns></returns>
        public static IMap GetCurrentMapObject()
        {
            return ArcMap.Document.FocusMap;
        }

        /// <summary>
        /// Return current active view
        /// </summary>
        /// <returns></returns>
        public static IActiveView GetCurrentActiveViewObject()
        {
            return ArcMap.Document.ActiveView;
        }

        /// <summary>
        /// Return current content view
        /// </summary>
        /// <returns></returns>
        public static IContentsView GetCurrentContentsViewObject()
        {
            return ArcMap.Document.CurrentContentsView;
        }

        /// <summary>
        /// Retrieves a group layer objects
        /// </summary>
        /// <param name="inputGroupName">input wanted group name</param>
        /// <returns></returns>
        public static IGroupLayer GetGroupLayerPreProcessing(string inputGroupName)
        {
            //Call main repo method
            return GSC_ProjectEditor.FeatureLayers.GetGroupLayer(inputGroupName, GetCurrentMapObject());

        }

        /// <summary>
        /// Adds a new layer to Arc map
        /// </summary>
        /// <param name="inputLayer">Reference to wanted layer object to add</param>
        /// <param name="inputTargetGroupLayer">If required, specifie a target group layer to add layer to</param>
        public static void AddLayerToArcMapPreProcessing(ILayer inputLayer, string inputTargetGroupLayer)
        {
            //Build list of arc map addin only objects.
            List<object> arcMapObjects = new List<object>();
            arcMapObjects.Add(GetCurrentMapObject());
            arcMapObjects.Add(GetCurrentActiveViewObject());
            arcMapObjects.Add(GetCurrentContentsViewObject());

            //Send object to method in main repo.
            GSC_ProjectEditor.FeatureLayers.AddLayerToArcMap(inputLayer, inputTargetGroupLayer, arcMapObjects);
        }

        /// <summary>
        /// Remove a layer from Arc Map
        /// </summary>
        /// <param name="inputLayerName">Wanted layer name to remove</param>
        public static void RemoveLayerFromArcMapPreProcessing(string inputLayerName)
        {
            //Call main repo method.
            GSC_ProjectEditor.FeatureLayers.RemoveLayerFromArcMap(inputLayerName, GetCurrentMapObject());

        }

        /// <summary>
        /// Get a feature layer object within Arc Map Table of Content
        /// </summary>
        /// <param name="groupLayerName">Feature layer name to get object from</param>
        /// <returns></returns>
        public static IFeatureLayer GetFeatureLayerPreProcessing(string featureClassName, string groupLayerName)
        {
            //Call main repo method
            return GSC_ProjectEditor.FeatureLayers.GetFeatureLayer(featureClassName, groupLayerName, GetCurrentMapObject());

        }

        /// <summary>
        /// Will return a list of features from a wanted feature class name and layer name
        /// </summary>
        /// <param name="featureClassName">The feature class to retrieve the selected objects from</param>
        /// <param name="featureLayerName">The feaure layer name associated with it's class to find the right one if multiple exists</param>
        /// <returns></returns>
        public static List<IFeature> GetListOfAllSelectedFeatures()
        {
            //Variables
            List<IFeature> selectedFeatureList = new List<IFeature>();

            //Get current editor
            IEditor currentEditor = Utilities.EditSession.GetEditor();
            
            //Get current selected feature
            IEnumFeature selectedFeatures = currentEditor.EditSelection;
            if (currentEditor.SelectionCount > 0)
            {
                IFeature selectedFeature = selectedFeatures.Next();
                while (selectedFeature != null)
                {
                    selectedFeatureList.Add(selectedFeature);
                    selectedFeature = selectedFeatures.Next();
                }
            }


            return selectedFeatureList;
        }

    }

}
