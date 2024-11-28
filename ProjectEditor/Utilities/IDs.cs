using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;

namespace GSC_ProjectEditor
{
    public class IDs
    {

        #region Main Variables

        //GEO_LINE feature
        private const string geolineFeature = GSC_ProjectEditor.Constants.Database.FGeoline;
        private const string geolineType = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineSubtype;
        private const string geolineQualif = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineQualif;
        private const string geolineConf = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineConf;
        private const string geolineAttitude = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineAtt;
        private const string geolineGeneration = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineGeneration;
        private const string geolineID = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineID;
        private const string geolineSymbol = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineFGDC;

        //M_GEOLINE_SYMBOL
        private const string mGeolineSymbol = GSC_ProjectEditor.Constants.Database.TGeolineSymbol;
        private const string mGeolineID = GSC_ProjectEditor.Constants.DatabaseFields.MGeolineID;
        private const string mGeolineFGDC = GSC_ProjectEditor.Constants.DatabaseFields.MGeolineFGDC;

        //GEO_POINT feature
        private const string geopointFeature = GSC_ProjectEditor.Constants.Database.FGeopoint;
        private const string geopointID = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointID;
        private const string geopointSymbol = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointFGDC;
        private const string geopointType = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointType;
        private const string geopointSubset = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointSubset;
        private const string geopointStrucAtt = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointStrucAtt;
        private const string geopointStrucGene = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointStrucGene;
        private const string geopointStrucYoung = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointStrucYoung;
        private const string geopointStrucMethod = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointStrucMethod;

        //SYMBOL_GEOPOINTS table
        private const string tGeopointSymbol = GSC_ProjectEditor.Constants.Database.TGeopointSymbol;
        private const string tGeopointID = GSC_ProjectEditor.Constants.DatabaseFields.TGeopointID;
        private const string tGeopointFGDC = GSC_ProjectEditor.Constants.DatabaseFields.TGeopointFGDC;

        //LEGEND TABLE
        private const string tLegend = GSC_ProjectEditor.Constants.Database.TLegendGene;
        private const string tLegendID = GSC_ProjectEditor.Constants.DatabaseFields.LegendLabelID;
        private const string tLegendSymbolID = GSC_ProjectEditor.Constants.DatabaseFields.LegendSymbol;

        #endregion

        #region Generic IDs

        /// <summary>
        /// Will calculate an ID based on a GUID that will be converted into a double value.
        /// </summary>
        /// <returns></returns>
        public static double CalculateDoubleIDFromGUID()
        {
            Guid newGUID = Guid.NewGuid();
            double newDouble = BitConverter.ToDouble(newGUID.ToByteArray(), 0);

            return newDouble;
        }

        /// <summary>
        /// Will calculate current time in this format YYMMDDHHMMSS then add a random 2 digits number to the end of the date.
        /// </summary>
        /// <returns></returns>
        public static long CalculateIDFromCustomTime()
        {
            //Calculate current time
            DateTime now = DateTime.Now;
            Random someRandom4digits = new Random();
            long currentStrDate = Convert.ToInt64(String.Format("{0:yyMMddhhmmss}", now) + someRandom4digits.Next(99).ToString());

            return currentStrDate;
        }

        /// <summary>
        /// Calculate an ID based on object ID from table
        /// </summary>
        /// <param name="idTable">A table name in which ids will be calculated</param>
        /// <param name="inputQuery">Input a query if needed, else enter null</param>
        /// <returns></returns>
        public static int CalculateIDFromOBJECTID(string idTable, string inputQuery)
        {
            //Variables
            int newID = 0;
            int caseNoObjectID = 1;
            string objField = GSC_ProjectEditor.Constants.DatabaseFields.ObjectID;

            //Get list of current ids
            List<string> ids = GSC_ProjectEditor.Tables.GetFieldValues(idTable, objField, inputQuery);

            //Get highest value
            foreach (string id in ids)
            {
                if (Convert.ToInt16(id) > newID)
                {
                    newID = Convert.ToInt16(id);
                }
            }
            
            //In case there is no objectID
            if (newID==0)
            {
                newID = caseNoObjectID;
            }

            return newID + 1; //Last object id number plus 1.
        }

        /// <summary>
        /// Calculate an ID based on object count from a table
        /// </summary>
        /// <param name="idTable">Calculate ID from input table</param>
        /// <param name="inputQuery">Pass a query to filter id</param>
        /// <returns></returns>
        public static int CalculateIDFromCount(string idTable, string inputQuery)
        {
            //Variables
            int rowCount = Tables.GetRowCount(idTable, inputQuery);

            return rowCount + 1; //Last object id number plus 1.
        }

        /// <summary>
        /// Will return an id based on alphabet
        /// </summary>
        /// <param name="idAlphaTable">input table to base increment from</param>
        /// <param name="overload"> if True, value will be AA, AB, etc. if it goes over 26, until for infinity, if false returned value will always be between A and Z.</param>
        /// <returns>Shall return A;Z than AA, AB, AC, etc.</returns>
        public static string CalculateSimplementAlphabeticIDFromTable(string idAlphaTable, bool overload, string inputAlphaQuery)
        {
            //Variable
            string id = ""; //New calculated value to return

            //Retrieve a new numerical id
            int numID = CalculateIDFromCount(idAlphaTable, inputAlphaQuery);

            id = CalculateSimplementAlphabeticID(overload, numID);

            return id;
        }

        /// <summary>
        /// Will return an id based on alphabet
        /// </summary>
        /// <param name="idAlphaTable">input table to base increment from</param>
        /// <param name="overload"> if True, value will be AA, AB, etc. if it goes over 26, until for infinity, if false returned value will always be between A and Z.</param>
        /// <returns>Shall return A;Z than AA, AB, AC, etc.</returns>
        public static string CalculateSimplementAlphabeticID(bool overload, int startingNumber)
        {
            //Variable
            string id = ""; //New calculated value to return
            int getRemainder; //In case it's needed, will be calculate from modulo
            int getQuotient; //In case needed, will be used for overload

            List<string> alphaList = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

            //Retrieve a new numerical id
            int numID = startingNumber;

            //Parse new id with associated caracter
            if (numID <= alphaList.Count)
            {
                id = alphaList[numID - 1];
            }
            else
            {
                //Get modulo from id
                getRemainder = numID % alphaList.Count;

                //Get current alpha from modulo
                id = alphaList[getRemainder - 1];

                //Get if user wants an overload
                if (overload == true)
                {
                    //Get how many time alphalist.count is found within current numID
                    getQuotient = numID / alphaList.Count;

                    //Check if quotient is within count range, if over, iterate until it's under range of the count number
                    if (getQuotient >= alphaList.Count)
                    {
                        //Iteration
                        while (getQuotient > alphaList.Count)
                        {
                            //Get modulo again
                            getRemainder = getQuotient % alphaList.Count;

                            //Add new alpha id to current id
                            id = alphaList[getRemainder - 1] + id;

                            //Recalculate quotient to make while run
                            getQuotient = getQuotient / alphaList.Count;
                        }

                        //Get id result of final while iteration
                        id = alphaList[getQuotient - 1] + id;
                    }
                    else
                    {
                        id = alphaList[getQuotient - 1] + id;
                    }

                }
            }

            return id;
        }

        /// <summary>
        /// Will calculate a random number between 0 and the max length of long variable (9,223,372,036,854,775,807)
        /// </summary>
        /// <param name="inputRandom">input the random variable.</param>
        /// <returns></returns>
        public static int GetRandomInt(Random inputRandom)
        {
            //Variable
            int randomNumber = 0;

            //Process
            randomNumber = (int)Math.Abs(inputRandom.Next());

            return randomNumber;
        }

        /// <summary>
        /// Calculates an ID based on domain object count from project database
        /// </summary>
        /// <param name="domName"></param>
        /// <returns></returns>
        public static int CalculateIDFromDomainCount(string domName)
        {
            return Domains.GetDomValueList(domName).Count;
        }

        #endregion

        #region Project related IDs

        /// <summary>
        /// Will calculate or update geolineID field within geoline feature, based on its subtypes and domains
        /// </summary>
        public static void CalculateGeolineID()
        {
            //Manage iterative message to show up multiple times
            GSC_ProjectEditor.Properties.Settings.Default.IterativeMessage = new System.Collections.Specialized.StringCollection();
            GSC_ProjectEditor.Properties.Settings.Default.Save();

            //Get a feature cursor
            IFeatureCursor getFeatCursor = FeatureClass.GetFeatureCursor("Update", null, geolineFeature);

            //Iterate through cursor
            IFeature getRow = null;
            while ((getRow = getFeatCursor.NextFeature()) != null)
            {
                //Pass the row object to another validator
                CalculateGeolineIDFromRow(getRow);

                getFeatCursor.UpdateFeature(getRow);
            }

            //Release the cursor or else some lock could happen.
            System.Runtime.InteropServices.Marshal.ReleaseComObject(getFeatCursor);
        }

        /// <summary>
        /// Will calculate or update a geoline id passed from a row object
        /// </summary>
        /// <param name="inputRow">An feature object containing the desire row to validate id.</param>
        public static void CalculateGeolineIDFromRow(IObject inputRow)
        {
            //Get some info. about geoline id field   
            int geolineIDIndex = inputRow.Fields.FindField(geolineID);
            int geolineSymbolIndex = inputRow.Fields.FindField(geolineSymbol);

            //If geoline id field exists
            if (geolineIDIndex != -1)
            {
                //Get geolineid value
                var getgeolineIDValue = inputRow.get_Value(geolineIDIndex);

                #region Get current field values

                int currentTypeIndex = inputRow.Fields.FindField(geolineType);
                var currentType = inputRow.get_Value(currentTypeIndex);

                int currentQualifierIndex = inputRow.Fields.FindField(geolineQualif);
                var currentQualifier = inputRow.get_Value(currentQualifierIndex);

                int currentConfidenceIndex = inputRow.Fields.FindField(geolineConf);
                var currentConfidence = inputRow.get_Value(currentConfidenceIndex);

                int currentAttitudeIndex = inputRow.Fields.FindField(geolineAttitude);
                var currentAttitude = inputRow.get_Value(currentAttitudeIndex);

                int currentGenerationIndex = inputRow.Fields.FindField(geolineGeneration);
                var currentGeneration = inputRow.get_Value(currentGenerationIndex);

                #endregion

                #region Calculate

                if (currentType != DBNull.Value && currentQualifier != DBNull.Value && currentConfidence != DBNull.Value && currentAttitude != DBNull.Value && currentGeneration != DBNull.Value)
                {
                    //Build new geolineID
                    string currentCalculatedID = currentType.ToString() + currentQualifier.ToString() + currentConfidence.ToString() + currentAttitude.ToString() + currentGeneration.ToString();

                    //Validate geolineID and symbol code
                    if (currentCalculatedID != getgeolineIDValue.ToString())
                    {
                        UpdateGeolineIDFields(inputRow, currentCalculatedID, geolineIDIndex, geolineSymbolIndex);
                    }
                }


                #endregion
            }
        }

        /// <summary>
        /// Will update geolineID field and trigger error message if it doesn't exist
        /// </summary>
        /// <param name="geoObject">the object containing row information</param>
        /// <param name="newGeoID">the new geoline id</param>
        /// <param name="idFieldIndex"> The field index for geoline id, within feature class</param>
        public static void UpdateGeolineIDFields(IObject geoObject, string newGeoID, int idFieldIndex, int symbolFieldIndex)
        {
            try
            {

                //Validate if new value can exist
                string getNewGeolineQuery = mGeolineID + " = '" + newGeoID + "'";
                string getNewGeopointLegendQuery = tLegendID + " = '" + newGeoID + "'";
                Dictionary<string, List<string>> getValueFromTable = GSC_ProjectEditor.Tables.GetUniqueFieldValues(mGeolineSymbol, mGeolineID, getNewGeolineQuery, true, mGeolineFGDC);
                Dictionary<string, List<string>> getValueFromLegend = GSC_ProjectEditor.Tables.GetUniqueFieldValues(tLegend, tLegendID, getNewGeopointLegendQuery, true, tLegendSymbolID);

                if (getValueFromLegend["Main"].Count != 0)
                {
                    //Update geolineID
                    geoObject.set_Value(idFieldIndex, newGeoID);
                    geoObject.set_Value(symbolFieldIndex, getValueFromLegend["Tag"][0]);


                }
                else if (getValueFromTable["Main"].Count != 0)
                {
                    //Update geopointID
                    geoObject.set_Value(idFieldIndex, newGeoID);
                    geoObject.set_Value(symbolFieldIndex, getValueFromTable["Tag"][0]);
                }
                else
                {
                    //Update symbol
                    geoObject.set_Value(idFieldIndex, newGeoID);
                    geoObject.set_Value(symbolFieldIndex, DBNull.Value);
                }

            }
            catch (Exception updateFieldException)
            {
                MessageBox.Show("updateFieldException: " + updateFieldException.Message);
            }


        }

        /// <summary>
        /// Will calculate or update geopointID field within geopoint feature, based on its subtypes and domains
        /// </summary>
        public static void CalculateGeopointID()
        {
            //Get a feature cusor
            IFeatureCursor getFeatCursor = FeatureClass.GetFeatureCursor("Update", null, geopointFeature);

            //Iterate through cursor
            IFeature getRow = null;

            //Manage iterative message to show up multiple times
            GSC_ProjectEditor.Properties.Settings.Default.IterativeMessage = new System.Collections.Specialized.StringCollection();
            GSC_ProjectEditor.Properties.Settings.Default.Save();

            while ((getRow = getFeatCursor.NextFeature()) != null)
            {
                //Pass the row object ot another validator
                CalculateGeopointIDFromRow(getRow);

                //Update row
                getFeatCursor.UpdateFeature(getRow);
            }

            //Release the cursor or else some lock could happen
            System.Runtime.InteropServices.Marshal.ReleaseComObject(getFeatCursor);

        }

        /// <summary>
        /// Will calculate or update a geopoint id passed from a row object
        /// </summary>
        /// <param name="getRow">A feature object containing the desire row to validate id.</param>
        public static void CalculateGeopointIDFromRow(IObject inputRow)
        {
            //Get some info. about geopoint id field   
            int geopointIDIndex = inputRow.Fields.FindField(geopointID);
            int geopointSymbolIndex = inputRow.Fields.FindField(geopointSymbol);

            //If geopoint id field exists
            if (geopointIDIndex != -1)
            {
                //Get geolineid value
                var getgeopointIDValue = inputRow.get_Value(geopointIDIndex);

                #region Get current field values

                int currentTypeIndex = inputRow.Fields.FindField(geopointType);
                var currentType = inputRow.get_Value(currentTypeIndex);

                int currentSubsetIndex = inputRow.Fields.FindField(geopointSubset);
                var currentSubset = inputRow.get_Value(currentSubsetIndex);

                int currentAttIndex = inputRow.Fields.FindField(geopointStrucAtt);
                var currentAtt = inputRow.get_Value(currentAttIndex);

                int currentGenIndex = inputRow.Fields.FindField(geopointStrucGene);
                var currentGen = inputRow.get_Value(currentGenIndex);

                int currentYoungIndex = inputRow.Fields.FindField(geopointStrucYoung);
                var currentYoung = inputRow.get_Value(currentYoungIndex);

                int currentMethodIndex = inputRow.Fields.FindField(geopointStrucMethod);
                var currentmethod = inputRow.get_Value(currentMethodIndex);

                #endregion

                #region Calculate
                if (currentType != DBNull.Value && currentSubset != DBNull.Value && currentAtt != DBNull.Value && currentGen != DBNull.Value && currentYoung != DBNull.Value && currentmethod != DBNull.Value)
                {
                    //Build new geolineID
                    string currentCalculatedID = currentType.ToString() + currentSubset.ToString() + currentAtt.ToString() + currentGen.ToString() + currentYoung.ToString() + currentmethod.ToString();

                    //Validate geolineID and symbol code
                    if (currentCalculatedID != getgeopointIDValue.ToString())
                    {
                        UpdateGeopointIDFields(inputRow, currentCalculatedID, geopointIDIndex, geopointSymbolIndex);
                    }
                }

                #endregion


            }

        }

        /// <summary>
        /// Will update geopointID field and trigger error message if it doesn't exist
        /// </summary>
        /// <param name="geoObject">the object containing row information</param>
        /// <param name="newGeoID">the new geopoint id</param>
        /// <param name="idFieldIndex"> The field index for geopoint id, within feature class</param>
        public static void UpdateGeopointIDFields(IObject geoObject, string newGeoID, int idFieldIndex, int symbolFieldIndex)
        {
            try
            {

                //Validate if new value can exist
                string getNewGeopointQuery = tGeopointID + " = '" + newGeoID + "'";
                string getNewGeopointLegendQuery = tLegendID + " = '" + newGeoID + "'";
                Dictionary<string, List<string>> getValueFromSymbol = GSC_ProjectEditor.Tables.GetUniqueFieldValues(tGeopointSymbol, tGeopointID, getNewGeopointQuery, true, tGeopointFGDC);
                Dictionary<string, List<string>> getValueFromLegend = GSC_ProjectEditor.Tables.GetUniqueFieldValues(tLegend, tLegendID, getNewGeopointLegendQuery, true, tLegendSymbolID);

                if (getValueFromLegend["Main"].Count != 0)
                {
                    //Update geopointID
                    geoObject.set_Value(idFieldIndex, newGeoID);
                    geoObject.set_Value(symbolFieldIndex, getValueFromLegend["Tag"][0]);


                }
                else if (getValueFromSymbol["Main"].Count != 0)
                {
                    //Update geopointID
                    geoObject.set_Value(idFieldIndex, newGeoID);
                    geoObject.set_Value(symbolFieldIndex, getValueFromSymbol["Tag"][0]);
                }
                else
                {
                    //Update symbol
                    geoObject.set_Value(idFieldIndex, newGeoID);
                    geoObject.set_Value(symbolFieldIndex, DBNull.Value);
                }

            }
            catch (Exception updateFieldException)
            {
                MessageBox.Show("updateFieldException: " + updateFieldException.Message);
            }


        }

        /// <summary>
        /// An ID calculate only for the project GDB project table ids.
        /// </summary>
        /// <param name="inTable">The person table to get ids information from</param>
        /// <param name="inQuery">A query to refine id, in case.</param>
        /// <returns></returns>
        public static int CalculateProjectID(string inTable, string inQuery)
        {
            return 1000 + GSC_ProjectEditor.IDs.CalculateIDFromOBJECTID(inTable, inQuery);
        }

        /// <summary>
        /// An ID calculator only for project GDB person table ids.
        /// </summary>
        /// <param name="inTable">The person table to get ids information from</param>
        /// <param name="inQuery">A query to refine id, in case.</param>
        /// <returns></returns>
        public static int CalculatePersonID(string inTable, string inQuery)
        {
            return  GSC_ProjectEditor.IDs.CalculateIDFromCount(inTable, inQuery) + GSC_ProjectEditor.IDs.CalculateIDFromOBJECTID(inTable, inQuery);
        }

        /// <summary>
        /// An ID calculator only for project GDB main act. table ids.
        /// </summary>
        /// <param name="inTable">The person table to get ids information from</param>
        /// <param name="inQuery">A query to refine id, in case.</param>
        /// <param name="prefix"> A prefix to append to the ids.</param>
        /// <returns></returns>
        public static string CalculateMainActicityID(string inTable, string inQuery,string prefix)
        {
            return prefix + "-" + GSC_ProjectEditor.IDs.CalculateSimplementAlphabeticIDFromTable(inTable, true, inQuery);
        }

        /// <summary>
        /// An ID calculator only for project GDB sub act. table ids.
        /// </summary>
        /// <param name="inTable">The person table to get ids information from</param>
        /// <param name="inQuery">A query to refine id, in case.</param>
        /// <param name="prefix"> A prefix to append to the ids.</param>
        /// <returns></returns>
        public static string CalculateSubActivityID(string inTable, string inQuery,string prefix)
        {
            return prefix + GSC_ProjectEditor.IDs.CalculateIDFromCount(inTable, inQuery);
        }

        /// <summary>
        /// Will calculate a new geo event id of the table. 
        /// The returned id comes from row count.
        /// </summary>
        /// <returns></returns>
        public static int CalculateGeoEventIDs(string inTable)
        {
            return GSC_ProjectEditor.IDs.CalculateIDFromCount(inTable, null);
        }

        #endregion
    }
}
