using BedrockEditorPro.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace BedrockEditorPro.Models
{
    [Table(Constants.Database.FStudyArea)]
    public class PStudyArea
    {

        [Column(Constants.DatabaseFields.FStudyAreaRelatedID), PrimaryKey]
        public int RelatedID { get; set; }

        [Column(Constants.DatabaseFields.FStudyAreaAbbr), PrimaryKey]
        public string Abbreviation { get; set; }

        [Column(Constants.DatabaseFields.FStudyAreaRemarks), PrimaryKey]
        public string Remarks { get; set; }

        /// <summary>
        /// A list of all possible fields
        /// </summary>
        [Ignore]
        public Dictionary<double, List<string>> getFieldList
        {
            get
            {

                //Create a new list of all current columns in current class. This will act as the most recent
                //version of the class
                Dictionary<double, List<string>> fieldList = new Dictionary<double, List<string>>();
                List<string> defaultFieldList = new List<string>();

                foreach (System.Reflection.PropertyInfo item in this.GetType().GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(ColumnAttribute))).ToList())
                {
                    if (item.CustomAttributes.First().ConstructorArguments.Count() > 0)
                    {
                        defaultFieldList.Add(item.CustomAttributes.First().ConstructorArguments[0].ToString().Replace("\\", "").Replace("\"", ""));
                    }

                }

                fieldList[Constants.Database.CurrentDBVersion] = defaultFieldList;

                return fieldList;
            }
            set { }
        }

        /// <summary>
        /// A list of all possible fields
        /// </summary>
        [Ignore]
        public Dictionary<string, object> getModelReadyForInsert
        {
            get
            {

                Dictionary<string, object> valueDictionary = new Dictionary<string, object>();

                foreach (System.Reflection.PropertyInfo item in this.GetType().GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(ColumnAttribute))).ToList())
                {
                    if (item.CustomAttributes.First().ConstructorArguments.Count() > 0)
                    {
                        string fieldName = item.CustomAttributes.First().ConstructorArguments[0].ToString().Replace("\\", "").Replace("\"", "");
                        valueDictionary[fieldName] = item.GetValue(this);
                    }

                }
                
                return valueDictionary;
            }
            set { }
        }

    }
}
