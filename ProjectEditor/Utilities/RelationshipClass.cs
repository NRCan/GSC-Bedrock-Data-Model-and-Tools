using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;

namespace GSC_ProjectEditor
{
    public class RelationshipClass
    {
        /// <summary>
        /// Will return a list of relation ship class object from a given workspace
        /// </summary>
        /// <param name="inWorkspace">The workspace to get the relation list from</param>
        /// <returns></returns>
        public static List<IRelationshipClass> GetListOfRelationFromWorkspace(IWorkspace inWorkspace, IFeatureDataset inputFeatureDataset = null)
        {
            //Variable
            List<IRelationshipClass> listOfRelation = new List<IRelationshipClass>();
            IEnumDataset fcEnum;

            if (inputFeatureDataset != null)
            {
                //Get a list of all feature classes from workspace
                fcEnum = inputFeatureDataset.Subsets;

            }
            else
            {
                //Get a list of all feature classes from workspace
                fcEnum = inWorkspace.get_Datasets(esriDatasetType.esriDTRelationshipClass);
            }

            ////Get a list of all feature classes from workspace
            //fcEnum = inWorkspace.get_Datasets(esriDatasetType.esriDTRelationshipClass);

            //Convert enum to list
            IDataset currentDS = fcEnum.Next();
            while (currentDS != null)
            {
                if (currentDS.Type == esriDatasetType.esriDTRelationshipClass)
                {
                    listOfRelation.Add(currentDS as IRelationshipClass);
                }

                currentDS = fcEnum.Next();
            }

            return listOfRelation;
        }

        /// <summary>
        /// From a given list of relations, will convert them from simple to composite
        /// </summary>
        /// <param name="relationsToConvert"></param>
        public static void ConvertSimpleToCompositeRelations(List<IRelationshipClass> relationsToConvert, bool isComposite)
        {
            //Update relations to be composite
            foreach (IRelationshipClass rel in relationsToConvert)
            {
                IRelClassSchemaEdit editRelation = rel as IRelClassSchemaEdit;
                try
                {
                    
                    editRelation.AlterIsComposite(isComposite);
                }
                catch (Exception e)
                {
                    Messages.ShowGenericErrorMessage("relation error: " + e.Message);
                }

            }
        }

        /// <summary>
        /// From a given workspace will return a list of all field names involved in relations.
        /// </summary>
        /// <param name="inWorkspace">The workspace to get a list from.</param>
        /// <returns></returns>
        public static List<string> GetListOfRelationFieldKeysFromWorkspace(IWorkspace inWorkspace)
        {
            //Variables
            List<string> fieldKeysList = new List<string>();

            //Get a list of relations
            List<IRelationshipClass> listOfRelations = GetListOfRelationFromWorkspace(inWorkspace);

            foreach (IRelationshipClass rels in listOfRelations)
            {
                //Fill key list
                if (rels.OriginForeignKey != null && !fieldKeysList.Contains(rels.OriginForeignKey))
	            {
		            fieldKeysList.Add(rels.OriginForeignKey);
	            }
                if (rels.OriginPrimaryKey != null && !fieldKeysList.Contains(rels.OriginPrimaryKey))
                {
                    fieldKeysList.Add(rels.OriginPrimaryKey);
                }
                if (rels.DestinationPrimaryKey != null && !fieldKeysList.Contains(rels.DestinationPrimaryKey))
                {
                    fieldKeysList.Add(rels.DestinationPrimaryKey);
                }
                if (rels.DestinationForeignKey != null && !fieldKeysList.Contains(rels.DestinationForeignKey))
                {
                    fieldKeysList.Add(rels.DestinationForeignKey);
                }

            }

            return fieldKeysList;
        }

        /// <summary>
        /// Will recreate a new relationship class into a given workspace from an original existing one in
        /// another workspace without duplication all the features or tables involved in this relation.
        /// </summary>
        /// <param name="outworkspace"></param>
        /// <param name="inTableFrom"></param>
        /// <param name="inTableTo"></param>
        /// <param name="inRelation"></param>
        public static void CreateRelationshipFromExisting(IWorkspace outworkspace, IObjectClass inTableFrom, IObjectClass inTableTo, IRelationshipClass inRelation)
        {
            //Get original name
            IDataset inDataset = inRelation as IDataset;
            string relationName = inDataset.Name;

            IFeatureWorkspace featWorkspace = outworkspace as IFeatureWorkspace;
            IRelationshipClass cartoRelClass = featWorkspace.CreateRelationshipClass(relationName,
                inTableFrom, inTableTo,
                inRelation.ForwardPathLabel,
                inRelation.BackwardPathLabel,
                inRelation.Cardinality,
                inRelation.Notification,
                inRelation.IsComposite,
                inRelation.IsAttributed, null,
                inRelation.OriginPrimaryKey,
                "",
                inRelation.OriginForeignKey,
                "");

        }

    }
}
