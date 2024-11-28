using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ESRI.ArcGIS.Geodatabase;

namespace GSC_ProjectEditor
{
    public class Domains
    {
        #region GET methods

        /// <summary>
        /// Retrieve a value list from a domain of project database
        /// </summary>
        /// <param name="domName">Reference to domain name, to get list from</param>
        /// <returns></returns>
        public static List<string> GetDomValueList(string domName)
        {
            //Variables
            List<string> valueList = new List<string>(); //Empty list to contain roles

            //Get workspace object
            string inputWorkspacePath = GSC_ProjectEditor.Workspace.GetDBPath();//Utilities.globalVariables.database.path;
            IWorkspace inputWorkspace = GSC_ProjectEditor.Workspace.AccessWorkspace(inputWorkspacePath);

            //Get domain coded value object
            try
            {
                ICodedValueDomain codesDom = AccessDomCodes(inputWorkspace, domName);

                //Iterate through list of values within domain object
                int[] numberOfCodes = Enumerable.Range(0, codesDom.CodeCount).ToArray(); //Create an array of exact same lenght as number of values in domain.
                foreach (int index in numberOfCodes)
                {
                    //Add code values description to list
                    valueList.Add(codesDom.get_Name(index).ToString());
                }
            }
            catch
            {
                //Domain could be missing, skip if it crashes, the returned list will be empty
            }

            return valueList;

        }

        /// <summary>
        /// Retrieve a complete code and description dictionary from a domain of project database, specify order Key = Code or Key = Description
        /// </summary>
        /// <param name="domName">Reference domain name to get dico from</param>
        /// <param name="KeyType">Enter desired type of key, if "Code" domain code = key, if "Description" domain desc. = key</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetDomDico(string domName, string KeyType)
        {
            //Variables
            Dictionary<string, string> domDico = new Dictionary<string, string>(); //Empty dico to contain complete domain

            //Get workspace object
            string inputWorkspacePath = GSC_ProjectEditor.Workspace.GetDBPath();//Utilities.globalVariables.database.path;
            IWorkspace inputWorkspace = GSC_ProjectEditor.Workspace.AccessWorkspace(inputWorkspacePath);



            return GetDomDicoFromWorkspace(inputWorkspace, domName, KeyType);

        }

        /// <summary>
        /// Retrieve a complete code and description dictionary from a domain of project database, specify order Key = Code or Key = Description
        /// </summary>
        /// <param name="domName">Reference domain name to get dico from</param>
        /// <param name="KeyType">Enter desired type of key, if "Code" domain code = key, if "Description" domain desc. = key</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetDomDicoFromWorkspace(IWorkspace inWork, string domName, string KeyType)
        {
            //Variables
            Dictionary<string, string> domDico = new Dictionary<string, string>(); //Empty dico to contain complete domain

            //Get domain coded value object
            try
            {
                ICodedValueDomain codesDom = AccessDomCodes(inWork, domName);

                //Iterate through list of values within domain object
                int[] numberOfCodes = Enumerable.Range(0, codesDom.CodeCount).ToArray(); //Create an array of exact same lenght as number of values in domain.
                foreach (int index in numberOfCodes)
                {
                    //Add code values description to list
                    if (KeyType == "Code")
                    {
                        domDico[codesDom.get_Value(index).ToString()] = codesDom.get_Name(index).ToString();
                    }
                    else if (KeyType == "Description")
                    {
                        domDico[codesDom.get_Name(index).ToString()] = codesDom.get_Value(index).ToString();
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
        /// Will return a domain name from a given subtype and field name
        /// </summary>
        /// <param name="inputFeature">Reference feature class to get domain name from</param>
        /// <param name="inputSubtypeCode">Reference subtype code, to get domain from</param>
        /// <param name="inputFieldName">Reference field name to get domain from</param>
        /// <returns></returns>
        public static string GetSubDomName(string inputFeature, int inputSubtypeCode, string inputFieldName)
        {
            //Variables
            string domainName = string.Empty;

            //Access subtypes
            ISubtypes getSubtype = GSC_ProjectEditor.Subtypes.AccessSubtypes(inputFeature);

            //Access domain
            IDomain getDom = getSubtype.get_Domain(inputSubtypeCode, inputFieldName);
            try
            {
                domainName = getDom.Name;
            }
            catch (Exception)
            {

            }
            
            return domainName;
        }

        /// <summary>
        /// Will return a filtered string, from a given look-up table name, to create a new domain name. lutEarthmatCompositional --> F_Compositional
        /// </summary>
        /// <param name="inDomName">The input look-up table name</param>
        /// <returns></returns>
        public static string GetFilteredGanfeldDomName(string inDomName)
        {
            //Variable
            string newName = "";
            List<string> shapeNames = new List<string>();

            //Build list of shapefile names
            shapeNames.Add(Constants.GanfeldShapefiles.shpbiogeo.ToLower());
            shapeNames.Add(Constants.GanfeldShapefiles.shpEarthMath.ToLower());
            shapeNames.Add(Constants.GanfeldShapefiles.shpEnviron.ToLower());
            shapeNames.Add(Constants.GanfeldShapefiles.shpLinework.ToLower());
            shapeNames.Add(Constants.GanfeldShapefiles.shpMA.ToLower());
            shapeNames.Add(Constants.GanfeldShapefiles.shpMetadata.ToLower());
            shapeNames.Add(Constants.GanfeldShapefiles.shpMineral.ToLower());
            shapeNames.Add(Constants.GanfeldShapefiles.shpPFlow.ToLower());
            shapeNames.Add(Constants.GanfeldShapefiles.shpPhoto.ToLower());
            shapeNames.Add(Constants.GanfeldShapefiles.shpSample.ToLower());
            shapeNames.Add(Constants.GanfeldShapefiles.shpSoilPro.ToLower());
            shapeNames.Add(Constants.GanfeldShapefiles.shpStation.ToLower());
            shapeNames.Add(Constants.GanfeldShapefiles.shpStruc.ToLower());
            shapeNames.Add(Constants.GanfeldShapefiles.shpTraverses.ToLower());

            //Lower case everything
            newName = inDomName.ToLower();

            ////Get rid of the possible dbf extension
            newName = Path.GetFileNameWithoutExtension(newName);

            //Get look-up table prefix out of the string
            int lutPrefixCount = Constants.GanfeldLookUpTables.lutPrefix.Count();
            if (newName.Contains(Constants.GanfeldLookUpTables.lutPrefix))
            {
                newName = newName.Remove(0, lutPrefixCount);
            }

            //Get associated table out of the string
            foreach (string shp in shapeNames)
            {
                //Find proper keyword
                if (newName.Contains(shp))
                {
                    //Extra condition, prevents MA keyword from being removed in earthmat
                    if (newName[0] == shp[0] && newName[1] == shp[1])
                    {
                        newName = newName.Replace(shp, "");

                        //Extra condition for "Type" SampleType and StrucType or else they will be mixed together
                        if (shp == Constants.GanfeldShapefiles.shpStruc.ToLower())
                        {
                            newName = "STRUC" + newName;
                        }

                        break;
                    }

                }
            }

            //Make all upper case 
            newName = newName.ToUpper();

            //Add proper keyword in new dom name, to quickly associate domain with field data
            newName = Constants.DatabaseGanfeldDomains.ProjectPrefix + newName;

            return newName;

        }

        /// <summary>
        /// Will return a complete list of all domains within a workspace
        /// </summary>
        /// <param name="inputWorkspace">The workspace to retrieve the list of domains</param>
        public static List<IDomain> GetDomainList(IWorkspace inputWorkspace)
        {
            //Variables
            List<IDomain> domainList = new List<IDomain>();

            //Make sure nothing exists
            IWorkspaceDomains iWD = null;

            //Create domain workspace factory from input workspace
            iWD = inputWorkspace as IWorkspaceDomains;

            //Get list of domains
            IEnumDomain domList = iWD.Domains;
            IDomain currentDom = domList.Next();

            //Iterate through list to retrieve names
            while (currentDom != null)
            {
                domainList.Add(currentDom);

                currentDom = domList.Next();
            }

            return domainList;
        }

        #endregion

        #region CREATE methods

        /// <summary>
        /// Will add a new domain into project database workspace. Will first check if it exists.
        /// </summary>
        public static IDomain CreateCodedValueDomain(string domName, string domDescription, esriFieldType domFieldType)
        {
            //Get workspace object
            string inputWorkspacePath = GSC_ProjectEditor.Workspace.GetDBPath();//Utilities.globalVariables.database.path;
            IWorkspace inputWorkspace = GSC_ProjectEditor.Workspace.AccessWorkspace(inputWorkspacePath);

            //Get second domain workspace (to have possibility to alter domain)
            IWorkspaceDomains2 updateWorkspaceDomain = inputWorkspace as IWorkspaceDomains2;

            //Verify existance
            IDomain newDomain;
            if (updateWorkspaceDomain.get_DomainByName(domName) == null)
            {
                //Create new domain
                newDomain = new CodedValueDomainClass();
                newDomain.Description = domDescription;
                newDomain.Name = domName;
                newDomain.FieldType = domFieldType;

                //Add new domain to workspace
                updateWorkspaceDomain.AddDomain(newDomain);
                updateWorkspaceDomain.AlterDomain(newDomain);
            }
            else
            {
                newDomain = updateWorkspaceDomain.get_DomainByName(domName);
            }

            return newDomain;
        }

        #endregion

        #region ADD methods

        /// <summary>
        /// Adding values to domains, for project database
        /// </summary>
        /// <param name="inDom">Reference domain name to add value in</param>
        /// <param name="inCode">Domain code to add</param>
        /// <param name="inDesc">Domain description associated with code</param>
        public static void AddDomainValue(string inDom, string inCode, string inDesc)
        {
            //Create a dictionary of input code and description
            Dictionary<string, string> inputCodeDesc = new Dictionary<string, string>();
            inputCodeDesc[inCode] = inDesc;

            //Call proper method
            AddDomainValueDictionary(inDom, inputCodeDesc);

        }

        /// <summary>
        /// Adding values to domains, for project database
        /// </summary>
        /// <param name="inDom">Reference domain name to add value in</param>
        /// <param name="inCode">Domain code to add</param>
        /// <param name="inDesc">Domain description associated with code</param>
        public static void AddDomainValueFromWorkspace(IWorkspace inWorkspace, string inDom, string inCode, string inDesc)
        {
            //Create a dictionary of input code and description
            Dictionary<string, string> inputCodeDesc = new Dictionary<string, string>();
            inputCodeDesc[inCode] = inDesc;

            //Call proper method
            AddDomainValueDictionaryFromWorkspace(inWorkspace, inDom, inputCodeDesc);

        }

        /// <summary>
        /// Will add values to a given domain, from an input dictionary. For project database only.
        /// </summary>
        /// <param name="inDom">The domain to add values to</param>
        /// <param name="CodeDescription">The dictionary to contain as keys the domain code and values the domain description</param>
        public static void AddDomainValueDictionary(string inDom, Dictionary<string, string> CodeDescription)
        {
            try
            {
                //Get workspace object
                string inputWorkspacePath = GSC_ProjectEditor.Workspace.GetDBPath();//Utilities.globalVariables.database.path;
                IWorkspace inputWorkspace = GSC_ProjectEditor.Workspace.AccessWorkspace(inputWorkspacePath);

                AddDomainValueDictionaryFromWorkspace(inputWorkspace, inDom, CodeDescription);
            }
            catch (Exception addDomainValueExcept)
            {
                int lineNumber = Exceptions.LineNumber(addDomainValueExcept);
                MessageBox.Show("addDomainValueExcept (" + lineNumber.ToString() + "): " + addDomainValueExcept.Message);
            }
        }

        /// <summary>
        /// Will add values to a given domain, from an input dictionary.
        /// </summary>
        /// <param name="inWorkspace">The workspace in witch the new dictionary will be added as a domain.</param>
        /// <param name="inDom">The domain to add values to</param>
        /// <param name="CodeDescription">The dictionary to contain as keys the domain code and values the domain description</param>
        public static void AddDomainValueDictionaryFromWorkspace(IWorkspace inWorkspace, string inDom, Dictionary<string, string> CodeDescription)
        {
            try
            {

                //Get second domain workspace (to have possibility to alter domain)
                IWorkspaceDomains iWD = null;
                iWD = inWorkspace as IWorkspaceDomains;

                //Get a specific domain from workspace
                IDomain dom2 = iWD.get_DomainByName(inDom);

                //Get the coded value domain
                ICodedValueDomain codesDom = dom2 as ICodedValueDomain;

                //Add values
                foreach (KeyValuePair<string, string> codeDesc in CodeDescription)
                {
                    try
                    {
                        codesDom.AddCode(codeDesc.Key, codeDesc.Value);
                    }
                    catch
                    {
                    }

                }

                IWorkspaceDomains2 iWD2 = iWD as IWorkspaceDomains2;
                iWD2.AlterDomain(dom2);
            }
            catch (Exception adDomainValueDictionaryFromWorkspaceExcept)
            {
                MessageBox.Show("adDomainValueDictionaryFromWorkspaceExcept:" + adDomainValueDictionaryFromWorkspaceExcept.StackTrace);
            }
        }

        #endregion

        #region UPDATE methods

        /// <summary>
        /// Modify an existing domain description, for project database
        /// </summary>
        /// <param name="updateDomname">The domain name to modify</param>
        /// <param name="updateDomCode">The domain code to modify description from</param>
        /// <param name="updateDomDescription">The new domain description</param>
        public static void UpdateDomainDescription(string updateDomname, string updateDomCode, string updateDomDescription)
        {
            //Create a dictionary with input values
            Dictionary<string, string> updateCodeDesc = new Dictionary<string, string>();
            updateCodeDesc[updateDomCode] = updateDomDescription;

            UpdateDomainDescriptionFromDico(updateDomname, updateCodeDesc);

        }

        /// <summary>
        /// Modify an existing domain description, for project database
        /// </summary>
        /// <param name="updateDomname">The domain name to modify</param>
        /// <param name="updateDomCode">The domain code to modify description from</param>
        /// <param name="updateDomDescription">The new domain description</param>
        public static void UpdateDomainDescriptionFromWorkspace(IWorkspace inputWorkspace, string updateDomname, string updateDomCode, string updateDomDescription)
        {
            //Create a dictionary with input values
            Dictionary<string, string> updateCodeDesc = new Dictionary<string, string>();
            updateCodeDesc[updateDomCode] = updateDomDescription;

            UpdateDomainDescriptionFromDicoFromWorkspace(inputWorkspace, updateDomname, updateCodeDesc);

        }

        /// <summary>
        /// Modify an existing domain description, for project database, and will also add new code if needed.
        /// </summary>
        /// <param name="updateDomname">The domain name to modify</param>
        /// <param name="CodeDescription">The domain code desription dictionary to modify description from</param>
        public static void UpdateDomainDescriptionFromDico(string inDom, Dictionary<string, string> CodeDescription)
        {

            //Get workspace object
            string inputWorkspacePath = GSC_ProjectEditor.Workspace.GetDBPath();//Utilities.globalVariables.database.path;
            IWorkspace inputWorkspace = GSC_ProjectEditor.Workspace.AccessWorkspace(inputWorkspacePath);

            UpdateDomainDescriptionFromDicoFromWorkspace(inputWorkspace, inDom, CodeDescription);
        }

        /// <summary>
        /// Modify an existing domain description, for project database, and will also add new code if needed.
        /// </summary>
        /// <param name="updateDomname">The domain name to modify</param>
        /// <param name="CodeDescription">The domain code desription dictionary to modify description from</param>
        public static void UpdateDomainDescriptionFromDicoFromWorkspace(IWorkspace inputWorkspace, string inDom, Dictionary<string, string> CodeDescription)
        {

            //Get second domain workspace (to have possibility to alter domain)
            IWorkspaceDomains2 updateWorkspaceDomain = inputWorkspace as IWorkspaceDomains2;

            //Get wanted domain to update
            ICodedValueDomain2 dom2Update = updateWorkspaceDomain.get_DomainByName(inDom) as ICodedValueDomain2;

            //Get wanted codedvalue domain to update
            int domCount = dom2Update.CodeCount;
            for (int iDom = 0; iDom < domCount; iDom++)
            {
                //keep actual domain code
                string currentDomCode = dom2Update.Value[iDom].ToString();

                if (CodeDescription.ContainsKey(dom2Update.Value[iDom].ToString()))
                {
                    //Delete --> This is the only way, there is no object read-write accessible within ICodedValueDomain...
                    dom2Update.DeleteCode(currentDomCode);

                    //Re-add
                    dom2Update.AddCode(currentDomCode, CodeDescription[currentDomCode]);

                    //Sort
                    dom2Update.SortByValue(false);

                    //Update
                    updateWorkspaceDomain.AlterDomain(dom2Update as IDomain);

                    //Remove from dictionary
                    CodeDescription.Remove(currentDomCode);
                }

            }

            //Add new codes, if there is some
            foreach (KeyValuePair<string, string> kv in CodeDescription)
            {
                //Re-add
                dom2Update.AddCode(kv.Key, kv.Value);

                //Sort
                dom2Update.SortByValue(false);

                //Update
                updateWorkspaceDomain.AlterDomain(dom2Update as IDomain);
            }
        }

        #endregion

        #region ACCESS methods

        /// <summary>
        /// Create a domain workspace factory to access domain and coded values
        /// </summary>
        /// <param name="inputWorkspaceFactory">A workspace factory reference</param>
        /// <param name="DomName">The domain name to get object ICodedValueDomain object from</param>
        /// <returns></returns>
        public static ICodedValueDomain AccessDomCodes(IWorkspace inputWorkspaceFactory, string DomName)
        {
            //Make sure nothing exists
            IWorkspaceDomains iWD = null;

            //Create domain workspace factory from input workspace
            iWD = inputWorkspaceFactory as IWorkspaceDomains;

            //Get domain and associated coded values
            IDomain dom = iWD.get_DomainByName(DomName);
            ICodedValueDomain domCodes = dom as ICodedValueDomain;

            return domCodes;

        }

        #endregion

        #region ASSIGNATION methods and OTHER

        /// <summary>
        /// Will assign a given domain to a certain field within a table or feature class, for project database only.
        /// </summary>
        /// <param name="FieldName">The field name to attach a domain to</param>
        /// <param name="dataName">The table or feature name to attach domain to</param>
        /// <param name="inputDomain">The domain object that will be attached to the field</param>
        public static void AssignDomainToField(string inputFieldName, IObjectClass inputObject, IDomain inputDomain)
        {
            //Cast the feature class to a SchemaLock and ClassSchemaEdit objects
            ISchemaLock schemaLock = inputObject as ISchemaLock;
            IClassSchemaEdit classSchema = inputObject as IClassSchemaEdit;

            try
            {
                //Lock the object
                schemaLock.ChangeSchemaLock(esriSchemaLock.esriExclusiveSchemaLock);

                //Alter domain
                classSchema.AlterDomain(inputFieldName, inputDomain);
            }
            catch (Exception assignDomainToFieldExcept)
            {
                int lineNumber = Exceptions.LineNumber(assignDomainToFieldExcept);
                MessageBox.Show("ERROR assignDomainToField (" + lineNumber.ToString() + "): " + assignDomainToFieldExcept.Message);
            }
            finally
            {
                //Unlock
                schemaLock.ChangeSchemaLock(esriSchemaLock.esriSharedSchemaLock);
            }

        }

        /// <summary>
        /// Will return true if a given domain names already exists in project database workspace
        /// </summary>
        /// <param name="inDom">Domain name to search for</param>
        /// <returns></returns>
        public static bool IsDomExisting(string inDom)
        {
            //Variable
            bool exists = false;

            //Get workspace object
            string inputWorkspacePath = GSC_ProjectEditor.Workspace.GetDBPath();//Utilities.globalVariables.database.path;
            IWorkspace inputWorkspace = GSC_ProjectEditor.Workspace.AccessWorkspace(inputWorkspacePath);

            //Get second domain workspace (to have possibility to alter domain)
            IWorkspaceDomains2 updateWorkspaceDomain = inputWorkspace as IWorkspaceDomains2;

            if (updateWorkspaceDomain.get_DomainByName(inDom) != null)
            {
                exists = true;
            }

            return exists;

        }

        #endregion

        #region DELETE methods

        /// <summary>
        /// Remove values to domains, for project database
        /// </summary>
        /// <param name="inDom">Reference domain name to remove value in</param>
        /// <param name="inCode">Domain code to remove</param>
        /// <param name="inDesc">Domain description associated with code</param>
        public static void DeleteDomainValue(string inDom, string inCode)
        {
            //Create a dictionary of input code and description
            List<string> inputCodeDesc = new List<string>();
            inputCodeDesc.Add(inCode);

            //Call proper method
            DeleteDomainValueDictionary(inDom, inputCodeDesc);

        }

        /// <summary>
        /// Will remove values to a given domain, from an input dictionary. For project database only.
        /// </summary>
        /// <param name="inDom">The domain to remove values to</param>
        /// <param name="codeList">The dictionary to contain as keys the domain code and values the domain description</param>
        public static void DeleteDomainValueDictionary(string inDom, List<string> codeList)
        {
            try
            {
                //Get workspace object
                string inputWorkspacePath = GSC_ProjectEditor.Workspace.GetDBPath();//Utilities.globalVariables.database.path;
                IWorkspace inputWorkspace = GSC_ProjectEditor.Workspace.AccessWorkspace(inputWorkspacePath);

                //Get second domain workspace (to have possibility to alter domain)
                IWorkspaceDomains iWD = null;
                iWD = inputWorkspace as IWorkspaceDomains;

                //Get a specific domain from workspace
                IDomain dom2 = iWD.get_DomainByName(inDom);

                //Get the coded value domain
                ICodedValueDomain codesDom = dom2 as ICodedValueDomain;

                //Add values
                foreach (string code in codeList)
                {
                    try
                    {
                        codesDom.DeleteCode(code);
                    }
                    catch
                    {
                    }

                }


                IWorkspaceDomains2 iWD2 = iWD as IWorkspaceDomains2;
                iWD2.AlterDomain(dom2);
            }
            catch (Exception DeleteDomainValueDictionaryExcept)
            {
                int lineNumber = Exceptions.LineNumber(DeleteDomainValueDictionaryExcept);
                MessageBox.Show("DeleteDomainValueDictionaryExcept (" + lineNumber.ToString() + "): " + DeleteDomainValueDictionaryExcept.Message);
            }
        }

        /// <summary>
        /// Will remove values to a given domain, from an input dictionary. For project database only.
        /// </summary>
        /// <param name="inDom">The domain to remove values to</param>
        /// <param name="codeList">The dictionary to contain as keys the domain code and values the domain description</param>
        public static void DeleteAllDomainValue(string inDom)
        {
            try
            {
                //Get workspace object
                string inputWorkspacePath = GSC_ProjectEditor.Workspace.GetDBPath();//Utilities.globalVariables.database.path;
                IWorkspace inputWorkspace = GSC_ProjectEditor.Workspace.AccessWorkspace(inputWorkspacePath);

                //Get second domain workspace (to have possibility to alter domain)
                IWorkspaceDomains iWD = null;
                iWD = inputWorkspace as IWorkspaceDomains;

                //Get a specific domain from workspace
                IDomain dom2 = iWD.get_DomainByName(inDom);

                //Get the coded value domain
                ICodedValueDomain codesDom = dom2 as ICodedValueDomain;

                //Add values
                for (int i = 0; i < codesDom.CodeCount; i++)
			    {
			        try
                    {
                        codesDom.DeleteCode(i);
                    }
                    catch
                    {
                    }
			    }

                IWorkspaceDomains2 iWD2 = iWD as IWorkspaceDomains2;
                iWD2.AlterDomain(dom2);
            }
            catch (Exception addDomainValueExcept)
            {
                int lineNumber = Exceptions.LineNumber(addDomainValueExcept);
                MessageBox.Show("addDomainValueExcept (" + lineNumber.ToString() + "): " + addDomainValueExcept.Message);
            }
        }

        #endregion

    }
}
