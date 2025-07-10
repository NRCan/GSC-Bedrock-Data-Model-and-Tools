using ArcGIS.Desktop.Core.Geoprocessing;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Threading.Tasks;
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
        public static async Task<IGPResult> AppendInEmptyTables(string inputTablePath, string targetTablePath)
        {

            //Build an array of parameters
            IEnumerable<string> valueArray = await QueuedTask.Run<IReadOnlyList<string>>(() =>
            {

                var valueArray = Geoprocessing.MakeValueArray(inputTablePath, targetTablePath, "NO_TEST");
                return valueArray;
            });

            //Launch
            IGPResult gpResult = await Geoprocessing.ExecuteToolAsync("management.append", valueArray, null, CancelableProgressor.None, GPExecuteToolFlags.Default);

            // Check if the tool was successful
            if (gpResult.IsFailed)
            {
                // display error messages if the tool fails, otherwise shows the default messages
                new ErrorToLogFile(gpResult).WriteToFile();

                FrameworkApplication.AddNotification(new Notification()
                {
                    Title = Properties.Resources.FormEnvironmentNewGeodatabaseTitle,
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
                new ErrorToLogFile(gpResult).WriteToFile();

                FrameworkApplication.AddNotification(new Notification()
                {
                    Title = Properties.Resources.FormEnvironmentNewGeodatabaseTitle,
                    Message = Properties.Resources.GenericMessageError,
                    ImageSource = System.Windows.Application.Current.Resources["Warning_Toast48"] as ImageSource
                });
            }

            return gpResult;

        }
    }
}
