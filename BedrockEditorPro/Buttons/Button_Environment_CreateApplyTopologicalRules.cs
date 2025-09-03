using ArcGIS.Core;
using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Data.Topology;
using ArcGIS.Core.Geometry;
using ArcGIS.Core.Internal.CIM;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Core.Geoprocessing;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.KnowledgeGraph;
using ArcGIS.Desktop.Layouts;
using ArcGIS.Desktop.Mapping;
using BedrockEditorPro.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BedrockEditorPro.Buttons
{
    internal class Button_Environment_CreateApplyTopologicalRules : Button
    {

        public string _geodatabasePath = string.Empty;
        public string GeodatabasePath
        {
            get { return _geodatabasePath; }
            set
            {
                SetProperty(ref _geodatabasePath, value, () => GeodatabasePath);
            }
        }

        protected override async void OnClick()
        {

            bool? success = await CreateAppllyTopologicalRules();

            if (success.HasValue)
            {
                if (success.HasValue && success.Value)
                {
                    //Show notification sucess
                    FrameworkApplication.AddNotification(new Notification()
                    {
                        Title = Properties.Resources.ButtonEnvironmentTopologicalRulesTitle,
                        Message = Properties.Resources.GenericMessageCompleted,
                        ImageSource = System.Windows.Application.Current.Resources["Success_Toast48"] as ImageSource
                    });
                }
                else
                {
                    MessageBox.Show(Properties.Resources.GenericMessageError, "", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }


        }

        /// <summary>
        /// Will create and apply all the topoligical rules needed
        /// in a bedrock model
        /// </summary>
        /// <returns></returns>
        public async Task<bool?> CreateAppllyTopologicalRules()
        {
            //Variables
            bool? success = null;

            //Get wanted geodatabase to add topology to
            string GeodatabasePath = Dialog.GetFGDBPrompt(Properties.Resources.ButtonEnvironmentTopologicalRulesPromptTitle);

            if (GeodatabasePath != null && GeodatabasePath != string.Empty)
            {
                success = await QueuedTask.Run(async Task<bool> () =>
                {
                    using (Geodatabase currentWorkspace = new Geodatabase(new FileGeodatabaseConnectionPath(new Uri(GeodatabasePath))))
                    {
                        if (currentWorkspace != null)
                        {
                            if (Utilities.Workspace.FeatureDatasetExists(currentWorkspace, Constants.Database.FDGeo) &&
                                Utilities.Workspace.FeatureClassExists(currentWorkspace, Constants.Database.FGeopoly) &&
                                Utilities.Workspace.FeatureClassExists(currentWorkspace, Constants.Database.FLabel) &&
                                Utilities.Workspace.FeatureClassExists(currentWorkspace, Constants.Database.FGeoline))
                            {

                                try
                                {
                                    FeatureDataset geoDataset = currentWorkspace.OpenDataset<FeatureDataset>(Constants.Database.FDGeo);

                                    //Detect if the topology is already in the database
                                    bool validTopology = true;
                                    if (!Utilities.Workspace.TopologyExists(currentWorkspace, Constants.Database.Topology))
                                    {
                                        //Create a topological layer within database
                                        IGPResult topoResult = await Utilities.GeoprocessingBedrock.CreateTopology(geoDataset.GetPath().AbsolutePath, Constants.Database.Topology);

                                        if (topoResult.IsFailed)
                                        {
                                            validTopology = false;
                                        }
                                    }

                                    if (validTopology)
                                    {
                                        using (Topology topo = currentWorkspace.OpenDataset<Topology>(Constants.Database.Topology))
                                        {
                                            using (TopologyDefinition topoDef = topo.GetDefinition())
                                            {

                                                #region For Geolines

                                                //Build list of topo rules
                                                List<Tuple<TopologyRuleType, string>> geolineRuleList = new List<Tuple<TopologyRuleType, string>>();
                                                geolineRuleList.Add(new Tuple<TopologyRuleType, string>(TopologyRuleType.LineNoOverlap, "Must Not Overlap (Line)"));
                                                geolineRuleList.Add(new Tuple<TopologyRuleType, string>(TopologyRuleType.LineNoDangles, "Must Not Have Dangles (Line)"));
                                                geolineRuleList.Add(new Tuple<TopologyRuleType, string>(TopologyRuleType.LineNoSelfOverlap, "Must Not Self-Overlap (Line)"));
                                                geolineRuleList.Add(new Tuple<TopologyRuleType, string>(TopologyRuleType.LineNoSelfIntersect, "Must Not Self-Intersect (Line)"));
                                                geolineRuleList.Add(new Tuple<TopologyRuleType, string>(TopologyRuleType.LineNoMultipart, "Must Be Single Part (Line)"));

                                                using (FeatureClass geolineFC = currentWorkspace.OpenDataset<FeatureClass>(Constants.Database.FGeoline))
                                                {

                                                    //Add feature class to topology
                                                    if (!topoDef.GetFeatureClassNames().Contains(Constants.Database.FGeoline))
                                                    {
                                                        await Utilities.GeoprocessingBedrock.AddFeatureClassToTopology(topo.GetPath().OriginalString, geolineFC.GetPath().OriginalString);

                                                    }

                                                    //Add rules to topology
                                                    foreach (Tuple<TopologyRuleType, string> geolineRules in geolineRuleList)
                                                    {
                                                        IGPResult newRulesResult = await Utilities.GeoprocessingBedrock.AddRuleToTopology(topo.GetPath().OriginalString, geolineRules.Item2, geolineFC.GetPath().OriginalString, geolineFC.GetPath().OriginalString);

                                                        if (newRulesResult.IsFailed)
                                                        {
                                                            return false;
                                                        }

                                                    }
                                                }

                                                #endregion

                                                #region For Geopolys and labels

                                                //Build list of topo rules
                                                List<Tuple<TopologyRuleType, string>> otherRuleList = new List<Tuple<TopologyRuleType, string>>();
                                                otherRuleList.Add(new Tuple<TopologyRuleType, string>(TopologyRuleType.AreaContainPoint, "Contains Point (Area-Point)"));

                                                using (FeatureClass geopolyFC = currentWorkspace.OpenDataset<FeatureClass>(Constants.Database.FGeopoly))
                                                {
                                                    using (FeatureClass labelFC = currentWorkspace.OpenDataset<FeatureClass>(Constants.Database.FLabel))
                                                    {

                                                        //Add feature classes to topology
                                                        if (!topoDef.GetFeatureClassNames().Contains(Constants.Database.FGeopoly))
                                                        {
                                                            await Utilities.GeoprocessingBedrock.AddFeatureClassToTopology(topo.GetPath().OriginalString, geopolyFC.GetPath().OriginalString);
                                                        }
                                                        if (!topoDef.GetFeatureClassNames().Contains(Constants.Database.FLabel))
                                                        {
                                                            await Utilities.GeoprocessingBedrock.AddFeatureClassToTopology(topo.GetPath().OriginalString, labelFC.GetPath().OriginalString);
                                                        }

                                                        //Add rules to topology
                                                        foreach (Tuple<TopologyRuleType, string> otherRules in otherRuleList)
                                                        {
                                                            IGPResult newRulesResult = await Utilities.GeoprocessingBedrock.AddRuleToTopology(topo.GetPath().OriginalString, otherRules.Item2, geopolyFC.GetPath().OriginalString, labelFC.GetPath().OriginalString);

                                                            if (newRulesResult.IsFailed)
                                                            {
                                                                return false;
                                                            }

                                                        }
                                                    }
                                                }

                                                #endregion

                                                return true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        new ErrorToLogFile($"{nameof(CreateAppllyTopologicalRules)}: Couldn't create a topology within {Constants.Database.FDGeo}.");
                                        return false;
                                    }

                                }
                                catch (Exception buttonCreateTopoClickExcept)
                                {
                                    new ErrorToLogFile(buttonCreateTopoClickExcept).WriteToFile();
                                    return false;
                                }
                            }
                            else
                            {
                                new ErrorToLogFile($"{nameof(CreateAppllyTopologicalRules)}: Can't find one of these [{Constants.Database.FDGeo}, {Constants.Database.FGeopoly}, {Constants.Database.FGeoline}, {Constants.Database.FLabel}] in which to apply rules.");
                                return false;
                            }
                        }
                    }

                    return true;
                });


            }
            else
            {
                return success;
            }

            return success;
        }
    }
}
