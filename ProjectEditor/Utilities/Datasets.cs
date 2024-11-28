using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;

namespace GSC_ProjectEditor
{
    public class Datasets
    {
        /// <summary>
        /// From a given string path, will find if the entry has some geometry inside it.
        /// Doing so will make the difference between a feature class or a table for exemple.
        /// 
        /// Another way would have been to open the table from a string, but this method uses a
        /// geoprocessing object that requires more time to the system. This current method is faster.
        /// </summary>
        /// <param name="inputPath"></param>
        /// <returns></returns>
        public static bool GDBDatasetHasGeometry(string inputPath)
        {
            //Variables
            bool haveGeometry = false;

            //Access the table behind the dataset
            ITable inputDatasetTable = GSC_ProjectEditor.Tables.OpenTableFromStringFaster(inputPath);

            //Iterate through field in search of the geometry field (SHAPE)
            IFields fieldList = inputDatasetTable.Fields;
            for (int i = 0; i < fieldList.FieldCount; i++)
            {
                if (fieldList.get_Field(i).GeometryDef != null)
                {
                    haveGeometry = true;
                }
            }

            GSC_ProjectEditor.ObjectManagement.ReleaseObject(inputDatasetTable);
            GSC_ProjectEditor.ObjectManagement.ReleaseObject(fieldList);

            return haveGeometry;
        }
    }
}
