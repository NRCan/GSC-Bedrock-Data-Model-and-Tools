using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Editor;

namespace GSC_ProjectEditor
{
    public class Geometry
    {
        /// <summary>
        /// Will return a point array buid with a list of vertices pairs [[x1,y1, z1], [x2,y2, z2], [xi, yi, zi], ...]
        /// </summary>
        /// <param name="coordinateList">List of coordinate pairs</param>
        /// <returns></returns>
        public static IPointCollection BuildPointArray(List<List<double>> coordinateList)
        {
            //Create the new point object
            IPointCollection newPointArray = new PolygonClass();

            //Create all points by looping list
            IPoint newPoint = new PointClass();

            foreach (List<double> vertices in coordinateList)
            {
                //Add geometry
                newPoint.X = vertices[0];
                newPoint.Y = vertices[1];
                newPoint.Z = vertices[2];

                //Make Z aware the point collection, or else throws an error because all database feat are 3D
                MakeZAware(newPoint as IGeometry);

                //Add to array
                newPointArray.AddPoint(newPoint);
            }



            return newPointArray;
        }

        /// <summary>
        /// Will enable z awareness on any geometry
        /// </summary>
        /// <param name="inputGeometry"></param>
        public static void MakeZAware(IGeometry inputGeometry)
        {
            IZAware setZ = inputGeometry as IZAware;
            setZ.ZAware = true;
        }

        /// <summary>
        /// Will apply a constant Z value to polyline or polygon geometries, special method is used for points
        /// </summary>
        /// <param name="inputGeometry">A double value of Z to set as constant</param>
        public static void ApplyConstantZ(IGeometry inputGeometry, double zValue)
        {
            //For lines and polygons
            if (inputGeometry.GeometryType != esriGeometryType.esriGeometryPoint)
            {
                IZ setConstZ = inputGeometry as IZ;
                setConstZ.SetConstantZ(zValue); 
            }
            else //For points only
            {
                //Cast the geometry as a point
                IPoint inputPoint = inputGeometry as IPoint;

                //Make Z aware
                IZAware setZPnt = inputPoint as IZAware;
                setZPnt.ZAware = true;

                //Apply constant
                inputPoint.Z = zValue;
            }

        }

        /// <summary>
        /// Will calculate a bearing value for a line geometry, the result is a float numbes
        /// </summary>
        /// <param name="inputGeometry"></param>
        /// <returns></returns>
        public static double CalculateBearing(IGeometry inputGeometry)
        {
            //Variables
            double bearing;
            
            //Cast geomety
            IPolyline getPolylineGeom = inputGeometry as IPolyline;

            //Cast first and end nodes
            IPoint firstGeom = getPolylineGeom.FromPoint;
            IPoint endGeom = getPolylineGeom.ToPoint;

            //Calculate
            bearing = 180 + Math.Atan2((firstGeom.X - endGeom.X),(firstGeom.Y-endGeom.Y)) * (180/Math.PI);

            return bearing;
        }

        /// <summary>
        /// Will delete any geometry inside given feature that has null geometries.
        /// </summary>
        /// <param name="inFC">The feature in which the null values will be cleaned</param>
        /// <returns></returns>
        public static string DeleteNullGeometries(IFeatureClass inFC)
        {
            return GSC_ProjectEditor.GeoProcessing.RepairGeometry(inFC, true);
        }

        /// <summary>
        /// From a given polyline feature class, will epxlode multiparts to single part.
        /// </summary>
        /// <param name="inFC">The polyline feature class to explode</param>
        public static void PolylineMultiPartToSinglePart(IFeatureClass inFC, IEditor editSession)
        {

            //Start a cursor to iterate through all the features inside feature class
            IFeatureCursor polylineCursor = inFC.Search(null, false); //Set recycling to false, or else it's not possible to edit geometry while in a cursor
            IFeature polylineRow = polylineCursor.NextFeature();
            while (polylineRow != null)
            {
                //Cast a copy of the geometry from shape field
                IGeometry geom = polylineRow.ShapeCopy;

                //Cast to polyline the current geometry
                IPolyline6 polylinePolish = geom as IPolyline6;

                //Casst the geometry as a collection
                IGeometryCollection geomCollection = geom as IGeometryCollection;

                //Default values for futur new feature, if applies
                object Missing = Type.Missing;

                //Densify current part
                editSession.StartOperation();
                polylinePolish.Densify(-1, -1);
                polylineRow.Shape = polylinePolish as IGeometry;
                polylineRow.Store();
                editSession.StopOperation("Densify");

                //Get a part count from collection
                int geomCount = geomCollection.GeometryCount;

                //Iterate through all parts
                while (geomCount != 1)
                {
                    //Get current part index from count
                    int part = geomCount - 1;

                    //Create a new feature to store new part as a feature
                    IFeature newFeature = inFC.CreateFeature();

                    //Get current part geometry
                    IGeometry origPartGeometry = geomCollection.get_Geometry(part);

                    //Start an edit operation on current feature
                    editSession.StartOperation();

                    //Delete the original part.
                    geomCollection.RemoveGeometries(part, 1);
                    geomCollection.GeometriesChanged();

                    //Create a new polyline class and apply Z awareness to it
                    IPolyline polyline = new PolylineClass();
                    IZAware zAware;
                    zAware = polyline as IZAware;
                    zAware.ZAware = true;

                    IGeometryCollection newGeomCollection = polyline as IGeometryCollection;
                    newGeomCollection.AddGeometry(origPartGeometry, ref Missing, ref Missing);

                    //Add current removed geometry to new feature
                    newFeature.Shape = newGeomCollection as IGeometry;

                    //Copy the attributes of the original feature the new feature.
                    IField field = new FieldClass();
                    IFields fields = polylineRow.Fields;

                    //Skip OID and geometry.
                    for (int fieldCount = 0; fieldCount < fields.FieldCount; fieldCount++)
                    {
                        field = fields.get_Field(fieldCount);

                        if ((field.Type != esriFieldType.esriFieldTypeGeometry) &&
                            (field.Type != esriFieldType.esriFieldTypeOID) && field.Editable)
                        {
                            newFeature.set_Value(fieldCount, polylineRow.get_Value(fieldCount));
                        }
                    }

                    //Store the new geometry 
                    newFeature.Store();

                    //Stop edit operation and give it a name for the stack
                    editSession.StopOperation("Convert Part to Feature");

                    //Recalculate geomCollection count for while loop
                    geomCount = geomCollection.GeometryCount;
                }

                polylineRow = polylineCursor.NextFeature();
            }

            //Release coms and flush
            polylineCursor.Flush();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(polylineCursor);
        }

        /// <summary>
        /// Will densify a given polyline feature class, in order to remove any arc from it, like bezier curves
        /// </summary>
        /// <param name="inFC">The polyline to densify</param>
        /// <param name="editSession">An edit session to work inside of.</param>
        public static void PolylineDensify(IFeatureClass inFC, IEditor editSession)
        {
            //Get spatial reference
            ISpatialReference inSR = GSC_ProjectEditor.SpatialReferences.GetSpatialRef(inFC);
            double inSRTolerance = GSC_ProjectEditor.SpatialReferences.GetXYToleranceFromProjectScale(inSR);

            //Start a cursor to iterate through all the features inside feature class
            IFeatureCursor polylineCursor = inFC.Search(null, false); //Set recycling to false, or else it's not possible to edit geometry while in a cursor
            IFeature polylineRow = polylineCursor.NextFeature();
            while (polylineRow != null)
            {
                //Cast a copy of the geometry from shape field
                IGeometry geom = polylineRow.ShapeCopy;

                //Cast to polyline the current geometry
                IPolyline6 polylinePolish = geom as IPolyline6;

                //Densify current part
                editSession.StartOperation();
                polylinePolish.Densify(inSRTolerance, -1);
                polylineRow.Shape = polylinePolish as IGeometry;
                polylineRow.Store();
                editSession.StopOperation("Densify");

                polylineRow = polylineCursor.NextFeature();
            }

            //Release coms and flush
            polylineCursor.Flush();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(polylineCursor);
        }

        /// <summary>
        /// Will remove any empty geometry or null lenght from a polyline. DOESN'T WORK WHEN A TOPOLOGY IS ASSOCIATED WITH FEATURE CLASS
        /// </summary>
        /// <param name="inFC">The polyline feature class to process.</param>
        /// <param name="editSession">An edit session to work with.</param>
        public static void PolylineRemoveNullLenght_EmptyFeature(IFeatureClass inFC, IEditor editSession)
        {
            //Start a cursor to iterate through all the features inside feature class
            IFeatureCursor polylineCursor = inFC.Update(null, false); //Set recycling to false, or else it's not possible to edit geometry while in a cursor
            IDataset inDataset = inFC as IDataset;
            IFeature polylineRow = polylineCursor.NextFeature();
            while (polylineRow != null)
            {
                //Cast a copy of the geometry from shape field
                IGeometry geom = polylineRow.ShapeCopy;

                //Cast to polyline the current geometry
                IPolyline6 polylinePolish = geom as IPolyline6;

                if (polylinePolish.IsEmpty || polylinePolish.Length == 0)
                {
                    //Remove feature
                    polylineCursor.DeleteFeature(); //This method doesn't work because of the topology layer associated with geoline
                }

                polylineRow = polylineCursor.NextFeature();
            }

            //Release coms and flush
            System.Runtime.InteropServices.Marshal.ReleaseComObject(polylineCursor);
        }

        /// <summary>
        /// Will convert a input degree from a geographic system to an arithmetic one.
        /// </summary>
        /// <param name="arithmeticDegree">The degree value to convert</param>
        /// <returns></returns>
        public static int ConvertGeographicToArithmeticDegree(int geoDegree)
        {
            //Axes are shifted by 270 degree
            int outDegree = geoDegree + 270;

            //Modulo 360
            while (outDegree>360)
            {
                outDegree = outDegree - 360;
            }
            //Absolute substract to get conversion
            outDegree = Math.Abs(outDegree - 360);

            return outDegree;
        }

        /// <summary>
        /// Will convert an input degree value to a radian one
        /// </summary>
        /// <param name="inDegree">The value to convert</param>
        /// <returns></returns>
        public static double ConvertToRadians(double inDegree) 
        {
            return inDegree * Math.PI / 180;

        }

    }
}
