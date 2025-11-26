using ArcGIS.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BedrockEditorPro.Utilities
{
    public class GeometryMethods
    {
        /// <summary>
        /// Will output a list of all geometries from an exploded
        /// multipart feature
        /// https://github.com/Esri/arcgis-pro-sdk/wiki/ProSnippets-Geometry#get-the-individual-parts-of-a-multipart-feature
        /// </summary>
        /// <param name="inputGeometry"></param>
        /// <returns></returns>
        public static IEnumerable<Geometry> MultipartToSinglePart(Geometry inputGeometry)
        {
            // list holding the part(s) of the input geometry
            List<Geometry> singleParts = new List<Geometry>();

            // check if the input is a null pointer or if the geometry is empty
            if (inputGeometry == null || inputGeometry.IsEmpty)
                return singleParts;

            // based on the type of geometry, take the parts/points and add them individually into a list
            switch (inputGeometry.GeometryType)
            {
                case GeometryType.Envelope:
                    singleParts.Add(inputGeometry.Clone() as Envelope);
                    break;
                case GeometryType.Multipatch:
                    singleParts.Add(inputGeometry.Clone() as Multipatch);
                    break;
                case GeometryType.Multipoint:
                    var multiPoint = inputGeometry as Multipoint;

                    foreach (var point in multiPoint.Points)
                    {
                        // add each point of collection as a standalone point into the list
                        singleParts.Add(point);
                    }
                    break;
                case GeometryType.Point:
                    singleParts.Add(inputGeometry.Clone() as MapPoint);
                    break;
                case GeometryType.Polygon:
                    var polygonNew = inputGeometry as Polygon;

                    foreach (var polygonPart in polygonNew.Parts)
                    {
                        // use the PolygonBuilderEx turning the segments into a standalone 
                        // polygon instance
                        singleParts.Add(PolygonBuilderEx.CreatePolygon(polygonPart));
                    }
                    break;
                case GeometryType.Polyline:
                    var polylineNew = inputGeometry as Polyline;

                    foreach (var polylinePart in polylineNew.Parts)
                    {
                        // use the PolylineBuilderEx turning the segments into a standalone
                        // polyline instance
                        singleParts.Add(PolylineBuilderEx.CreatePolyline(polylinePart));
                    }
                    break;
                case GeometryType.Unknown:
                    break;
                default:
                    break;
            }
            return singleParts;
        }
    }
}
