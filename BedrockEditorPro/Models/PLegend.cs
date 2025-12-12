using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Core;
using BedrockEditorPro.Utilities;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public string GISDisplay { get; set; }

        [Column(Constants.DatabaseFields.LegendItemType)]
        public string Element { get; set; }

        [Column(Constants.DatabaseFields.LegendSymbol)]
        public string Style1 { get; set; }

        [Column(Constants.DatabaseFields.LegendSymbol2)]
        public string Style2 { get; set; }

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
        public int Column { get; set; }

        [Column(Constants.DatabaseFields.LegendOrder)]
        public double Order { get; set; }

        [Column(Constants.DatabaseFields.LegendDescription)]
        public string Description { get; set; }

        [Column(Constants.DatabaseFields.LegendGeolRank)]
        public string GeolRank { get; set; }

        [Column(Constants.DatabaseFields.LegendOverprint)]
        public string Overprint { get; set; }


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

        /// <summary>
        ///Will prepare the model so it can be ready for inserting
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
                        if (item.GetValue(this) == null || item.GetValue(this).ToString() == string.Empty)
                        {
                            valueDictionary[fieldName] = null;
                        }
                        else
                        {
                            valueDictionary[fieldName] = item.GetValue(this);
                        }
                        
                    }

                }

                return valueDictionary;
            }
            set { }
        }

        public List<string> LineElements = new List<string>() { "LINE", "TWOSIDE", "TWOSIDE_FLOW", "WAVE", "BEACH", "DUNES", "LANDSLIDE", "MORAINES", "TWOSIDE_FLIP" };
        public List<string> MarkerElements = new List<string>() { "POINT_CC", "POINT_CC_45", "POINT_LC_45" };
        public List<string> UnitElements = new List<string>() { "UNIT_BOX", "UNIT_SPLIT", "UNIT_PARENT", "UNIT_CHILD", "UNIT_CHILD_LINE", "UNIT_LINE", "UNIT_INDENT", "UNIT_INDENT2", "OVERLAY", "BLOB" };
        public List<string> OtherElements = new List<string>() { "HEADING1", "HEADING2", "HEADING3", "HEADING4", "HEADING5", "NOTE", "TOP_NOTE", "ANNO_BRACKET", "ANNO_BREAK", "L_BRACKET_L", "L_BRACKET_U", "R_BRACKET_L", "R_BRACKET_U", "BREAK" };
    }
}
