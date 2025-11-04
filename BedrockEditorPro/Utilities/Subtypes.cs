using ArcGIS.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BedrockEditorPro.Utilities
{
    public class Subtypes
    {
        /// <summary>
        /// Retrieve a complete code and description dictionary from a subtype of project database, specify order Key = Code or Key = Description
        /// </summary>
        /// <param name="sourceGeodatabase"></param>
        /// <param name="tableName"></param>
        /// <param name="domName"></param>
        /// <returns></returns>
        public static SortedList<string, string> GetSubtypeDicoFromWorkspace(Geodatabase sourceGeodatabase, string tableName)
        {
            //Variables
            SortedList<string, string> subtypeDico = new SortedList<string, string>();

            //Get subtype codes
            try
            {
                using (Table inputTable = sourceGeodatabase.OpenDataset<Table>(tableName))
                {
                    using (TableDefinition tableDefinition = inputTable.GetDefinition())
                    {
                        // Get name of subtype field
                        string subtypeFieldName = tableDefinition.GetSubtypeField();

                        // Get subtype, if any
                        List<Subtype> subtypes = new List<Subtype>();

                        if (subtypeFieldName.Length != 0)
                        {
                            // Get subtype for this row
                            subtypes.AddRange(tableDefinition.GetSubtypes().ToList());

                            foreach (Subtype subs in subtypes)
                            {
                                subtypeDico[subs.GetName().ToString()] = subs.GetCode().ToString();
                            }
                        }
                    }
                }


            }
            catch
            {
                //Subtype could be missing, skip if it crashes, the returned list will be empty
            }

            return subtypeDico;

        }
    }
}
