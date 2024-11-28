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
    public class Button_View_CreateThematicLayers : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public Button_View_CreateThematicLayers()
        {
            //Set culture before init.
            Utilities.Culture.SetCulture();
        }

        /// <summary>
        /// Will take selected layer from TOC, and create a new selection layer from all subtypes values.
        /// </summary>
        protected override void OnClick()
        {
            List<string> restrictedDataset = new List<string>();
            restrictedDataset.Add(Constants.Database.FGeopoint);
            restrictedDataset.Add(Constants.Database.FGeoline);

            string currentWorkspacePath = Properties.Settings.Default.PROJECT_DATABASE_PATH;
            IWorkspace currentWorkspace = Workspace.AccessWorkspace(currentWorkspacePath);
            bool isProjectValid = Workspace.isWorkspaceAProperProjectDatabase(currentWorkspace, restrictedDataset);

            if (isProjectValid)
            {
                //Pop a form for user to select which layer to create sub layer based on a theme from subtypes values
                Form_View_CreateThematicLayers getNewForm = new Form_View_CreateThematicLayers();
                getNewForm.Show();

                //Get any new event coming from the form ok button
                getNewForm.createButtonPushed += new Form_View_CreateThematicLayers.createButtonEventHandler(getNewForm_createButtonPushed);
            }

        }

        void getNewForm_createButtonPushed(object sender, EventArgs e)
        {

            //Cast sender as the proper control to retrieve user choice
            CheckedListBox formCheckList = sender as CheckedListBox;

            List<object> arcMapObjectList = GetArcMapObjects();

            //Start the chain of events depending on user choice
            if (formCheckList.GetItemCheckState(0) == CheckState.Checked) //For geopoints
            {
                //Get geopoint feature layer from table of content
                string geopointFC = GSC_ProjectEditor.Constants.Database.FGeopoint;
                string interGL = GSC_ProjectEditor.Properties.Resources.GroupLayerInterpretation;
                IFeatureLayer geopointFL = GSC_ProjectEditor.FeatureLayers.GetFeatureLayer(geopointFC, interGL, arcMapObjectList[0] as IMap);

                //Create themes
                CreateTheme(geopointFL);
            }

            if (formCheckList.GetItemCheckState(1) == CheckState.Checked) //For geolines
            {
                //Get geoline feature layer from table of content
                string geolineFC = GSC_ProjectEditor.Constants.Database.FGeoline;
                string interGL = GSC_ProjectEditor.Properties.Resources.GroupLayerInterpretation;
                IFeatureLayer geolineFL = GSC_ProjectEditor.FeatureLayers.GetFeatureLayer(geolineFC, interGL, arcMapObjectList[0] as IMap);

                //Create themes
                CreateTheme(geolineFL);
            }

            GSC_ProjectEditor.Messages.ShowEndOfProcess();

            
        }

        protected override void OnUpdate()
        {
            //Enabled = ArcMap.Application != null;
        }

        /// <summary>
        /// From a given feature layer will create a new layer added into VISUALIZATION group layer based on input feature layer subtype values
        /// </summary>
        /// <param name="layerToCreateThemeFrom">Add wanted feature layer to get subtype layers from</param>
        public void CreateTheme(IFeatureLayer layerToCreateThemeFrom)
        {

            try
            {

                //Get basic information from feature layer
                IFeatureLayer currentFL = layerToCreateThemeFrom;

                //Get current subtype list from it.
                Dictionary<string, int> subDico = GSC_ProjectEditor.Subtypes.GetSubtypeDicoFromLayer(currentFL);

                if (subDico.Count != 0)
                {
                    //Iterate through all subtypes to create new layers
                    foreach (KeyValuePair<string, int> subKV in subDico)
                    {

                        #region Build proper definition query
                        //Prepare a list of value to create selection from
                        List<string> valueList = new List<string>();
                        valueList.Add(subKV.Value.ToString());

                        //Create a new layer name
                        string newLayerName = currentFL.Name + " - " + subKV.Key;

                        //Remove layer from TOC if exists
                        Utilities.MapDocumentFeatureLayers.RemoveLayerFromArcMapPreProcessing(newLayerName);

                        //Get current subtype field name
                        string subFieldName = GSC_ProjectEditor.Subtypes.GetSubtypeFieldFromFeatureLayer(currentFL);

                        //Get a query based on undefined order list
                        IDataset currentFLDataset = currentFL.FeatureClass as IDataset;
                        string buildQuery = GSC_ProjectEditor.Queries.BuildQueryFromStringList(subFieldName, new List<string> { subKV.Value.ToString() }, "Int", "OR", currentFLDataset.Workspace);

                        #endregion

                        #region Make a copy of original layer and add it to the proper place in the TOC

                        //Create a copy of original feature layer
                        object copiedFL = GSC_ProjectEditor.ObjectManagement.CopyInputObject(currentFL as object);

                        //Rename the layer
                        IFeatureLayer copiedFeatureLayer = copiedFL as IFeatureLayer;
                        copiedFeatureLayer.Name = newLayerName;


                        #endregion

                        #region Add proper definition query within the new layer

                        //Cast the layer definition from the feature layer object
                        IFeatureLayerDefinition newFLD = copiedFeatureLayer as IFeatureLayerDefinition;

                        //Add the query
                        newFLD.DefinitionExpression = buildQuery;

                        #endregion

                        #region Remove empty symbols, Update renderer and add new fl in TOC

                        List<object> currentArcObjects = GetArcMapObjects();

                        //Remove any empty symbols
                        GSC_ProjectEditor.Symbols.RemoveEmptySymbols(copiedFeatureLayer as IGeoFeatureLayer, currentFL as IGeoFeatureLayer);

                        //Add new feature layer to arc map
                        GSC_ProjectEditor.FeatureLayers.AddLayerToArcMap(copiedFeatureLayer as ILayer, GSC_ProjectEditor.Properties.Resources.GroupLayerVisualization, currentArcObjects);

                        #endregion

                    }
                }
                else
                {
                    //Manage no subtypes layer
                    MessageBox.Show(GSC_ProjectEditor.Properties.Resources.Error_NoSubtypes);
                }

            }
            catch (Exception Button_View_CreateThematicLayers)
            {

                MessageBox.Show(Button_View_CreateThematicLayers.StackTrace);
            }
        }

        //Will return a list of objects for arc map documents
        public List<object> GetArcMapObjects()
        {
            //Create a list of current arc maps objects
            List<object> currentArcObjects = new List<object>();
            currentArcObjects.Add(Utilities.MapDocumentFeatureLayers.GetCurrentMapObject());
            currentArcObjects.Add(Utilities.MapDocumentFeatureLayers.GetCurrentActiveViewObject());
            currentArcObjects.Add(Utilities.MapDocumentFeatureLayers.GetCurrentContentsViewObject());

            return currentArcObjects;
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
