using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;

namespace GSC_ProjectEditor
{
    public class Queries
    {
        /// <summary>
        /// Will build an 'OR' only query from a given dictionary and a single field. The query is a definition query, intented to be used with selection layers
        /// </summary>
        /// <param name="fieldName"> The field name to add to query</param>
        /// <param name="fieldValueList"> A string list of all field values</param>
        /// <param name="fieldType"> Can be "String" or "Int". Will add " or not depending on the choice.</param>
        /// <param name="QuerySymbol">"OR", "AND", etc.</param>
        /// <param name="definitionQuery">Indicate if the query will be use for definition queries (true) else field names needs "</param>
        /// <returns></returns>
        public static string BuildQueryFromStringList(string fieldName, List<string> fieldValueList, string fieldType, string querySymbol, IWorkspace inputWork)
        {
            //Variables
            string outputQuery = "";
            string bracketValue = ""; //Default for int values
            string mainQuery = ""; //init.
            string querySym = " " + querySymbol + " ";

            //Get proper field delimiter from workspace
            ISQLSyntax sqlSyn = inputWork as ISQLSyntax;
            string prefix = sqlSyn.GetSpecialCharacter(esriSQLSpecialCharacters.esriSQL_DelimitedIdentifierPrefix);
            string suffix = sqlSyn.GetSpecialCharacter(esriSQLSpecialCharacters.esriSQL_DelimitedIdentifierSuffix);

            //Modify bracket variable when it's needed
            if (fieldType == "String")
            {
                bracketValue = "'";
            }

            foreach(string values in fieldValueList)
            {
                ///USING PREFIX AND SUFFIX CAUSES PROBLEM BECAUSE " caracters are inserted and query fails because of this...
                //mainQuery = prefix + fieldName + suffix + " = " + bracketValue + values + bracketValue;
                mainQuery = fieldName + " = " + bracketValue + values + bracketValue;
                //First iteration
                if (outputQuery == "")
                {
                    outputQuery = mainQuery;
                }

                //Iteration > 1
                else
                {
                    outputQuery = outputQuery + querySym + mainQuery;
                }

            }

            return outputQuery; //Last object id number plus 1.
        }
    }
}
