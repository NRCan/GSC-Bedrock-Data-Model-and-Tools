using ArcGIS.Core.Geometry;
using BedrockEditorPro.Utilities;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BedrockEditorPro.Models
{
    [Table(Constants.Database.FLabel)]
    public class Labels
    {

        [Column(Constants.DatabaseFields.FLabelID), PrimaryKey]
        public string LabelID { get; set; }

        [Column(Constants.DatabaseFields.SourceID)]
        public string SourceID { get; set; }

        [Column(Constants.DatabaseFields.FLabelGeoEventID)]
        public string GeoEventID { get; set; }

        [Column(Constants.DatabaseFields.FGeopointRemark)]
        public string Remarks { get; set; }

        [Column(Constants.DatabaseFields.LegendSymbol)]
        public string GSCSymbol { get; set; }

        [Column(Constants.DatabaseFields.ETCreatorID)]
        public string CreatorID { get; set; }

        [Column(Constants.DatabaseFields.ETEditorID)]
        public string EditorID { get; set; }

        [Column(Constants.DatabaseFields.ETCreateDate)]
        public string CreateDate { get; set; }

        [Column(Constants.DatabaseFields.ETEditDate)]
        public string EditDate { get; set; }

        //Not a true database field, used for temporary naming
        public string Name { get; set; }

        //Not a true database field, used for temporary naming
        public int OverprintLevel { get; set; }

        [Ignore]
        public Geometry Geometry { get; set; }

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
        /// Will output the database field name associated with the given property name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nameOfProperty"></param>
        /// <returns></returns>
        public string GetPropertyAttributeName(string nameOfProperty)
        {
            string propertyFieldName = string.Empty;

            System.Reflection.PropertyInfo item = this.GetType().GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(ColumnAttribute))
            && prop.Name == nameOfProperty).First();


            if (item != null)
            {
                propertyFieldName = item.CustomAttributes.First().ConstructorArguments[0].ToString().Replace("\\", "").Replace("\"", "");
            }


            return propertyFieldName;
        }

    }
}
