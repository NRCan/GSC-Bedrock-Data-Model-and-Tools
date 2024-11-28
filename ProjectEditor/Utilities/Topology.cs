using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace GSC_ProjectEditor
{
    public class Topology
    {

        #region CREATE METHODS

        /// <summary>
        /// Will create a topological layer within GEO feature dataset within project database
        /// </summary>
        /// <returns></returns>
        public static ITopology CreateTopoLayerInProjectDatabase()
        {
            //Get workspace object
            string inputWorkspacePath = GSC_ProjectEditor.Workspace.GetDBPath();//Utilities.globalVariables.database.path;
            IWorkspace inputWorkspace = GSC_ProjectEditor.Workspace.AccessWorkspace(inputWorkspacePath);

            //Get wanted feature dataset to add topology layer to
            IFeatureDataset inputGeoDataset = GSC_ProjectEditor.FeatureDataset.OpenFeatureDataSet(inputWorkspace, GSC_ProjectEditor.Constants.Database.FDGeo);

            //Create topo layer 
            ITopology newTopo = CreateTopoLayerFromWorkspace(inputWorkspace, inputGeoDataset, false, GSC_ProjectEditor.Constants.Database.Topology);

            return newTopo;

        }

        /// <summary>
        /// Will create a topological layer within a given feature dataset within a given database (workspace
        /// </summary>
        /// <returns></returns>
        public static ITopology CreateTopoLayerFromWorkspace(IWorkspace inputWorkspace, IFeatureDataset inputFD, bool zCluster, string topoLayerName)
        {
            //Create a topo container from feature dataset
            ITopologyContainer2 topoContainer = inputFD as ITopologyContainer2;

            //Declare topology
            ITopology topo = null;

            //Make sure it doesn't already exists
            IWorkspace2 inWork2 = inputWorkspace as IWorkspace2;
            if (!inWork2.get_NameExists(esriDatasetType.esriDTTopology, topoLayerName))
            {

                //Get exclusive schema lock
                ISchemaLock schLock = inputFD as ISchemaLock;

                try
                {


                    //Create the layer
                    if (zCluster)
                    {
                        //Extented topology
                        topo = topoContainer.CreateTopologyEx(topoLayerName, topoContainer.DefaultClusterTolerance, topoContainer.DefaultZClusterTolerance, -1, "");

                    }
                    else
                    {
                        //Default normal topology
                        topo = topoContainer.CreateTopology(topoLayerName, topoContainer.DefaultClusterTolerance, -1, "");
                    }

                }
                catch (Exception schemaLockExcept)
                {
                    MessageBox.Show(schemaLockExcept.Message);
                }
                finally
                {
                    //Release lock
                    schLock.ChangeSchemaLock(esriSchemaLock.esriSharedSchemaLock);
                }
            }

            return topo;

        }

        #endregion

        #region ADD, VALIDATE METHODS

        /// <summary>
        /// Will add a tuple(rules, rule name) of rules into a given topology
        /// </summary>
        public static void AddRulesToTopology(List<Tuple<esriTopologyRuleType, string>> ruleList, ITopology inputTopology, IFeatureClass originFeat, IFeatureClass destinationFeat, bool subtypes)
        {
            try
            {
                //Create a topo rule container for all rules
                ITopologyRuleContainer topoRuleContainer = inputTopology as ITopologyRuleContainer;

                //Add rules to it
                foreach (Tuple<esriTopologyRuleType, string> tuple in ruleList)
                {
                    //Create a new topo rule object
                    ITopologyRule topoRule = new TopologyRuleClass();
                    topoRule.TopologyRuleType = tuple.Item1;
                    topoRule.Name = tuple.Item2;
                    topoRule.OriginClassID = originFeat.FeatureClassID;
                    topoRule.DestinationClassID = destinationFeat.FeatureClassID;
                    topoRule.AllDestinationSubtypes = subtypes;
                    topoRule.AllOriginSubtypes = subtypes;

                    //Add to container
                    if (topoRuleContainer.get_CanAddRule(topoRule))
                    {
                        topoRuleContainer.AddRule(topoRule);
                    }

                }
            }
            catch (Exception addRulesToTopologyExcept)
            {

                MessageBox.Show(addRulesToTopologyExcept.StackTrace);
            }

        }

        /// <summary>
        /// Will validate entire area of input topology layer
        /// </summary>
        /// <param name="inputTopo"></param>
        public static void ValidatTopology(ITopology inputTopo, IEnvelope inputFeatureClassExtent)
        {
            //Get a dirty area
            IPolygon locationPoly = new PolygonClass();
            ISegmentCollection segmentCollection = locationPoly as ISegmentCollection;
            segmentCollection.SetRectangle(inputFeatureClassExtent);
            IPolygon dirtyPoly = inputTopo.get_DirtyArea(locationPoly);

            //Validate topology
            if (!dirtyPoly.IsEmpty)
            {
                //Cast area to validate, which is full feature extent.
                IEnvelope areaToValidate = dirtyPoly.Envelope;
                IEnvelope areaValidated = inputTopo.ValidateTopology(areaToValidate);
            }
        }

        #endregion

        #region OPEN METHODS

        /// <summary>
        /// Will return a given topology object from a string of topo feature name, for PROJECT database only
        /// </summary>
        /// <param name="topoName"></param>
        /// <returns></returns>
        public static ITopology OpenTopoLayerLayer(string topoName)
        {
            //Get workspace object
            string inputWorkspacePath = GSC_ProjectEditor.Workspace.GetDBPath();//Utilities.globalVariables.database.path;
            IWorkspace inputWorkspace = GSC_ProjectEditor.Workspace.AccessWorkspace(inputWorkspacePath);

            //Return object
            return OpenTopoLayerLayerFromWorkspace(inputWorkspace, topoName);

        }

        /// <summary>
        /// Will return a given topology object from a string of topo feature name, for PROJECT database only
        /// </summary>
        /// <param name="topoName"></param>
        /// <returns></returns>
        public static ITopology OpenTopoLayerLayerFromWorkspace(IWorkspace inWorkspace, string topoName)
        {

            //Get wanted feature dataset to add topology layer to
            IFeatureDataset inputGeoDataset = GSC_ProjectEditor.FeatureDataset.OpenFeatureDataSet(inWorkspace, GSC_ProjectEditor.Constants.Database.FDGeo);

            //Create a topo container from feature dataset
            ITopologyContainer2 topoContainer = inputGeoDataset as ITopologyContainer2;

            //Return object
            return topoContainer.get_TopologyByName(topoName);

        }

        #endregion

    }
}
