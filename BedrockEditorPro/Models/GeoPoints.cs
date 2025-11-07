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
    [Table(Constants.Database.FGeopoint)]
    public class GeoPoints
    {
        [Column(Constants.DatabaseFields.FGeopointID), PrimaryKey]
        public string GeopointID { get; set; }

        [Column(Constants.DatabaseFields.FGeopointType)]
        public int GeopointType { get; set; }

        [Column(Constants.DatabaseFields.FGeopointSubset)]
        public string Subset { get; set; }

        [Column(Constants.DatabaseFields.FGeopointStrucAtt)]
        public string Attitude { get; set; }

        [Column(Constants.DatabaseFields.FGeopointStrucGene)]
        public string Generation { get; set; }

        [Column(Constants.DatabaseFields.FGeopointStrucYoung)]
        public string Younging { get; set; }

        [Column(Constants.DatabaseFields.FGeopointStrucMethod)]
        public string Method { get; set; }

        [Column(Constants.DatabaseFields.FGeopointRelatedStruc)]
        public string RelatedStructure { get; set; }

        [Column(Constants.DatabaseFields.FGeopointAzimuth)]
        public string Azimuth { get; set; }

        [Column(Constants.DatabaseFields.FGeopointDipPlunge)]
        public string DipPlunge { get; set; }

        [Column(Constants.DatabaseFields.FGeopointDipDescription)]
        public string DipDescription { get; set; }

        [Column(Constants.DatabaseFields.FGeopointSenseEvid)]
        public string SenseEvidence { get; set; }

        [Column(Constants.DatabaseFields.FGeopointStrain)]
        public string Strain { get; set; }

        [Column(Constants.DatabaseFields.FGeopointFlat)]
        public string Flattening { get; set; }

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

        [Column(Constants.DatabaseFields.SourceID)]
        public string SourceID { get; set; }

        [Column(Constants.DatabaseFields.FGeopointOriginalCode)]
        public string OriginalCode { get; set; }

        [Column(Constants.DatabaseFields.FGeopointRemark)]
        public string Remarks { get; set; }

        [Column(Constants.DatabaseFields.FGeopointEditRemarks)]
        public string EditRemarks { get; set; }

        [Column(Constants.DatabaseFields.FGeopointDisplayFrom)]
        public int DisplayFrom{ get; set; }

        [Column(Constants.DatabaseFields.FGeopointDisplayTo)]
        public int DisplayTo { get; set; }

        [Column(Constants.DatabaseFields.FGeopointDisplayPub)]
        public int DisplayPub { get; set; }

        //Not a true database field, used for temporary naming
        public string Name { get; set; }

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
                if (GeopointType != -1 && Subset != string.Empty && Attitude != string.Empty && Younging != string.Empty &&
                    Method != string.Empty && Generation != string.Empty)
                {
                    return string.Format("{0}{1}{2}{3}{4}{5}", GeopointType.ToString(), Subset, Attitude, Generation, Younging, Method);
                }
                else
                {
                    return string.Empty;
                }

            }
            set { }
        }

        [Ignore]
        public string GetGeopointSubtypeFromID
        {
            get
            {
                if (GeopointID != string.Empty && GeopointID.Length == 13)
                {
                    return GeopointID.Substring(0, 1);
                }
                else
                {
                    return string.Empty;
                }

            }
            set { }
        }

        [Ignore]
        public string GetGeolpointSubsetFromID
        {
            get
            {
                if (GeopointID != string.Empty && GeopointID.Length == 13)
                {
                    return GeopointID.Substring(1, 4);
                }
                else
                {
                    return string.Empty;
                }

            }
            set { }
        }

        [Ignore]
        public string GetGeopointAttitudeFromID
        {
            get
            {
                if (GeopointID != string.Empty && GeopointID.Length == 13)
                {
                    return GeopointID.Substring(5, 2);
                }
                else
                {
                    return string.Empty;
                }

            }
            set { }
        }

        [Ignore]
        public string GetGeopointGenerationFromID
        {
            get
            {
                if (GeopointID != string.Empty && GeopointID.Length == 13)
                {
                    return GeopointID.Substring(7, 2);
                }
                else
                {
                    return string.Empty;
                }

            }
            set { }
        }

        [Ignore]
        public string GetGeopointYoungingFromID
        {
            get
            {
                if (GeopointID != string.Empty && GeopointID.Length == 13)
                {
                    return GeopointID.Substring(9, 2);
                }
                else
                {
                    return string.Empty;
                }

            }
            set { }
        }

        [Ignore]
        public string GetGeopointMethodFromID
        {
            get
            {
                if (GeopointID != string.Empty && GeopointID.Length == 13)
                {
                    return GeopointID.Substring(11, 2);
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
