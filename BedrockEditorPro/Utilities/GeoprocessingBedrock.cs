using ArcGIS.Core.Data;
using ArcGIS.Desktop.Core.Geoprocessing;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BedrockEditorPro.Utilities
{
    public class GeoprocessingBedrock
    {
        /// <summary>
        /// Will append a datasets into a given table with no test on the field attributes
        /// </summary>
        /// <param name="inputWorkspace">The input database object</param>
        /// <param name="importPath">The input path to XML</param>
        public static async Task<IGPResult> AppendInEmptyTables(object inputTable, object targetTable)
        {

            //Build an array of parameters
            IEnumerable<string> valueArray = await QueuedTask.Run<IReadOnlyList<string>>(() =>
            {

                var valueArray = Geoprocessing.MakeValueArray(inputTable, targetTable, "NO_TEST");
                return valueArray;
            });

            //Launch
            IGPResult gpResult = await Geoprocessing.ExecuteToolAsync("management.append", valueArray, null, CancelableProgressor.None, GPExecuteToolFlags.Default);

            // Check if the tool was successful
            if (gpResult.IsFailed)
            {
                // display error messages if the tool fails, otherwise shows the default messages
                new ErrorService(gpResult).WriteToFile();

                FrameworkApplication.AddNotification(new Notification()
                {
                    Title = Properties.Resources.GenericMessageErrorTitle,
                    Message = Properties.Resources.GenericMessageError,
                    ImageSource = System.Windows.Application.Current.Resources["Warning_Toast48"] as ImageSource
                });
            }

            return gpResult;

        }

        /// <summary>
        /// Will convert a json report schema into an xml workspace
        /// </summary>
        /// <param name="inputWorkspace">The input database object</param>
        /// <param name="importPath">The input path to XML</param>
        public static async Task<IGPResult> ConvertJSONToXML(string reportPath, string outputFolder, string name)
        {
            //Build an array of parameters
            IEnumerable<string> valueArray = await QueuedTask.Run<IReadOnlyList<string>>(() =>
            {

                var valueArray = Geoprocessing.MakeValueArray(reportPath, outputFolder, name, "XML");
                return valueArray;
            });

            //Launch
            IGPResult gpResult = await Geoprocessing.ExecuteToolAsync("management.ConvertSchemaReport", valueArray, null, CancelableProgressor.None, GPExecuteToolFlags.Default);

            // Check if the tool was successful
            if (gpResult.IsFailed)
            {
                // display error messages if the tool fails, otherwise shows the default messages
                new ErrorService(gpResult).WriteToFile();

                FrameworkApplication.AddNotification(new Notification()
                {
                    Title = Properties.Resources.GenericMessageErrorTitle,
                    Message = Properties.Resources.GenericMessageError,
                    ImageSource = System.Windows.Application.Current.Resources["Warning_Toast48"] as ImageSource
                });
            }

            return gpResult;

        }

        /// <summary>
        /// Will create a topology layer within a given feature dataset
        /// </summary>
        /// <param name="featureDatasetPath">The input feature dataset path</param>
        /// <param name="topologyName">The output topology name</param>
        public static async Task<IGPResult> CreateTopology(string featureDatasetPath, string topologyName)
        {
            //Build an array of parameters
            IEnumerable<string> valueArray = await QueuedTask.Run<IReadOnlyList<string>>(() =>
            {

                var valueArray = Geoprocessing.MakeValueArray(featureDatasetPath, topologyName);
                return valueArray;
            });

            //Launch
            IGPResult gpResult = await Geoprocessing.ExecuteToolAsync("management.CreateTopology", valueArray, null, CancelableProgressor.None, GPExecuteToolFlags.Default);

            // Check if the tool was successful
            if (gpResult.IsFailed)
            {
                // display error messages if the tool fails, otherwise shows the default messages
                new ErrorService(gpResult).WriteToFile();

                FrameworkApplication.AddNotification(new Notification()
                {
                    Title = Properties.Resources.GenericMessageErrorTitle,
                    Message = Properties.Resources.GenericMessageError,
                    ImageSource = System.Windows.Application.Current.Resources["Warning_Toast48"] as ImageSource
                });
            }

            return gpResult;

        }

        /// <summary>
        /// Will add a given rule to a topology within a geodatabase
        /// </summary>
        /// <param name="featureDatasetPath">The input feature dataset path</param>
        /// <param name="topologyName">The output topology name</param>
        public static async Task<IGPResult> AddRuleToTopology(string topologyPath, string topologyRuleName, string fromFeatureClassPath, string againstFeatureClassPath, string fromSubtype = "", string againstSubtype = "")
        {
            //Build an array of parameters
            IEnumerable<string> valueArray = await QueuedTask.Run<IReadOnlyList<string>>(() =>
            {

                var valueArray = Geoprocessing.MakeValueArray(topologyPath, topologyRuleName, fromFeatureClassPath, fromSubtype, againstFeatureClassPath, againstSubtype);
                return valueArray;
            });

            //Launch
            IGPResult gpResult = await Geoprocessing.ExecuteToolAsync("management.AddRuleToTopology", valueArray, null, CancelableProgressor.None, GPExecuteToolFlags.Default);

            // Check if the tool was successful
            if (gpResult.IsFailed)
            {
                // display error messages if the tool fails, otherwise shows the default messages
                new ErrorService(gpResult).WriteToFile();

                FrameworkApplication.AddNotification(new Notification()
                {
                    Title = Properties.Resources.GenericMessageErrorTitle,
                    Message = Properties.Resources.GenericMessageError,
                    ImageSource = System.Windows.Application.Current.Resources["Warning_Toast48"] as ImageSource
                });
            }

            return gpResult;

        }

        /// <summary>
        /// Will add a given rule to a topology within a geodatabase
        /// </summary>
        /// <param name="featureDatasetPath">The input feature dataset path</param>
        /// <param name="topologyName">The output topology name</param>
        public static async Task<IGPResult> AddFeatureClassToTopology(string topologyPath, string featureClassPath)
        {
            //Build an array of parameters
            IEnumerable<string> valueArray = await QueuedTask.Run<IReadOnlyList<string>>(() =>
            {

                var valueArray = Geoprocessing.MakeValueArray(topologyPath, featureClassPath, 1, 1);
                return valueArray;
            });

            //Launch
            IGPResult gpResult = await Geoprocessing.ExecuteToolAsync("management.AddFeatureClassToTopology", valueArray, null, CancelableProgressor.None, GPExecuteToolFlags.Default);

            // Check if the tool was successful
            if (gpResult.IsFailed)
            {
                // display error messages if the tool fails, otherwise shows the default messages
                new ErrorService(gpResult).WriteToFile();

                FrameworkApplication.AddNotification(new Notification()
                {
                    Title = Properties.Resources.GenericMessageErrorTitle,
                    Message = Properties.Resources.GenericMessageError,
                    ImageSource = System.Windows.Application.Current.Resources["Warning_Toast48"] as ImageSource
                });
            }

            return gpResult;

        }

        /// <summary>
        /// Will create polygons out of a line layer and a given label one. By default the attributes will be kept
        /// </summary>
        /// <param name="featureDatasetPath">The input feature dataset path</param>
        /// <param name="topologyName">The output topology name</param>
        public static async Task<IGPResult> FeaturesToPolygon(FeatureLayer lineLayer, FeatureLayer labelLayer, string outputFeatureClass, bool keepAttributes = true)
        {
            //Build an array of parameters
            IEnumerable<string> valueArray = await QueuedTask.Run<IReadOnlyList<string>>(() =>
            {

                var valueArray = Geoprocessing.MakeValueArray(lineLayer, outputFeatureClass, null, keepAttributes, labelLayer);
                return valueArray;
            });

            //Launch
            IGPResult gpResult = await Geoprocessing.ExecuteToolAsync("management.FeatureToPolygon", valueArray, null, CancelableProgressor.None, GPExecuteToolFlags.Default);

            // Check if the tool was successful
            if (gpResult.IsFailed)
            {
                // display error messages if the tool fails, otherwise shows the default messages
                new ErrorService(gpResult).WriteToFile();

                FrameworkApplication.AddNotification(new Notification()
                {
                    Title = Properties.Resources.GenericMessageErrorTitle,
                    Message = Properties.Resources.GenericMessageError,
                    ImageSource = System.Windows.Application.Current.Resources["Warning_Toast48"] as ImageSource
                });
            }

            return gpResult;

        }

        /// <summary>
        /// Will dissolve the incoming features based on a given field, usually only the shape one
        /// </summary>
        /// <param name="inputFeature"></param>
        /// <param name="outputFeature"></param>
        /// <param name="dissolveField"></param>
        /// <returns></returns>
        public static async Task<IGPResult> Dissolve(object inputFeature, object outputFeature, string dissolveField)
        {
            //Build an array of parameters
            IEnumerable<string> valueArray = await QueuedTask.Run<IReadOnlyList<string>>(() =>
            {

                var valueArray = Geoprocessing.MakeValueArray(inputFeature, outputFeature, dissolveField, null, false);
                return valueArray;
            });

            //Launch
            IGPResult gpResult = await Geoprocessing.ExecuteToolAsync("management.Dissolve", valueArray, null, CancelableProgressor.None, GPExecuteToolFlags.Default);

            // Check if the tool was successful
            if (gpResult.IsFailed)
            {
                // display error messages if the tool fails, otherwise shows the default messages
                new ErrorService(gpResult).WriteToFile();

                FrameworkApplication.AddNotification(new Notification()
                {
                    Title = Properties.Resources.GenericMessageErrorTitle,
                    Message = Properties.Resources.GenericMessageError,
                    ImageSource = System.Windows.Application.Current.Resources["Warning_Toast48"] as ImageSource
                });
            }

            return gpResult;
        }

        /// <summary>
        /// Will remove empty and null geometries from a given feature class or feature layer
        /// List of all repairs https://pro.arcgis.com/en/pro-app/3.5/tool-reference/data-management/repair-geometry.htm
        /// </summary>
        /// <param name="inputFeature"></param>
        /// <param name="outputFeature"></param>
        /// <returns></returns>
        public static async Task<IGPResult> RepairGeometry(object inputFeature)
        {
            //Build an array of parameters
            IEnumerable<string> valueArray = await QueuedTask.Run<IReadOnlyList<string>>(() =>
            {

                var valueArray = Geoprocessing.MakeValueArray(inputFeature, true, "ESRI");
                return valueArray;
            });

            //Launch
            IGPResult gpResult = await Geoprocessing.ExecuteToolAsync("management.RepairGeometry", valueArray, null, CancelableProgressor.None, GPExecuteToolFlags.Default);

            // Check if the tool was successful
            if (gpResult.IsFailed)
            {
                // display error messages if the tool fails, otherwise shows the default messages
                new ErrorService(gpResult).WriteToFile();

                FrameworkApplication.AddNotification(new Notification()
                {
                    Title = Properties.Resources.GenericMessageErrorTitle,
                    Message = Properties.Resources.GenericMessageError,
                    ImageSource = System.Windows.Application.Current.Resources["Warning_Toast48"] as ImageSource
                });
            }

            return gpResult;
        }

        /// <summary>
        /// Will explode in single parts, any multipart features from the given layer/feature class
        /// </summary>
        /// <param name="inputFeature"></param>
        /// <param name="outputFeature"></param>
        /// <returns></returns>
        public static async Task<IGPResult> MultipartToSinglepart(object inputFeature, object outputFeature)
        {
            //Build an array of parameters
            IEnumerable<string> valueArray = await QueuedTask.Run<IReadOnlyList<string>>(() =>
            {

                var valueArray = Geoprocessing.MakeValueArray(inputFeature, outputFeature);
                return valueArray;
            });

            //Launch
            IGPResult gpResult = await Geoprocessing.ExecuteToolAsync("management.MultipartToSinglepart", valueArray, null, CancelableProgressor.None, GPExecuteToolFlags.Default);

            // Check if the tool was successful
            if (gpResult.IsFailed)
            {
                // display error messages if the tool fails, otherwise shows the default messages
                new ErrorService(gpResult).WriteToFile();

                FrameworkApplication.AddNotification(new Notification()
                {
                    Title = Properties.Resources.GenericMessageErrorTitle,
                    Message = Properties.Resources.GenericMessageError,
                    ImageSource = System.Windows.Application.Current.Resources["Warning_Toast48"] as ImageSource
                });
            }

            return gpResult;
        }

        /// <summary>
        /// Will densify a given feature class based on the project scale input for XY tolerance
        /// </summary>
        /// <param name="inputFeature"></param>
        /// <returns></returns>
        public static async Task<IGPResult> Densify(object inputFeature, int xyTolerance)
        {
            //Build an array of parameters
            IEnumerable<string> valueArray = await QueuedTask.Run<IReadOnlyList<string>>(() =>
            {

                var valueArray = Geoprocessing.MakeValueArray(inputFeature, "DISTANCE", string.Format("{0} meter",xyTolerance.ToString()));
                return valueArray;
            });

            //Launch
            IGPResult gpResult = await Geoprocessing.ExecuteToolAsync("edit.Densify", valueArray, null, CancelableProgressor.None, GPExecuteToolFlags.Default);

            // Check if the tool was successful
            if (gpResult.IsFailed)
            {
                // display error messages if the tool fails, otherwise shows the default messages
                new ErrorService(gpResult).WriteToFile();

                FrameworkApplication.AddNotification(new Notification()
                {
                    Title = Properties.Resources.GenericMessageErrorTitle,
                    Message = Properties.Resources.GenericMessageError,
                    ImageSource = System.Windows.Application.Current.Resources["Warning_Toast48"] as ImageSource
                });
            }

            return gpResult;
        }

    }
}
