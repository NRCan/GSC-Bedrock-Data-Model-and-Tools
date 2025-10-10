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
    }
}
