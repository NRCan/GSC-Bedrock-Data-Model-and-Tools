using ArcGIS.Core.Data;
using ArcGIS.Core.Data.DDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BedrockEditorPro.Utilities
{
    public class Domains
    {

        /// <summary>
        /// Retrieve a complete code and description dictionary from a domain of project database, specify order Key = Code or Key = Description
        /// </summary>
        /// <param name="domName">Reference domain name to get dico from</param>
        /// <returns></returns>
        public static SortedList<object, string> GetDomDicoFromWorkspace(Geodatabase sourceGeodatabase, string domName)
        {
            //Variables
            SortedList<object, string> domDico = null; 

            //Get domain coded value object
            try
            {

                CodedValueDomain codedValueDomain = sourceGeodatabase.GetDomains().FirstOrDefault(d => d.GetName().Equals(domName)) as CodedValueDomain;

                if (codedValueDomain != null)
                {
                    SortedList<object, string> keyValuePairs = codedValueDomain.GetCodedValuePairs();

                    if (keyValuePairs != null && keyValuePairs.Count() > 0)
                    {
                        domDico = keyValuePairs;
                    }
                }
            }
            catch
            {
                //Domain could be missing, skip if it crashes, the returned list will be empty
            }

            return domDico;

        }

        /// <summary>
        /// Will return a dictionary of code and description from a domain assigned to a specific subtype and field
        /// </summary>
        /// <param name="subtype"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static SortedList<object, string> GetDomDicoFromSubtype(Geodatabase sourceGeodatabase, string tableName, string subtypeCode, string fieldName)
        {
            //Variables
            SortedList<object, string> domDico = null;

            //Get domain coded value object
            try
            {
                using (Table inputTable = sourceGeodatabase.OpenDataset<Table>(tableName))
                {
                    using (TableDefinition tableDefinition = inputTable.GetDefinition())
                    {
                        // Get name of subtype field
                        string subtypeFieldName = tableDefinition.GetSubtypeField();

                        // Get subtype, if any
                        Subtype subtype = null;
                        if (subtypeFieldName.Length != 0)
                        {
                            // Get subtype for this row
                            subtype = tableDefinition.GetSubtypes().First(x => x.GetCode().ToString() == subtypeCode);
                        }

                        // Get the field
                        Field field = tableDefinition.GetFields().First(x => x.Name == fieldName);

                        if (field != null && subtype != null)
                        {
                            // Get the coded value domain for this field
                            CodedValueDomain domain = field.GetDomain(subtype) as CodedValueDomain;
                            if (domain != null)
                            {
                                SortedList<object, string> keyValuePairs = domain.GetCodedValuePairs();
                                if (keyValuePairs != null && keyValuePairs.Count() > 0)
                                {
                                    domDico = keyValuePairs;
                                }
                            }
                        }

                    }
                }

            }
            catch
            {
                //Domain could be missing, skip if it crashes, the returned list will be empty
            }

            return domDico;

        }
    }
}
