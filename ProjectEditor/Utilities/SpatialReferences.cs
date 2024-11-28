using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace GSC_ProjectEditor
{
    public class SpatialReferences
    {
        #region GET METHODS

        /// <summary>
        /// Will return a spatial reference from an input feature
        /// </summary>
        /// <param name="inputFeatureName">Input feature name to get spatial ref from</param>
        /// <returns></returns>
        public static ISpatialReference GetSpatialRef(IFeatureClass inputFeatureName)
        {
            IGeoDataset getGeoDataset = inputFeatureName as IGeoDataset;
            return getGeoDataset.SpatialReference;
        }

        /// <summary>
        /// Will return the XY tolerance of a given spatial reference. Addin a multiple, will return the xy tolerance times the multiple. Wil return -1 if projection is unknown
        /// </summary>
        /// <param name="inSR">The input spatial reference to get tolerance from</param>
        /// <param name="multiple">A double value to multiple the tolerance with.</param>
        /// <returns></returns>
        public static double GetXYTolerance(ISpatialReference inSR)
        {
            //Variables
            double calculatedTolerance = 0;

            if (inSR is IGeographicCoordinateSystem)
            {
                IGeographicCoordinateSystem inGeoSR = inSR as IGeographicCoordinateSystem;
                ISpatialReferenceResolution inSRRsn = inGeoSR as ISpatialReferenceResolution;
                calculatedTolerance = inSRRsn.get_XYResolution(true);
            }

            if (inSR is IProjectedCoordinateSystem)
            {
                IProjectedCoordinateSystem inGeoSR = inSR as IProjectedCoordinateSystem;
                ISpatialReferenceResolution inSRRsn = inGeoSR as ISpatialReferenceResolution;
                calculatedTolerance = inSRRsn.get_XYResolution(true);
            }

            if (inSR is IUnknownCoordinateSystem)
            {
                calculatedTolerance = -1; 
            }
            return calculatedTolerance;

        }

        /// <summary>
        /// Will return a double value from input spatial reference XY resolution divided by 10 000, times 2, based from project scale.
        /// </summary>
        /// <param name="inSR">The input spatial reference to calculate the value from.</param>
        /// <returns></returns>
        public static double GetXYToleranceFromProjectScale(ISpatialReference inSR)
        {
            //Variables
            double scaleTolerance = 1;
            double inTolerance = GetXYTolerance(inSR);


            double currentProjectScale = 50000;
            string currentProjectScaleString = Form_Generic.ShowGenericTextboxForm("Project Scale", "Input Project Scale to determine XY Tolerance", null, "50000");
            Double.TryParse(currentProjectScaleString, out currentProjectScale);

            if (currentProjectScale>1)
            {
                scaleTolerance = (currentProjectScale / 10000) * 2;
            }

            return scaleTolerance;
        }

        #endregion

        #region SET METHODS

        /// <summary>
        /// Will set a spatial reference to an input file geodatabase feature class or feature dataset
        /// </summary>
        /// <param name="inputDatset"></param>
        /// <returns></returns>
        public static bool SetSpatialRef(object inputDataset, ISpatialReference newSR)
        {
            //Variables
            bool didSet = false;

            //Try cast to feature class
            IFeatureClass castToFC = inputDataset as IFeatureClass;
            
            //Try Cast to feature dataset
            IFeatureDataset castToFD = inputDataset as IFeatureDataset;

            if (castToFC != null || castToFD != null)
            {
                //Cast features into geodatasetschema to edit projection
                IGeoDatasetSchemaEdit inGeoSchema = castToFC as IGeoDatasetSchemaEdit;

                if (castToFC!=null)
                {
                    inGeoSchema = castToFC as IGeoDatasetSchemaEdit;
                }
                else if (castToFD != null)
	            {
                    inGeoSchema = castToFD as IGeoDatasetSchemaEdit;
	            }

                //Alter the feature with new projection (use this method if no projection is set)
                if (inGeoSchema.CanAlterSpatialReference)
                {
                    inGeoSchema.AlterSpatialReference(newSR);
                    didSet = true;
                }
                
            }

            return didSet;
        }

        #endregion

        /// <summary>
        /// Will create a geographic coordinate system from an input type
        /// </summary>
        /// <param name="inType"></param>
        /// <returns></returns>
        public static IGeographicCoordinateSystem CreateGeographicCoordinateSystem(esriSRGeoCSType inType)
        {
            // Set up the SpatialReferenceEnvironment.
            // SpatialReferenceEnvironment is a singleton object and needs to use the Activator class.

            Type t = Type.GetTypeFromProgID("esriGeometry.SpatialReferenceEnvironment");
            System.Object obj = Activator.CreateInstance(t);
            ISpatialReferenceFactory srFact = obj as ISpatialReferenceFactory;

            // Use the enumeration to create an instance of the predefined object.

            IGeographicCoordinateSystem geographicCS =
                srFact.CreateGeographicCoordinateSystem((int)
                inType);

            return geographicCS;
        }

    }
}
