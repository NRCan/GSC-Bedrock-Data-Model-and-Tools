using BedrockEditorPro.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using ArcGIS.Core.Geometry;

namespace BedrockEditorPro.Models
{
    [Table(Constants.Database.FStudyArea)]
    public class PStudyArea
    {

        [Column(Constants.DatabaseFields.FStudyAreaRelatedID), PrimaryKey]
        public int RelatedID { get; set; }

        [Column(Constants.DatabaseFields.FStudyAreaEast)]
        public double East { get; set; }

        [Column(Constants.DatabaseFields.FStudyAreaWest)]
        public double West { get; set; }

        [Column(Constants.DatabaseFields.FStudyAreaNorth)]
        public double North { get; set; }

        [Column(Constants.DatabaseFields.FStudyAreaSouth)]
        public double South { get; set; }

        [Column(Constants.DatabaseFields.FStudyAreaAbbr)]
        public string Abbreviation { get; set; }

        [Column(Constants.DatabaseFields.FStudyAreaRemarks)]
        public string Remarks { get; set; }

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

        /// <summary>
        /// A list of all possible fields
        /// </summary>
        [Ignore]
        public IEnumerable<Coordinate3D> getCoordinatesFromFields
        {
            get
            {

                IEnumerable <Coordinate3D> polygonCoordinates = new List<Coordinate3D>
                {
                    new Coordinate3D(West, South, 0),
                    new Coordinate3D(West, North, 0),
                    new Coordinate3D(East, North, 0),
                    new Coordinate3D(East, South, 0),
                    new Coordinate3D(West, South, 0)
                };

                return polygonCoordinates;
            }
            set { }
        }


    }
}
