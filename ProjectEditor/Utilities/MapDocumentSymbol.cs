using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;

namespace GSC_ProjectEditor.Utilities
    
{
    class MapDocumentSymbol
    {
        /// <summary>
        /// Retrieve a line style for input feature
        /// </summary>
        /// <param name="inputFeature">Reference to feature class name to get matching line symbol</param>
        /// <returns></returns>
        public static ILineSymbol GetMatchingSymbol(string inputFeature, string stylePath)
        {
            ILineSymbol getLineSymbol = GSC_ProjectEditor.Symbols.GetMatchingLineSymbolFromStyle(ArcMap.Document, inputFeature, stylePath);

            return getLineSymbol;
        }

        /// <summary>
        /// Retrieve a polygon style from a style file and manages empty values with a default style, for map units
        /// </summary>
        /// <param name="inputSymbolCode">Reference symbol code to get style from</param>
        /// <returns></returns>
        public static IFillSymbol GetMatchingPolygonSymbol(string inputSymbolCode, string stylePath)
        {

            //Get matching style
            IFillSymbol getPolySym = GSC_ProjectEditor.Symbols.GetMatchingPolygonSymbolFromStyle(ArcMap.Document, inputSymbolCode, stylePath);

            return getPolySym;
        }

        /// <summary>
        /// Retrieve a point style from a style file and manages empty values with a default style, fro point symbols
        /// </summary>
        /// <param name="inputSymbolCode"></param>
        /// <returns></returns>
        public static IMarkerSymbol GetMatchingPointSymbol(string inputSymbolCode, string stylePath)
        {
            //Get matching style
            IMarkerSymbol getPointSym = GSC_ProjectEditor.Symbols.GetMatchingPointSymbolFromStyle(ArcMap.Document, inputSymbolCode, stylePath);

            return getPointSym;

        }

        /// <summary>
        /// DEPRECIATED: Finds same RGB code into a style and returns the style code name
        /// </summary>
        /// <param name="getRGB"></param>
        /// <returns></returns>
        public static string GetMatchingPolygonCodeFromSymbolRGB(IColor getRGB, string stylePath)
        {
            return GSC_ProjectEditor.Symbols.GetMatchingPolygonRGBFromStyle(ArcMap.Document, getRGB, stylePath);
        }

        /// <summary>
        /// Will return as a string the correct style item name for given symbol, passed as object.
        /// </summary>
        /// <param name="inputSymbol">symbol object</param>
        /// <param name="inputSymbolType">symbole type name</param>
        /// <returns></returns>
        public static string GetMatchingLineCodeFromSymbol(object inputSymbol, string inputSymbolType, string stylePath)
        {
            string outputCode = "";

            try
            {

                //Variables
                object currentSymbol = new object(); //object that will contain symbol match from style

                //Cast symbol as a clone object
                IClone objSRClone1 = inputSymbol as IClone;

                //Access style gallery
                IStyleGallery styleGal = null;
                styleGal = ArcMap.Document.StyleGallery;
                styleGal.ImportStyle(stylePath);

                //Iterate through gallery
                IStyleGalleryItem styleItem = null;
                IEnumStyleGalleryItem enumStyle = styleGal.Items["Line Symbols", stylePath, null];
                
                while ((styleItem = enumStyle.Next()) != null)
                {
                    if (inputSymbolType == "IMarkerLineSymbol")
                    {
                        IMarkerLineSymbol currentLine = styleItem.Item as MarkerLineSymbol;
                        currentSymbol = currentLine;

                    }
                    else if (inputSymbolType == "ICartographicLineSymbol")
                    {

                        ICartographicLineSymbol currentLine = styleItem.Item as CartographicLineSymbol;
                        currentSymbol = currentLine;

                    }
                    else if (inputSymbolType == "ISimpleLineSymbol")
                    {

                        ISimpleLineSymbol currentLine = styleItem.Item as SimpleLineSymbol;
                        currentSymbol = currentLine;

                    }
                    else if (inputSymbolType == "IMultiLayerLineSymbol")
                    {

                        IMultiLayerLineSymbol currentLine = styleItem.Item as MultiLayerLineSymbol;
                        currentSymbol = currentLine;

                    }
                    else if (inputSymbolType == "IHashLineSymbol")
                    {

                        IHashLineSymbol currentLine = styleItem.Item as HashLineSymbol;
                        currentSymbol = currentLine;

                    }

                    if (currentSymbol != null)
                    {
                        if (objSRClone1.IsEqual((IClone)currentSymbol))
                        {
                            outputCode = styleItem.Name;
                            break;
                        }
                    }

                }

                //Release the cursor or else some lock could happen.
                System.Runtime.InteropServices.Marshal.ReleaseComObject(styleGal);


            }
            catch (Exception e)
            {
                MessageBox.Show("GetMatchingLineCodeFromSymbol: " + e.Message);
            }


            return outputCode;
        }

        /// <summary>
        /// Will return as a string the correct style item name for given symbol, passed as object.
        /// </summary>
        /// <param name="inputSymbol">symbol object</param>
        /// <param name="inputSymbolType">symbole type name</param>
        /// <returns></returns>
        public static string GetMatchingPointCodeFromSymbol(object inputSymbol, string inputSymbolType, string stylePath)
        {
            string outputCode = "";

            try
            {

                //Variables
                object currentSymbol = new object(); //object that will contain symbol match from style

                //Cast symbol as a clone object
                IClone objSRClone1 = inputSymbol as IClone;

                //Access style gallery
                IStyleGallery styleGal = null;
                styleGal = ArcMap.Document.StyleGallery;
                styleGal.ImportStyle(stylePath);

                //Iterate through gallery
                IStyleGalleryItem styleItem = null;
                IEnumStyleGalleryItem enumStyle = styleGal.Items["Marker Symbols", stylePath, null];

                while ((styleItem = enumStyle.Next()) != null)
                {
                    if (inputSymbolType == "IArrowMarkerSymbol")
                    {
                        IArrowMarkerSymbol currentLine = styleItem.Item as ArrowMarkerSymbol;
                        currentSymbol = currentLine;

                    }
                    else if (inputSymbolType == "ICharacterMarkerSymbol")
                    {

                        ICharacterMarkerSymbol currentLine = styleItem.Item as CharacterMarkerSymbol;
                        currentSymbol = currentLine;

                    }
                    else if (inputSymbolType == "IPictureMarkerSymbol")
                    {

                        IPictureMarkerSymbol currentLine = styleItem.Item as PictureMarkerSymbol;
                        currentSymbol = currentLine;

                    }
                    else if (inputSymbolType == "ISimpleMarkerSymbol")
                    {

                        ISimpleMarkerSymbol currentLine = styleItem.Item as SimpleMarkerSymbol;
                        currentSymbol = currentLine;

                    }
                    else if (inputSymbolType == "IMultiLayerMarkerSymbol")
                    {

                        IMultiLayerMarkerSymbol currentLine = styleItem.Item as MultiLayerMarkerSymbol;
                        currentSymbol = currentLine;

                    }
                    if (currentSymbol!=null)
                    {
                        if (objSRClone1.IsEqual((IClone)currentSymbol))
                        {
                            outputCode = styleItem.Name;
                            break;
                        }
                    }


                }

                //Release the cursor or else some lock could happen.
                System.Runtime.InteropServices.Marshal.ReleaseComObject(styleGal);


            }
            catch (Exception e)
            {
                MessageBox.Show("GetMatchingPointCodeFromSymbol: " + e.Message);
            }


            return outputCode;
        }

        /// <summary>
        /// Will return as a string the correct style item name for given symbol, passed as object.
        /// </summary>
        /// <param name="inputSymbol">symbol object</param>
        /// <param name="inputSymbolType">symbole type name</param>
        /// <returns></returns>
        public static string GetMatchingPolygonCodeFromSymbol(object inputSymbol, string inputSymbolType, string stylePath)
        {
            string outputCode = "";

            try
            {

                //Variables
                object currentSymbol = new object(); //object that will contain symbol match from style

                //Cast symbol as a clone object
                IClone objSRClone1 = inputSymbol as IClone;

                //Access style gallery
                IStyleGallery styleGal = null;
                styleGal = ArcMap.Document.StyleGallery;
                styleGal.ImportStyle(stylePath);

                //Iterate through gallery
                IStyleGalleryItem styleItem = null;
                IEnumStyleGalleryItem enumStyle = styleGal.Items["Fill Symbols", stylePath, null];

                while ((styleItem = enumStyle.Next()) != null)
                {
                    if (inputSymbolType == "IGradientFillSymbol")
                    {
                        IGradientFillSymbol currentLine = styleItem.Item as GradientFillSymbol;
                        currentSymbol = currentLine;

                    }
                    else if (inputSymbolType == "ILineFillSymbol")
                    {

                        ILineFillSymbol currentLine = styleItem.Item as LineFillSymbol;
                        currentSymbol = currentLine;

                    }
                    else if (inputSymbolType == "IMarkerFillSymbol")
                    {

                        IMarkerFillSymbol currentLine = styleItem.Item as MarkerFillSymbol;
                        currentSymbol = currentLine;

                    }
                    else if (inputSymbolType == "IPictureFillSymbol")
                    {

                        IPictureFillSymbol currentLine = styleItem.Item as PictureFillSymbol;
                        currentSymbol = currentLine;

                    }
                    else if (inputSymbolType == "ISimpleFillSymbol")
                    {

                        ISimpleFillSymbol currentLine = styleItem.Item as SimpleFillSymbol;
                        currentSymbol = currentLine;

                    }

                    else if (inputSymbolType == "IMultiLayerFillSymbol")
                    {

                        IMultiLayerFillSymbol currentLine = styleItem.Item as MultiLayerFillSymbol;
                        currentSymbol = currentLine;

                    }

                    if (currentSymbol != null)
                    {
                        if (objSRClone1.IsEqual((IClone)currentSymbol))
                        {
                            outputCode = styleItem.Name;
                            break;
                        }
                    }


                }

                //Release the cursor or else some lock could happen.
                System.Runtime.InteropServices.Marshal.ReleaseComObject(styleGal);


            }
            catch (Exception e)
            {
                MessageBox.Show("GetMatchingPointCodeFromSymbol: " + e.Message);
            }


            return outputCode;
        }

    }
}
