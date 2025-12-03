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
    [Table(Constants.Database.TLegendGene)]
    public class PLegend
    {
        [Column(Constants.DatabaseFields.LegendLabelID), PrimaryKey]
        public int ItemID { get; set; }

        [Column(Constants.DatabaseFields.LegendGISDisplay)]
        public double GISDisplay { get; set; }

        [Column(Constants.DatabaseFields.LegendItemType)]
        public double Element { get; set; }

        [Column(Constants.DatabaseFields.LegendSymbol)]
        public double Style1 { get; set; }

        [Column(Constants.DatabaseFields.LegendSymbol2)]
        public double Style2 { get; set; }

        [Column(Constants.DatabaseFields.LegendLabel1)]
        public string Label1 { get; set; }

        [Column(Constants.DatabaseFields.LegendLabel1Style)]
        public string Label1Style { get; set; }

        [Column(Constants.DatabaseFields.LegendLabel2)]
        public string Label2 { get; set; }

        [Column(Constants.DatabaseFields.LegendLabel2Style)]
        public string Label2Style { get; set; }

        [Column(Constants.DatabaseFields.LegendHeading)]
        public string Heading { get; set; }

        [Column(Constants.DatabaseFields.LegendColumn)]
        public string Column { get; set; }

        [Column(Constants.DatabaseFields.LegendOrder)]
        public string Order { get; set; }

        [Column(Constants.DatabaseFields.LegendDescription)]
        public string Description { get; set; }

        [Column(Constants.DatabaseFields.LegendGeolRank)]
        public string GeolRank { get; set; }

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

                //Back track to version 2.10
                List<string> legend210 = new List<string>(defaultFieldList);
                legend210.AddRange(defaultFieldList);
                legend210.Remove(Constants.DatabaseFields.LegendItemType);
                legend210.Remove(Constants.DatabaseFields.LegendSymbol);
                legend210.Remove(Constants.DatabaseFields.LegendSymbol2);
                legend210.Remove(Constants.DatabaseFields.LegendLabel1);
                legend210.Remove(Constants.DatabaseFields.LegendLabel1Style);
                legend210.Remove(Constants.DatabaseFields.LegendLabel2);
                legend210.Remove(Constants.DatabaseFields.LegendLabel2Style);
                legend210.Remove(Constants.DatabaseFields.LegendHeading);
                legend210.Remove(Constants.DatabaseFields.LegendColumn);
                legend210.Remove(Constants.DatabaseFields.LegendOrder);
                legend210.Remove(Constants.DatabaseFields.LegendDescription);
                
                legend210.Add(Constants.DatabaseFields.LegendSymbol_190101);
                legend210.Add(Constants.DatabaseFields.LegendSymType_190101);
                legend210.Add(Constants.DatabaseFields.LegendLabelName_190101);
                legend210.Add(Constants.DatabaseFields.LegendMapUnit_190101);
                legend210.Add(Constants.DatabaseFields.LegendAnnotation_190101);
                legend210.Add(Constants.DatabaseFields.LegendOrder_190101);
                legend210.Add(Constants.DatabaseFields.LegendIndentation_190101);
                legend210.Add(Constants.DatabaseFields.LegendItemType_190101);

                fieldList[Constants.Database.DBVersion_210] = legend210;

                return fieldList;
            }
            set { }
        }

    }
}
