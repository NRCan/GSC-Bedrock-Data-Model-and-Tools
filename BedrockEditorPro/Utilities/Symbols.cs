using ArcGIS.Core.CIM;
using ArcGIS.Core.Internal.CIM;
using ArcGIS.Desktop.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BedrockEditorPro.Utilities
{
    public class Symbols
    {
        ///// <summary>
        ///// Creates a point symbol renderer
        ///// </summary>
        ///// <param name="pointRGB">A list containing red green blue numerical codes for point color</param>
        ///// <param name="pointSize">A point size</param>
        ///// <returns></returns>
        //public static CIMPointSymbol GetPointRenderer(List<int> pointRGB, double pointSize)
        //{
        //    //SymbolFactory.Instance.ConstructPointSymbol(ColorFactory.Instance.RedRGB, 8, SimpleMarkerStyle.Circle);

        //    ////Create a renderer for style
        //    //ISimpleRenderer pointRenderer = new SimpleRenderer();

        //    ////Create line symbol for default symbol
        //    //ISimpleMarkerSymbol pointSym = new SimpleMarkerSymbol();
        //    //pointSym.Style = esriSimpleMarkerStyle.esriSMSCircle; //Set it to null

        //    ////Create an RGB object for the fill part
        //    //RgbColor colorLine = new RgbColor();
        //    //colorLine.Red = pointRGB[0];
        //    //colorLine.Green = pointRGB[1];
        //    //colorLine.Blue = pointRGB[2];

        //    ////Add color and width
        //    //pointSym.Color = colorLine;
        //    //pointSym.Size = pointSize;

        //    ////Set renderer to line symbol
        //    //pointRenderer.Symbol = pointSym as ISymbol;

        //    //return pointRenderer;
        //}

        /// <summary>
        /// Get a grey point symbol for default values or null values
        /// </summary>
        /// <returns></returns>
        public static CIMPointSymbol GetDefaultPointSymbol()
        {
            return SymbolFactory.Instance.ConstructPointSymbol(ColorFactory.Instance.RedRGB, 4, SimpleMarkerStyle.Circle);
        }

        /// <summary>
        /// Get an empty fill polygon symbol for default values or null values
        /// Black outline with an empty filling
        /// </summary>
        /// <returns></returns>
        public static CIMPolygonSymbol GetDefaultPolygonSymbol()
        {
            CIMStroke outline = SymbolFactory.Instance.ConstructStroke(
                ColorFactory.Instance.BlackRGB, 1.0, SimpleLineStyle.Solid);

            CIMPolygonSymbol nullFillWithOutline = SymbolFactory.Instance.ConstructPolygonSymbol(
                ColorFactory.Instance.BlueRGB, SimpleFillStyle.Null , outline);

            return nullFillWithOutline;
        }

        ///// <summary>
        ///// Will apply default properties to an input geopoint renderer. If no renderer is passed, a new one will be created.
        ///// Doesn't set the field and field numbers.
        ///// </summary>
        ///// <param name="inRenderer">The renderer to modify, can be null to get a new one</param>
        //public static UniqueValueRendererDefinition SetGeopointRendererProperties(IUniqueValueRenderer geopointRenderer, string pathToStyle = "")
        //{
        //    ////Start with main default and then apply geoline defaults
        //    //UniqueValueRendererDefinition defaultGeopointRenderer = SetDefaultRendererProperties(geopointRenderer, GetDefaultPointSymbol() as ISymbol);

        //    //if (pathToStyle != string.Empty)
        //    //{
        //    //    defaultGeopointRenderer.LookupStyleset = pathToStyle;
        //    //    //Set rotation renderer
        //    //    IRotationRenderer rotRender = defaultGeopointRenderer as IRotationRenderer;
        //    //    rotRender.RotationField = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointAzimuth;
        //    //    rotRender.RotationType = esriSymbolRotationType.esriRotateSymbolGeographic;
        //    //}

        //    //return defaultGeopointRenderer;
        //}

        ///// <summary>
        ///// Will apply default properties to input renderer. If no renderer is passed, a new one will be created.
        ///// Doesn't set the field and field numbers.
        ///// Sets Color Scheme, field delimiter, default symbol and lpokup style sheet.
        ///// </summary>
        ///// <param name="inRenderer">The renderer to modify, can be null to get a new one</param>
        //public static UniqueValueRendererDefinition SetDefaultRendererProperties(UniqueValueRendererDefinition uniqueRenderer, Symbol defaultSymbol)
        //{
        //    //if (uniqueRenderer == null)
        //    //{
        //    //    uniqueRenderer = new UniqueValueRendererDefinition();
        //    //}

        //    //uniqueRenderer.ColorRamp = ColorFactory.Instance.GetColorRamp("Default"); //Set a default color scheme
        //    //uniqueRenderer.FieldDelimiter = Constants.Symbol4Layers.fieldDelimeter; //Add a field delimiter
        //    //uniqueRenderer.DefaultSymbol = defaultSymbol; //Give a symbol object to default symbol
        //    //uniqueRenderer.UseDefaultSymbol = true; //Null will be the default

        //    //return uniqueRenderer;
        //}
    }
}
