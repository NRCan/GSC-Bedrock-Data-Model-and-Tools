using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace GSC_ProjectEditor
{
    public class Button_View_CreateMapUnitOverprintLayer : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        #region Main Variables

        //FEATURE GEO_POLYS
        private const string geopoly = GSC_ProjectEditor.Constants.Database.FGeopoly;
        private const string geopolyLabel = GSC_ProjectEditor.Constants.DatabaseFields.FGeopolyLabel;
        private const string geopolyLayerName = GSC_ProjectEditor.Constants.Layers.geopoly;

        //geo_poly feature layer
        
        private const string geopolyFL = GSC_ProjectEditor.Constants.Layers.geopoly;

        //Join Field names
        public string joinGeopolyLabel = geopoly + "." + geopolyLabel;

        //Legend table
        private const string tLegend = GSC_ProjectEditor.Constants.Database.TLegendGene;
        private const string tLegendItemID = GSC_ProjectEditor.Constants.DatabaseFields.LegendLabelID;

        //Database
        private const string FDName = GSC_ProjectEditor.Constants.Database.FDGeo;
        private const string mapUnitDom = GSC_ProjectEditor.Constants.DatabaseDomains.MapUnit;

        #endregion


        public Button_View_CreateMapUnitOverprintLayer()
        {
            //Set culture before init.
            Utilities.Culture.SetCulture();
        }

        protected override void OnClick()
        {
            List<string> restrictedDataset = new List<string>();
            restrictedDataset.Add(Constants.Database.FGeopoly);
            restrictedDataset.Add(Constants.Database.FLabel);

            string currentWorkspacePath = Properties.Settings.Default.PROJECT_DATABASE_PATH;
            IWorkspace currentWorkspace = Workspace.AccessWorkspace(currentWorkspacePath);
            bool isProjectValid = Workspace.isWorkspaceAProperProjectDatabase(currentWorkspace, restrictedDataset);

            if (isProjectValid)
            {
                string interpGroup = GSC_ProjectEditor.Properties.Resources.GroupLayerInterpretation;

                try
                {
                    //Get current layer for map units
                    IFeatureLayer mapUnitFL = Utilities.MapDocumentFeatureLayers.GetFeatureLayerPreProcessing(geopoly, interpGroup);

                    //Layers name
                    string newOverprintLayerName = mapUnitFL.Name + " - " + GSC_ProjectEditor.Constants.Layers.overprintThematic;
                    string newNoOverprintLayerName = mapUnitFL.Name + " - " + GSC_ProjectEditor.Constants.Layers.noOverprintThematic;

                    #region Remove any existing layers

                    Utilities.MapDocumentFeatureLayers.RemoveLayerFromArcMapPreProcessing(newOverprintLayerName);
                    Utilities.MapDocumentFeatureLayers.RemoveLayerFromArcMapPreProcessing(newNoOverprintLayerName);

                    #endregion

                    #region Build proper definition queries

                    //Get a list of all labels from the map unit domain
                    Dictionary<string, string> allMapUnits = GSC_ProjectEditor.Domains.GetDomDico(mapUnitDom, "Description");
                    List<string> overprintsMapUnits = new List<string>();

                    //Get a smaller list of only the overprints labels
                    foreach (KeyValuePair<string, string> kv in allMapUnits)
                    {
                        if (kv.Key.Contains(GSC_ProjectEditor.Constants.ValueKeywords.labelOverprint))
                        {
                            //Add to overprint list
                            overprintsMapUnits.Add(kv.Value);
                        }
                    }

                    //Get a query based on undefined order list
                    IDataset mapUnitFLDataset = mapUnitFL.FeatureClass as IDataset;
                    string buildOverQuery = GSC_ProjectEditor.Queries.BuildQueryFromStringList(geopolyLabel, overprintsMapUnits, "String", "OR", mapUnitFLDataset.Workspace);
                    string buildNoOverQuery = "NOT (" + buildOverQuery + ")";

                    #endregion

                    //Iterate same process for the two new layers
                    if (buildOverQuery != "")
                    {
                        AddNewLayer(mapUnitFL, newOverprintLayerName, buildOverQuery);
                        AddNewLayer(mapUnitFL, newNoOverprintLayerName, buildNoOverQuery);
                    }
                    else
                    {
                        MessageBox.Show(Properties.Resources.Warning_NoOverprintFound, Properties.Resources.Warning_NoOverprintFoundTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    GSC_ProjectEditor.Messages.ShowEndOfProcess();
                }
                catch (Exception Button_View_CreateMapUnitOverprintLayer)
                {

                    MessageBox.Show(Button_View_CreateMapUnitOverprintLayer.StackTrace);
                }
            }

 
        }

        public void AddNewLayer(IFeatureLayer layerToCopyFrom, string newLayerName, string query)
        {
            #region Make a copy of original layer and add it to the proper place in the TOC

            //Create a list of current arc maps objects
            List<object> currentArcObjects = new List<object>();
            currentArcObjects.Add(Utilities.MapDocumentFeatureLayers.GetCurrentMapObject());
            currentArcObjects.Add(Utilities.MapDocumentFeatureLayers.GetCurrentActiveViewObject());
            currentArcObjects.Add(Utilities.MapDocumentFeatureLayers.GetCurrentContentsViewObject());

            //Create a copy of original feature layer
            object copiedFL = GSC_ProjectEditor.ObjectManagement.CopyInputObject(layerToCopyFrom as object);

            //Rename the layer
            IFeatureLayer copiedFeatureLayer = copiedFL as IFeatureLayer;
            copiedFeatureLayer.Name = newLayerName;

            #endregion

            #region Add proper definition query within the new layer

            //Cast the layer definition from the feature layer object and add query
            IFeatureLayerDefinition newFLD = copiedFeatureLayer as IFeatureLayerDefinition;
            newFLD.DefinitionExpression = query;

            #endregion

            #region Remove empty symbols, Update renderer and add new fl in TOC

            //Remove any empty symbols
            GSC_ProjectEditor.Symbols.RemoveEmptySymbols(copiedFeatureLayer as IGeoFeatureLayer, layerToCopyFrom as IGeoFeatureLayer);

            //Add new feature layer to arc map
            GSC_ProjectEditor.FeatureLayers.AddLayerToArcMap(copiedFeatureLayer as ILayer, GSC_ProjectEditor.Properties.Resources.GroupLayerVisualization, currentArcObjects);

            #endregion
        }

        protected override void OnUpdate()
        {
            //Enabled = ArcMap.Application != null;
        }

        /// <summary>
        /// Will enable or disable current control class.
        /// </summary>
        /// <param name="enable"></param>
        public void EnableDisable(bool enable)
        {
            if (enable)
            {
                this.Enabled = true;
            }
            else
            {
                this.Enabled = false;
            }
        }
    }
}
