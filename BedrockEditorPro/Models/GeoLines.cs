using ArcGIS.Core.Geometry;
using BedrockEditorPro.Utilities;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BedrockEditorPro.Models
{
    [Table(Constants.Database.FGeoline)]
    public class GeoLines
    {
        [Column(Constants.DatabaseFields.FGeolineID), PrimaryKey]
        public string GeolineID { get; set; }

        [Column(Constants.DatabaseFields.FGeolineSubtype)]
        public int GeolineType { get; set; }

        [Column(Constants.DatabaseFields.FGeolineQualif)]
        public string Qualifier { get; set; }

        [Column(Constants.DatabaseFields.FGeolineConf)]
        public string Confidence { get; set; }

        [Column(Constants.DatabaseFields.FGeolineAtt)]
        public string Attitude { get; set; }

        [Column(Constants.DatabaseFields.FGeolineGeneration)]
        public string Generation { get; set; }

        [Column(Constants.DatabaseFields.FGeolineName)]
        public string Name { get; set; }

        [Column(Constants.DatabaseFields.FGeolineMovement)]
        public string Movement { get; set; }

        [Column(Constants.DatabaseFields.FGeolineHangwall)]
        public string HangingWallDirection { get; set; }

        [Column(Constants.DatabaseFields.FGeolineFoldTrend)]
        public string FoldTrend { get; set; }

        [Column(Constants.DatabaseFields.FGeopointDipPlunge)]
        public string FoldPlunge { get; set; }

        [Column(Constants.DatabaseFields.FGeolineArrowDir)]
        public string ArrowDirection { get; set; }

        [Column(Constants.DatabaseFields.FGeolineBoundary)]
        public string IsBoundary { get; set; }

        [Column(Constants.DatabaseFields.ETCreatorID)]
        public string CreatorID { get; set; }

        [Column(Constants.DatabaseFields.ETEditorID)]
        public string EditorID { get; set; }

        [Column(Constants.DatabaseFields.ETCreateDate)]
        public string CreateDate { get; set; }

        [Column(Constants.DatabaseFields.ETEditDate)]
        public string EditDate { get; set; }

        [Column(Constants.DatabaseFields.LegendSymbol)]
        public string GSCSymbol { get; set; }

        [Column(Constants.DatabaseFields.SourceID)]
        public string SourceID { get; set; }

        [Column(Constants.DatabaseFields.FGeolineGeoEventID)]
        public int EventID { get; set; }

        [Column(Constants.DatabaseFields.FGeolineOriginalCode)]
        public string OriginalCode { get; set; }

        [Column(Constants.DatabaseFields.FGeolineRemarks)]
        public string Remarks { get; set; }

        [Column(Constants.DatabaseFields.FGeolineRemarksEdit)]
        public string EditRemarks { get; set; }

        [Column(Constants.DatabaseFields.FGeolineDisplayPub)]
        public int DisplayPub { get; set; }

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

        [Ignore]
        public string GetGeolineIDFromProperties
        {
            get
            {
                if (GeolineType != -1 && Qualifier != string.Empty && Confidence != string.Empty && Attitude != string.Empty &&
                    Generation != string.Empty)
                {
                    return string.Format("{0}{1}{2}{3}{4}", GeolineType.ToString(), Qualifier, Confidence, Attitude, Generation);
                }
                else
                {
                    return string.Empty;
                }

            }
            set { }
        }

        [Ignore]
        public string GetGeolineSubtypeFromID
        {
            get
            {
                if (GeolineID != string.Empty && GeolineID.Length == 12)
                {
                    return GeolineID.Substring(0, 2);
                }
                else
                {
                    return string.Empty;
                }

            }
            set { }
        }

        [Ignore]
        public string GetGeolineQualifierFromID
        {
            get
            {
                if (GeolineID != string.Empty && GeolineID.Length == 12)
                {
                    return GeolineID.Substring(2, 4);
                }
                else
                {
                    return string.Empty;
                }

            }
            set { }
        }

        [Ignore]
        public string GetGeolineConfidenceFromID
        {
            get
            {
                if (GeolineID != string.Empty && GeolineID.Length == 12)
                {
                    return GeolineID.Substring(6, 2);
                }
                else
                {
                    return string.Empty;
                }

            }
            set { }
        }

        [Ignore]
        public string GetGeolineAttitudeFromID
        {
            get
            {
                if (GeolineID != string.Empty && GeolineID.Length == 12)
                {
                    return GeolineID.Substring(8, 2);
                }
                else
                {
                    return string.Empty;
                }

            }
            set { }
        }

        [Ignore]
        public string GetGeolineGenerationFromID
        {
            get
            {
                if (GeolineID != string.Empty && GeolineID.Length == 12)
                {
                    return GeolineID.Substring(10, 2);
                }
                else
                {
                    return string.Empty;
                }

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
