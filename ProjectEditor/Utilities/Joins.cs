using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.GeoDatabaseUI;

namespace GSC_ProjectEditor
{
    public class Joins
    {
        /// <summary>
        /// Will verify if an input layer has a join with an indirect verification to its field.
        /// </summary>
        /// <param name="inputFeatureLayer">The layer to test for a join</param>
        /// <param name="fieldName">The field name with which a test can be accomplished</param>
        /// <returns></returns>
        public static Boolean HasJoin(IFeatureLayer inputFeatureLayer, string fieldName)
        {
            //Main Variable
            bool hasJoin = false;

            //Acccess fields from layer
            ILayerFields lFields = inputFeatureLayer as ILayerFields;

            //Validate if a join exists by finding field. A join will rename field like this: FeatureName.FieldName
            if (lFields.FindField(fieldName) == -1)
            {
                hasJoin = true;
            }

            return hasJoin;
        }

        /// <summary>
        /// Will do a join between a table and a feature class
        /// </summary>
        /// <param name="inFC">Input table to add join to</param>
        /// <param name="inTable">Input feature class to get info from</param>
        /// <param name="FCLinkField">Feature class link field</param>
        /// <param name="TableLinkField">Table link field</param>
        public static ITable DoJoinTableFC(ITable inTable, IFeatureClass inFC, string TableLinkField, string FCLinkField, string primeKey, string newName)
        {

            //Get some names
            IDataset inTableDataset = inTable as IDataset;
            string inTableName = inTableDataset.Name;
            IDataset inFCDataset = inFC as IDataset;
            string inFCName = inFCDataset.Name;

            //Create a query definition
            IFeatureWorkspace currentFeatWorkspace = inFC.FeatureDataset.Workspace as IFeatureWorkspace;
            IQueryDef2 qDef = currentFeatWorkspace.CreateQueryDef() as IQueryDef2;

            //Add the tables to join
            qDef.Tables = inTableName + ", " + inFCName;
            qDef.SubFields = "*";

            //Add query to the object
            qDef.WhereClause = inTableName + "." + TableLinkField + " = " + inFCName + "." + FCLinkField;

            //Create a TableQueryName object to get the output as a layer
            IQueryName2 qName2 = (IQueryName2)new TableQueryNameClass() ;
            qName2.QueryDef = qDef;
            qName2.PrimaryKey = inTableName + "." + primeKey;
            qName2.CopyLocally = true;
  
            //Set the workspace and  name of the new querytable
            IDataset ds = inFC.FeatureDataset.Workspace as IDataset;
            IWorkspaceName workName2 = ds.FullName as IWorkspaceName;

            IDatasetName dName2 = qName2 as IDatasetName;
            dName2.WorkspaceName = workName2;
            dName2.Name = inTableName + "_" + inFCName;//newName;

            //Open the virtual table
            IName name = qName2 as IName;
            ITable outputJoinedTable = (ITable)name.Open();

            return outputJoinedTable;
        }

        /// <summary>
        /// Will do a join between a table and a table
        /// </summary>
        /// <param name="inFC">Input to and from table in this order [to, from]</param>
        /// <param name="inTable">Input to and from table field as keys in this order [to, from]</param>
        /// <param name="FCLinkField">From table link field</param>
        /// <param name="TableLinkField">To Table link field</param>
        public static ITable DoJoinTableTable(Tuple<ITable, ITable> toFromTables, Tuple<string, string> toFromTableLinkFields, string newName)
        {

            //Get some names
            IDataset toTableDataset = toFromTables.Item1 as IDataset;
            string toTableName = toTableDataset.Name;
            IDataset fromTableDataset = toFromTables.Item2 as IDataset;
            string fromTableName = fromTableDataset.Name;

            //Create a query definition
            IFeatureWorkspace currentFeatWorkspace = fromTableDataset.Workspace as IFeatureWorkspace;
            IQueryDef2 qDef = currentFeatWorkspace.CreateQueryDef() as IQueryDef2;

            //Add the tables to join
            qDef.Tables = toTableName + ", " + fromTableName;
            qDef.SubFields = "*";

            //Add query to the object
            qDef.WhereClause = toTableName + "." + toFromTableLinkFields.Item1 + " = " + fromTableName + "." + toFromTableLinkFields.Item2;


            //Create a TableQueryName object to get the output as a layer
            IQueryName2 qName2 = (IQueryName2)new TableQueryNameClass();
            qName2.QueryDef = qDef;
            qName2.PrimaryKey = toTableName + "." + toFromTableLinkFields.Item1;
            qName2.CopyLocally = true;

            //Set the workspace and  name of the new querytable
            IDataset ds = fromTableDataset.Workspace as IDataset;
            IWorkspaceName workName2 = ds.FullName as IWorkspaceName;

            IDatasetName dName2 = qName2 as IDatasetName;
            dName2.WorkspaceName = workName2;
            dName2.Name = newName;

            //Open the virtual table
            IName name = qName2 as IName;
            ITable outputJoinedTable = (ITable)name.Open();

            return outputJoinedTable;
        }

        /// <summary>
        /// Will remove any joins inside a feature layer
        /// </summary>
        /// <param name="joinedLayer">The feature layer to remove join from</param>
        public static void RemoveAllJoins(ESRI.ArcGIS.Carto.IFeatureLayer joinedLayer)
        {
            //Cast relationship inside laer
            ESRI.ArcGIS.Carto.IDisplayRelationshipClass drClass = (ESRI.ArcGIS.Carto.IDisplayRelationshipClass)joinedLayer;
            ESRI.ArcGIS.Geodatabase.IRelationshipClass rClass = drClass.RelationshipClass;

            if (rClass !=null)
            {
                //Get origin and destination classes of relationship
                IObjectClass originClass = rClass.OriginClass;
                IObjectClass destinationClass = rClass.DestinationClass;

                //Set join to null
                drClass.DisplayRelationshipClass(null, esriJoinType.esriLeftInnerJoin);

                System.Runtime.InteropServices.Marshal.ReleaseComObject(originClass);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(destinationClass);

                //Release classes
                System.Runtime.InteropServices.Marshal.ReleaseComObject(rClass);

            }
        }

        /// <summary>
        /// Will add a join between two object class (feature class,  tables, etc.) with a left inner join
        /// </summary>
        /// <param name="inputLayer">The layer to attached a table to</param>
        /// <param name="inputJoinField">The prime key field from layer</param>
        /// <param name="joinTable">The table to join to the layer</param>
        /// <param name="outputJoinField">The prime key field of the table</param>
        /// <param name="cardinality">the cardinality of the relation for the join</param>
        public static void AddJoins(IFeatureLayer origineClass, string inputJoinField, IObjectClass foreignClass, string outputJoinField, esriRelCardinality cardinality)
        {
            // Build a memory relationship class. 
            Type memRelClassFactoryType = Type.GetTypeFromProgID("esriGeodatabase.MemoryRelationshipClassFactory");
            IMemoryRelationshipClassFactory relationshipFactory = (IMemoryRelationshipClassFactory)Activator.CreateInstance(memRelClassFactoryType);
            string joinName = "Z" + Guid.NewGuid().ToString().Substring(0,8);
            IRelationshipClass rClass = relationshipFactory.Open(joinName, origineClass.FeatureClass as IObjectClass, inputJoinField, foreignClass, outputJoinField, "ForwardPath", "BackwardPath", cardinality);
            ESRI.ArcGIS.Carto.IDisplayRelationshipClass drClass = ((ESRI.ArcGIS.Carto.IDisplayRelationshipClass)origineClass);
            drClass.DisplayRelationshipClass(rClass, esriJoinType.esriLeftInnerJoin);

            //Release classes
            System.Runtime.InteropServices.Marshal.ReleaseComObject(rClass);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(relationshipFactory);

        }

        /// <summary>
        /// From a relation name, a workspace and an a class (feature class, table, etc.) a join will be displayed inside arc map.
        /// </summary>
        /// <param name="relationshipName">The relation name</param>
        /// <param name="relationWorkspace">The workspace to find the relation in</param>
        /// <param name="featLayerToDisplayJoin">The class, needs to be inside relation, that will have a join displayed on-screen</param>
        public static void AddJoinsFromExistingRelationship(string relationshipName, IWorkspace relationWorkspace, IFeatureLayer featLayerToDisplayJoin)
        {
            //Access feature workspace from input workspace
            IFeatureWorkspace featWork = relationWorkspace as IFeatureWorkspace;
            
            //Acess relationship from feature workspace
            IRelationshipClass relClass = featWork.OpenRelationshipClass(relationshipName);

            //Add the join
            IDisplayRelationshipClass displayRel = ((IDisplayRelationshipClass)featLayerToDisplayJoin);
            displayRel.DisplayRelationshipClass(relClass, esriJoinType.esriLeftInnerJoin);

        }

    }
}
