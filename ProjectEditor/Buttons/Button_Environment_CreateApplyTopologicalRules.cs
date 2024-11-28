using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace GSC_ProjectEditor
{
    public class Button_Environment_CreateApplyTopologicalRules : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public Button_Environment_CreateApplyTopologicalRules()
        {

        }

        protected override void OnClick()
        {

            //Set culture
            Utilities.Culture.SetCulture();

            //Get wanted geodatabase to add topology to
            string input_geodatabase_path = Dialog.GetFGDBPrompt(ArcMap.Application.hWnd, "Select File Geodatabase to add Topology to: ");

            if (input_geodatabase_path!=null && input_geodatabase_path != string.Empty)
            {
                List<string> datasetRestriction = new List<string>();
                datasetRestriction.Add(GSC_ProjectEditor.Constants.Database.FGeoline);
                datasetRestriction.Add(GSC_ProjectEditor.Constants.Database.FGeopoly);
                datasetRestriction.Add(GSC_ProjectEditor.Constants.Database.FLabel);

                IWorkspace currentWorkspace = Workspace.AccessWorkspace(input_geodatabase_path);

                if (currentWorkspace != null && input_geodatabase_path != string.Empty)
                {
                    bool validDatasets = Workspace.isWorkspaceAProperProjectDatabase(currentWorkspace, datasetRestriction, true, false);

                    if (validDatasets)
                    {
                        try
                        {
                            //Get wanted feature dataset to add topology layer to
                            IFeatureDataset inputGeoDataset = GSC_ProjectEditor.FeatureDataset.OpenFeatureDataSet(currentWorkspace, GSC_ProjectEditor.Constants.Database.FDGeo);

                            //Create a topological layer within database
                            ITopology topo = GSC_ProjectEditor.Topology.CreateTopoLayerFromWorkspace(currentWorkspace, inputGeoDataset, false, GSC_ProjectEditor.Constants.Database.Topology);

                            //Make sure a topology was really created
                            if (topo != null)
                            {
                                #region For Geolines
                                //Add wanted feature into the topo layer
                                IFeatureClass geolineFC = GSC_ProjectEditor.FeatureClass.OpenFeatureClassFromWorkspace(currentWorkspace, GSC_ProjectEditor.Constants.Database.FGeoline);
                                topo.AddClass(geolineFC, 1, 1, 1, false);

                                //Build list of topo rules
                                List<Tuple<esriTopologyRuleType, string>> ruleList = new List<Tuple<esriTopologyRuleType, string>>();
                                ruleList.Add(new Tuple<esriTopologyRuleType, string>(esriTopologyRuleType.esriTRTLineNoOverlap, "Must Not Overlap"));
                                ruleList.Add(new Tuple<esriTopologyRuleType, string>(esriTopologyRuleType.esriTRTLineNoDangles, "Must Not Have Dangles (Line)"));
                                ruleList.Add(new Tuple<esriTopologyRuleType, string>(esriTopologyRuleType.esriTRTLineNoSelfOverlap, "Must Not Self-Overlap (Line)"));
                                ruleList.Add(new Tuple<esriTopologyRuleType, string>(esriTopologyRuleType.esriTRTLineNoSelfIntersect, "Must Not Self-Intersect (Line)"));
                                ruleList.Add(new Tuple<esriTopologyRuleType, string>(esriTopologyRuleType.esriTRTLineNoMultipart, "Must Be Single Part (Line)"));

                                //Add rules to topology
                                GSC_ProjectEditor.Topology.AddRulesToTopology(ruleList, topo, geolineFC, geolineFC, true);

                                #endregion

                                #region For Geopolys and labels
                                //Add wanted feature into the topo layer
                                IFeatureClass geopolys = GSC_ProjectEditor.FeatureClass.OpenFeatureClassFromWorkspace(currentWorkspace, GSC_ProjectEditor.Constants.Database.FGeopoly);
                                IFeatureClass geoLabels = GSC_ProjectEditor.FeatureClass.OpenFeatureClassFromWorkspace(currentWorkspace, GSC_ProjectEditor.Constants.Database.FLabel);
                                topo.AddClass(geopolys, 1, 1, 1, false);
                                topo.AddClass(geoLabels, 1, 1, 1, false);

                                //Build list of topo rules
                                List<Tuple<esriTopologyRuleType, string>> polyRuleList = new List<Tuple<esriTopologyRuleType, string>>();
                                polyRuleList.Add(new Tuple<esriTopologyRuleType, string>(esriTopologyRuleType.esriTRTAreaContainPoint, "Contains Point"));

                                //Add rules to topology
                                GSC_ProjectEditor.Topology.AddRulesToTopology(polyRuleList, topo, geopolys, geoLabels, false);

                                #endregion

                                //Cast geoline extent as envlope
                                IGeoDataset getDS = topo as IGeoDataset;
                                IEnvelope extent = getDS.Extent;

                                //Validate topo
                                GSC_ProjectEditor.Topology.ValidatTopology(topo, extent);

                                GSC_ProjectEditor.Messages.ShowEndOfProcess();
                            }
                            else
                            {
                                MessageBox.Show(Properties.Resources.Error_TopologyAlreadyExists);
                            }

                        }
                        catch (Exception buttonCreateTopoClickExcept)
                        {
                            MessageBox.Show(buttonCreateTopoClickExcept.StackTrace);
                        }
                    }
                }
            }


        }

        protected override void OnUpdate()
        {

        }


    }
}
