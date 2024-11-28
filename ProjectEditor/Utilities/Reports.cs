using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.ArcMap;
using ESRI.ArcGIS.Geodatabase;

namespace GSC_ProjectEditor
{
    public class Reports
    {
        /// <summary>
        /// Will build and export a report from a give ITable object.
        /// </summary>
        /// <param name="inTable">The table to build the report from</param>
        /// <param name="outputPath">The output file path, with extension to the report.</param>
        /// <param name="outReportType">The esri report type wanted for the output file.</param>
        /// <param name="templatePath">A template to build the report with.</param>
        public static void GenerateReport(ITable inTable, string outputPath, esriReportExportType outReportType, string templatePath)
        {
            //Build report title
            IDataset inTableDataset = inTable as IDataset;
            string reportTitle = inTableDataset.Name;

            //Define a new report and it's datasource
            IReportDataSource reportDataSource = new Report();
            reportDataSource.Table = inTable;

            //Add a template
            IReportTemplate reportTemplate = reportDataSource as IReportTemplate;
            reportTemplate.LoadReportTemplate(templatePath);
            reportTemplate.StartingPageNumber = 1;
            reportTemplate.ReportTitle = reportTitle;

            //Start report engine from table
            IReportEngine reportEngine;
            reportEngine = reportDataSource as IReportEngine;

            //Run
            reportEngine.RunReport(null);

            //Export
            reportEngine.ExportReport(outputPath, "1-2000", outReportType);
        }
    
    }
}
