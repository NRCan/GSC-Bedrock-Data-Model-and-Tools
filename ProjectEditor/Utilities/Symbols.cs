using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.ArcMapUI;

namespace GSC_ProjectEditor
{
    public class Symbols
    {

        #region Main Variables

        //GEO_LINE feature
        private const string geolineFeature = GSC_ProjectEditor.Constants.Database.FGeoline;
        private const string geolineID = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineID;
        private const string geolineSymbol = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineFGDC;

        //M_GEOLINE_SYMBOL
        private const string mGeolineSymbol = GSC_ProjectEditor.Constants.Database.TGeolineSymbol;
        private const string mGeolineID = GSC_ProjectEditor.Constants.DatabaseFields.MGeolineID;
        private const string mGeolineFGDC = GSC_ProjectEditor.Constants.DatabaseFields.MGeolineFGDC;

        //GEO_POINT feature
        private const string geopointFeature = GSC_ProjectEditor.Constants.Database.FGeopoint;
        private const string geopointID = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointID;
        private const string geopointSymbol = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointFGDC;
        private const string geopointType = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointType;
        private const string geopointSubset = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointSubset;
        private const string geopointStrucAtt = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointStrucAtt;
        private const string geopointStrucGene = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointStrucGene;
        private const string geopointStrucYoung = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointStrucYoung;
        private const string geopointStrucMethod = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointStrucMethod;

        //SYMBOL_GEOPOINTS table
        private const string tGeopointSymbol = GSC_ProjectEditor.Constants.Database.TGeopointSymbol;
        private const string tGeopointID = GSC_ProjectEditor.Constants.DatabaseFields.TGeopointID;
        private const string tGeopointFGDC = GSC_ProjectEditor.Constants.DatabaseFields.TGeopointFGDC;

        #endregion

        #region GET methods

        /// <summary>
        /// Retrieve a line style from a style file and manages empty values with a default style
        /// </summary>
        /// <param name="inputStyle">Reference to style to get line symbols from</param>
        /// <param name="inputFeature">Reference to feature class name to get matching line symbol</param>
        /// <returns></returns>
        public static ILineSymbol GetMatchingLineSymbolFromStyle(IMxDocument inputMapDocument, string inputSymbolCode, string pathToStyle)
        {
            //Variables
            ILineSymbol outputSym = null;

            /// Retrieve a line style from a style file and manages empty values with a default style
            /// /// <param name="inputFeature">Reference to feature class name to get matching line symbol</param>
            //Iterate through gallery
            IStyleGalleryItem styleItem = null;
            IEnumStyleGalleryItem enumStyle = inputMapDocument.StyleGallery.Items["Line Symbols", pathToStyle, null];

            while ((styleItem = enumStyle.Next()) != null)
            {
                if (inputSymbolCode == "")
                {
                    outputSym = GetDefaultLineSymbol();
                    break;
                }


                if (styleItem.Name == inputSymbolCode)
                {
                    outputSym = styleItem.Item as ILineSymbol;
                }

            }

            return outputSym;
        }

        /// <summary>
        /// Retrieve a polygon style from a style file and manages empty values with a default style, for map units
        /// </summary>
        /// <param name="inputStyle">Reference symbol code to get style from</param>
        /// <param name="inputSymbolCode">Reference to feature class name to get matching polygon symbol</param>
        /// <returns></returns>
        public static IFillSymbol GetMatchingPolygonSymbolFromStyle(IMxDocument inputMapDocument, string inputSymbolCode, string pathToStyle)
        {
            //Variables
            IFillSymbol outputSym = null;

            //Iterate through gallery
            IStyleGalleryItem styleItem = null;
            IEnumStyleGalleryItem enumStyle = inputMapDocument.StyleGallery.Items["Fill Symbols", pathToStyle, null];

            while ((styleItem = enumStyle.Next()) != null)
            {

                if (styleItem.Name == inputSymbolCode)
                {
                    outputSym = styleItem.Item as IFillSymbol;
                    break;
                }

            }

            return outputSym;
        }

        /// <summary>
        /// Get a grey line symbol for default values or null values
        /// </summary>
        /// <returns></returns>
        public static ISimpleLineSymbol GetDefaultLineSymbol()
        {
            //Create line symbol for default symbol
            ISimpleLineSymbol lineSym = new SimpleLineSymbol();
            lineSym.Style = esriSimpleLineStyle.esriSLSSolid; //Set it to null

            //Create an RGB object for the fill part
            RgbColor colorLine = new RgbColor();
            colorLine.Red = 0;
            colorLine.Green = 0;
            colorLine.Blue = 0;

            lineSym.Color = colorLine;

            return lineSym;
        }

        /// <summary>
        /// Create Simple fill renderer (for polygons), with outline too.
        /// </summary>
        /// <param name="fillRGB">A List containing red blue green numeric codes to fill surface</param>
        /// <param name="outlineRGB">A list containing red blue green numerci code for surface outline</param>
        /// <param name="outlineWidth">A double value to set ouline width</param>
        /// <returns></returns>
        public static ISimpleRenderer GetSimpleFillRenderer(List<int> fillRGB, List<int> outlineRGB, double outlineWidth)
        {

            //Create a renderer for style
            ISimpleRenderer renderer = new SimpleRenderer();

            //Create poly symbol 
            ISimpleFillSymbol polySym = new SimpleFillSymbol();
            polySym.Style = esriSimpleFillStyle.esriSFSSolid; //Default to solid

            //Create an RGB object for the fill part
            RgbColor inputFill = new RgbColor();
            inputFill.Red = fillRGB[0];
            inputFill.Green = fillRGB[1];
            inputFill.Blue = fillRGB[2];

            //Assign rgb to color
            polySym.Color = inputFill;

            //Create a line symbol for outline.
            ISimpleLineSymbol outlineSym = new SimpleLineSymbol();
            outlineSym.Style = esriSimpleLineStyle.esriSLSSolid; //Default to solid.

            //Create an RGB object for outline part
            RgbColor inputOutline = new RgbColor();
            inputOutline.Red = outlineRGB[0];
            inputOutline.Green = outlineRGB[1];
            inputOutline.Blue = outlineRGB[2];

            //Set color and width of outline
            outlineSym.Color = inputOutline;
            outlineSym.Width = outlineWidth;

            //Set outline symbol to fill symbol
            polySym.Outline = outlineSym;

            //Set symbol propertie of renderer.
            renderer.Symbol = polySym as ISymbol; //Give a symbol object to default symbol

            //Return object
            return renderer;

        }

        /// <summary>
        /// Create a line symbol renderer
        /// </summary>
        /// <param name="lineRGB">A list containing red green blue numerical codes for line color</param>
        /// <param name="lineWidth">A line width</param>
        /// <returns></returns>
        public static ISimpleRenderer GetSimpleLineRenderer(List<int> lineRGB, double lineWidth)
        {

            //Create a renderer for style
            ISimpleRenderer renderer = new SimpleRenderer();

            //Create line symbol for default symbol
            ISimpleLineSymbol lineSym = new SimpleLineSymbol();
            lineSym.Style = esriSimpleLineStyle.esriSLSSolid; //Set it to null

            //Create an RGB object for the fill part
            RgbColor colorLine = new RgbColor();
            colorLine.Red = lineRGB[0];
            colorLine.Green = lineRGB[1];
            colorLine.Blue = lineRGB[2];

            //Add color and width
            lineSym.Color = colorLine;
            lineSym.Width = lineWidth;

            //Set renderer to line symbol
            renderer.Symbol = lineSym as ISymbol;

            return renderer;
        }

        /// <summary>
        /// Creates a point symbol renderer
        /// </summary>
        /// <param name="pointRGB">A list containing red green blue numerical codes for point color</param>
        /// <param name="pointSize">A point size</param>
        /// <returns></returns>
        public static ISimpleRenderer GetPointRenderer(List<int> pointRGB, double pointSize)
        {
            //Create a renderer for style
            ISimpleRenderer pointRenderer = new SimpleRenderer();

            //Create line symbol for default symbol
            ISimpleMarkerSymbol pointSym = new SimpleMarkerSymbol();
            pointSym.Style = esriSimpleMarkerStyle.esriSMSCircle; //Set it to null

            //Create an RGB object for the fill part
            RgbColor colorLine = new RgbColor();
            colorLine.Red = pointRGB[0];
            colorLine.Green = pointRGB[1];
            colorLine.Blue = pointRGB[2];

            //Add color and width
            pointSym.Color = colorLine;
            pointSym.Size = pointSize;

            //Set renderer to line symbol
            pointRenderer.Symbol = pointSym as ISymbol;

            return pointRenderer;
        }

        /// <summary>
        /// Will return a style symbol name from a given color that should match input styles
        /// </summary>
        /// <param name="inputStyle">The style to get the name from</param>
        /// <param name="pathToStyle">The path to the style to get the name from</param>
        /// <param name="getRGB">The color to get RGB or CMYK color values from</param>
        /// <returns></returns>
        public static string GetMatchingPolygonRGBFromStyle(IMxDocument inputMapDocument, IColor getRGB, string pathToStyle)
        {

            //Variable
            string getSymbolCode = "";

            //Iterate through gallery
            IStyleGalleryItem styleItem;
            IEnumStyleGalleryItem enumStyle = inputMapDocument.StyleGallery.Items["Fill Symbols", pathToStyle, GSC_ProjectEditor.Constants.Styles.MapUnitCategory];

            while ((styleItem = enumStyle.Next()) != null)
            {
                //Prevents error message from poping, NEED TO DEFINE WHY THOUGH...
                try
                {
                    //MessageBox.Show(styleItem.Name);
                    ISimpleFillSymbol getFill = styleItem.Item as SimpleFillSymbol;
                    IColor currentColor = getFill.Color;

                    //Get CMYK color
                    CmykColor getRGB_ = getRGB as CmykColor;
                    CmykColor currentRGB = currentColor as CmykColor;

                    if (currentRGB.Cyan == getRGB_.Cyan && currentRGB.Yellow == getRGB_.Yellow && currentRGB.Magenta == getRGB_.Magenta && currentRGB.Black == getRGB_.Black)
                    {
                        getSymbolCode = styleItem.Name;
                        break;

                    }
                }
                catch
                {
                }


            }

            return getSymbolCode;
        }

        /// <summary>
        /// Retrieve a point style from a style file and manages empty values with a default style
        /// </summary>
        /// <param name="inputStyle">Reference to style to get point symbols from</param>
        /// <param name="inputFeature">Reference to feature class name to get matching point symbol</param>
        /// <returns></returns>
        public static IMarkerSymbol GetMatchingPointSymbolFromStyle(IMxDocument inputMapDocument, string inputSymbolCode, string pathToStyle)
        {
            //Variables
            IMarkerSymbol outputSym = null;

            //Iterate through gallery
            IStyleGalleryItem styleItem = null;
            IEnumStyleGalleryItem enumStyle = inputMapDocument.StyleGallery.Items["Marker Symbols", pathToStyle, null];

            while ((styleItem = enumStyle.Next()) != null)
            {
                if (inputSymbolCode == "")
                {
                    outputSym = GetDefaultPointSymbol();
                    break;
                }


                if (styleItem.Name == inputSymbolCode)
                {
                    outputSym = styleItem.Item as IMarkerSymbol;

                    if (outputSym != null)
                    {
                        break; //Breaking speeds up the process but also prevent empty duplicate, see: https://scm.nrcan.gc.ca/cti/issues/7320
                    }

                }

            }
            


            return outputSym;
        }

        /// <summary>
        /// Will return as a string, a line symbol type (MarkerLineSymbol, SimpleLineSymbol, etc.)
        /// </summary>
        /// <param name="inSymbol">The input symbol object to valide from</param>
        /// <param name="symbolTypeName"> Output of symbol type name</param>
        /// <returns></returns>
        public static object GetLineSymbolType(ISymbol inSymbol, out string symbolTypeName)
        {
            //Cast input symbol into all kinds of line symbol types
            IMarkerLineSymbol markerLine = inSymbol as MarkerLineSymbol;
            ICartographicLineSymbol cartoLine = inSymbol as CartographicLineSymbol;
            ISimpleLineSymbol simpleLine = inSymbol as SimpleLineSymbol;
            IMultiLayerLineSymbol multilayerLine = inSymbol as MultiLayerLineSymbol;
            IHashLineSymbol hashLine = inSymbol as HashLineSymbol;

            //Init a new object that will contain the correct symbol type
            object correctSymbol = new object();

            //Init symbol type name 
            symbolTypeName = "";

            if (markerLine != null)
            {
                correctSymbol = markerLine;
                symbolTypeName = "IMarkerLineSymbol";
            }
            else if (cartoLine != null)
            {
                correctSymbol = cartoLine;
                symbolTypeName = "ICartographicLineSymbol";
            }
            else if (simpleLine != null)
            {
                correctSymbol = simpleLine;
                symbolTypeName = "ISimpleLineSymbol";
            }
            else if (multilayerLine != null)
            {
                correctSymbol = multilayerLine;
                symbolTypeName = "IMultiLayerLineSymbol";
            }
            else if (hashLine != null)
            {
                correctSymbol = hashLine;
                symbolTypeName = "IHashLineSymbol";
            }

            return correctSymbol;
        }

        /// <summary>
        /// Will return as a string, a point symbol type (ArrowMarker, SimpleMarker, etc.)
        /// </summary>
        /// <param name="inSymbol">The input symbol object to validate from</param>
        /// <param name="symbolTypeName"> Output of symbol type name</param>
        /// <returns></returns>
        public static object GetPointSymbolType(ISymbol inSymbol, out string symbolTypeName)
        {
            //Cast input symbol into all kinds of line symbol types
            IArrowMarkerSymbol arrowPoint = inSymbol as ArrowMarkerSymbol;
            ICharacterMarkerSymbol charPoint = inSymbol as CharacterMarkerSymbol;
            IPictureMarkerSymbol picturePoint = inSymbol as PictureMarkerSymbol;
            ISimpleMarkerSymbol simplePoint = inSymbol as SimpleMarkerSymbol;
            IMultiLayerMarkerSymbol multiPoint = inSymbol as IMultiLayerMarkerSymbol;

            //Init a new object that will contain the correct symbol type
            object correctSymbol = new object();

            //Init symbol type name 
            symbolTypeName = "";

            if (arrowPoint != null)
            {
                correctSymbol = arrowPoint;
                symbolTypeName = "IArrowMarkerSymbol";
            }
            else if (charPoint != null)
            {
                correctSymbol = charPoint;
                symbolTypeName = "ICharacterMarkerSymbol";
            }
            else if (picturePoint != null)
            {
                correctSymbol = picturePoint;
                symbolTypeName = "IPictureMarkerSymbol";
            }
            else if (simplePoint != null)
            {
                correctSymbol = simplePoint;
                symbolTypeName = "ISimpleMarkerSymbol";
            }
            else if (multiPoint != null)
            {
                correctSymbol = multiPoint;
                symbolTypeName = "IMultiLayerMarkerSymbol";
            }

            return correctSymbol;
        }

        /// <summary>
        /// Will return as a string, a polygon symbol type (ArrowMarker, SimpleMarker, etc.)
        /// </summary>
        /// <param name="inSymbol">The input symbol object to validate from</param>
        /// <param name="symbolTypeName"> Output of symbol type name</param>
        /// <returns></returns>
        public static object GetPolygonSymbolType(ISymbol inSymbol, out string symbolTypeName)
        {
            //Cast input symbol into all kinds of line symbol types
            IGradientFillSymbol gradientFill = inSymbol as GradientFillSymbol;
            ILineFillSymbol lineFill = inSymbol as LineFillSymbol;
            IMarkerFillSymbol markerFil = inSymbol as MarkerFillSymbol;
            IPictureFillSymbol pictureFill = inSymbol as PictureFillSymbol;
            ISimpleFillSymbol simpleFill = inSymbol as SimpleFillSymbol;
            IMultiLayerFillSymbol multiFill = inSymbol as IMultiLayerFillSymbol;

            //Init a new object that will contain the correct symbol type
            object correctSymbol = new object();

            //Init symbol type name 
            symbolTypeName = "";

            if (gradientFill != null)
            {
                correctSymbol = gradientFill;
                symbolTypeName = "IGradientFillSymbol";
            }
            else if (lineFill != null)
            {
                correctSymbol = lineFill;
                symbolTypeName = "ILineFillSymbol";
            }
            else if (markerFil != null)
            {
                correctSymbol = markerFil;
                symbolTypeName = "IMarkerFillSymbol";
            }
            else if (pictureFill != null)
            {
                correctSymbol = pictureFill;
                symbolTypeName = "IPictureFillSymbol";
            }
            else if (simpleFill != null)
            {
                correctSymbol = simpleFill;
                symbolTypeName = "ISimpleFillSymbol";
            }
            else if (multiFill != null)
            {
                correctSymbol = multiFill;
                symbolTypeName = "IMultiLayerFillSymbol";
            }

            return correctSymbol;
        }

        /// <summary>
        /// Get a grey point symbol for default values or null values
        /// </summary>
        /// <returns></returns>
        public static ISimpleMarkerSymbol GetDefaultPointSymbol()
        {
            //Create line symbol for default symbol
            ISimpleMarkerSymbol pointSym = new SimpleMarkerSymbol();
            pointSym.Style = esriSimpleMarkerStyle.esriSMSCircle;
            pointSym.Size = 2;

            //Create an RGB object for the fill part
            RgbColor colorLine = new RgbColor();
            colorLine.Red = 0;
            colorLine.Green = 0;
            colorLine.Blue = 0;

            pointSym.Color = colorLine;

            return pointSym;
        }

        /// <summary>
        /// Get an empty fill polygon symbol for default values or null values
        /// </summary>
        /// <returns></returns>
        public static ISimpleFillSymbol GetDefaultPolygonSymbol()
        {
            //Create empty poly symbol for default symbol
            ISimpleFillSymbol polySym = new SimpleFillSymbol();
            polySym.Style = esriSimpleFillStyle.esriSFSNull;
           
            //Create an RGB object for the outline
            RgbColor colorLine = new RgbColor();
            colorLine.Red = 0;
            colorLine.Green = 0;
            colorLine.Blue = 0;

            polySym.Outline.Color = colorLine;

            return polySym;
        }

        #endregion

        #region VALIDATION methods

        public static string ValidateStyleFile2(IMxDocument inMapDocument)
        {
            //Variable
            string loadedStylePath = string.Empty;

            //Create a style gallery object from current arc map document
            IStyleGalleryStorage styleStore = inMapDocument.StyleGallery as IStyleGalleryStorage;

            //Iterate through storage and detect existing files
            int[] fileIndexes = Enumerable.Range(0, styleStore.FileCount).ToArray();
            foreach (int indexes in fileIndexes)
            {
                //Detect already loaded styles
                if (styleStore.File[indexes].Contains(GSC_ProjectEditor.Constants.Styles.DefaultStyleFileName))
                {
                    //Keep style path
                    loadedStylePath = styleStore.File[indexes];
                    break;
                }
            }

            if (loadedStylePath == string.Empty)
            {
                GSC_ProjectEditor.Messages.ShowGenericErrorMessage("Missing Style file. Please add " + GSC_ProjectEditor.Constants.Styles.DefaultStyleFileName + " to current map document");
            }

            return loadedStylePath;
        }

        /// <summary>
        /// Will start a cursor within geoline feature class and validate fgdc_symbol field with same field within geoline_symbol table
        /// </summary>
        public static void ValidateGeolineFieldSymbol()
        {

            //Get a dictionnary of unique values from geoline_symbol table
            Dictionary<string, string> uniqueIDsAndSymbol = Tables.GetUniqueDicoValues(mGeolineSymbol, mGeolineID, mGeolineFGDC, null);

            //Get a feature cursor
            IFeatureCursor getFeatCursor = FeatureClass.GetFeatureCursor("Update", null, geolineFeature);

            //Get a field index for fgdc_symbols and ids
            int idIndex = getFeatCursor.FindField(geolineID);
            int symbolIndex = getFeatCursor.FindField(geolineSymbol);

            //Iterate through cursor
            IFeature getRow = null;
            while ((getRow = getFeatCursor.NextFeature()) != null)
            {
                //Get current information
                string currentID = getRow.get_Value(idIndex).ToString();
                string currentSymbol = getRow.get_Value(symbolIndex).ToString();

                //Parse current values of ids
                if (uniqueIDsAndSymbol.ContainsKey(currentID))
                {

                    //Parse current symbol value
                    if (uniqueIDsAndSymbol[currentID] != currentSymbol)
                    {

                        //Update the value with the one from the symbol table
                        getRow.set_Value(symbolIndex, uniqueIDsAndSymbol[currentID]);
                        getFeatCursor.UpdateFeature(getRow);
                    }
                }

            }

            //Release the cursor or else some lock could happen.
            System.Runtime.InteropServices.Marshal.ReleaseComObject(getFeatCursor);
        }

        /// <summary>
        /// Will start a cursor within geopoint feature class and validate fgdc_symbol field with same field within geopoint_symbol table
        /// </summary>
        public static void ValidateGeopointFieldSymbol()
        {

            //Get a dictionnary of unique values from geoline_symbol table
            Dictionary<string, string> uniqueIDsAndSymbol = Tables.GetUniqueDicoValues(tGeopointSymbol, tGeopointID, tGeopointFGDC, null);

            //Get a feature cursor
            IFeatureCursor getFeatCursor = FeatureClass.GetFeatureCursor("Update", null, geopointFeature);

            //Get a field index for fgdc_symbols and ids
            int idIndex = getFeatCursor.FindField(geopointID);
            int symbolIndex = getFeatCursor.FindField(geopointSymbol);

            //Iterate through cursor
            IFeature getRow = null;
            while ((getRow = getFeatCursor.NextFeature()) != null)
            {
                //Get current information
                string currentID = getRow.get_Value(idIndex).ToString();
                string currentSymbol = getRow.get_Value(symbolIndex).ToString();

                //Parse current values of ids
                if (uniqueIDsAndSymbol.ContainsKey(currentID))
                {

                    //Parse current symbol value
                    if (uniqueIDsAndSymbol[currentID] != currentSymbol)
                    {

                        //Update the value with the one from the symbol table
                        getRow.set_Value(symbolIndex, uniqueIDsAndSymbol[currentID]);
                        getFeatCursor.UpdateFeature(getRow);
                    }
                }

            }

            //Release the cursor or else some lock could happen.
            System.Runtime.InteropServices.Marshal.ReleaseComObject(getFeatCursor);
        }

        #endregion

        #region REMOVE methods

        /// <summary>
        /// Will remove all styles from a gallery storage, and give an output list of user values, in case it needs to be re-inserted to it.
        /// </summary>
        /// <param name="inputStyleGal">Gallery storage to clean</param>
        /// <returns></returns>
        public static List<string> RemoveAllStylesExceptGiven(IStyleGalleryStorage inputStyleGal, string exceptionStylePath)
        {
            //Variables
            List<string> userCurrentStyle = new List<string>();

            //Iterage through all styles within storage
            while (inputStyleGal.FileCount != 0)
            {
                //Remove current style
                userCurrentStyle.Add(inputStyleGal.File[0]);
                inputStyleGal.RemoveFile(inputStyleGal.File[0]);

            }

            //Add exception style back
            inputStyleGal.AddFile(exceptionStylePath);

            return userCurrentStyle;

        }

        /// <summary>
        /// Will remove any empty symbols from any symbology
        /// </summary>
        public static void RemoveEmptySymbols(IGeoFeatureLayer inputGeoFeature, IGeoFeatureLayer geoFeatureToCopyFrom)
        {
            try
            {
                //Variables
                List<string> fieldList = new List<string>(); //A list to contain all the field names that are used to symbolize the layer
                bool shownMessage = false; //Will be used to determine if warning message was showned to the user, or else it could appear many times.

                //Get unique renderer from inputs
                IUniqueValueRenderer uniqueRenderer = inputGeoFeature.Renderer as IUniqueValueRenderer;

                if (uniqueRenderer!=null)
                {
                    //Update renderer to remove empty values
                    IFeatureRendererUpdate upGeoFeature = uniqueRenderer as IFeatureRendererUpdate;

                    ///IF THIS LINE CRASHES, THE LAYER TRYING TO UPDATE DOESN't HAVE A SOURCE PATH
                    upGeoFeature.Update(inputGeoFeature as IFeatureLayer);

                    string currentHeader = "";

                    #region Need to reset labels, headers and descriptions, because the update method used above, wipes everything....
                    //Get list of all field that are used
                    for (int f = 0; f < uniqueRenderer.FieldCount; f++)
                    {
                        fieldList.Add(uniqueRenderer.get_Field(f));
                    }

                    //Get a list of all values from renderer
                    int newLayerNumberOfValues = uniqueRenderer.ValueCount;
                    //int oriLayerNumberOfValues = copyUniqueRenderer.ValueCount;// Will be used to find any mistmatch

                    for (int v = 0; v < newLayerNumberOfValues; v++)
                    {
                        //Copy label, symbol, header and description only if it's found in the original layer, could have a mistmatch
                        try
                        {
                            if (geoFeatureToCopyFrom != null)
                            {
                                IUniqueValueRenderer copyUniqueRenderer = geoFeatureToCopyFrom.Renderer as IUniqueValueRenderer;

                                //Update new label with original one
                                string currentValue = uniqueRenderer.get_Value(v);
                                string originalLabel = copyUniqueRenderer.get_Label(currentValue);
                                uniqueRenderer.set_Label(currentValue, originalLabel);

                                //Update new header with original one
                                string originalHeader = copyUniqueRenderer.get_Heading(currentValue);
                                currentHeader = uniqueRenderer.get_Heading(currentValue);
                                uniqueRenderer.set_Heading(currentValue, originalHeader);

                                //Update symbol
                                ISymbol originalSymbol = copyUniqueRenderer.get_Symbol(currentValue);
                                uniqueRenderer.set_Symbol(currentValue, originalSymbol);

                                //Update new description with original one
                                string originalDescription = copyUniqueRenderer.get_Description(currentValue);
                                uniqueRenderer.set_Description(currentValue, originalDescription);
                            }

                        }
                        catch (Exception)
                        {
                            if (!shownMessage)
                            {
                                MessageBox.Show(Properties.Resources.Error_SymbolMistmatch, Properties.Resources.Error_SymbolMistmatchMessageboxTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }

                        }

                    }

                    //Uncheck visibility for <all other values> symbol
                    uniqueRenderer.UseDefaultSymbol = false;
                }

                #endregion
            }
            catch (Exception removeEmptySymbolsException)
            {
                //MessageBox.Show(removeEmptySymbolsException.Message + removeEmptySymbolsException.StackTrace);
            }


        }

        #endregion 

        #region ADD methods

        /// <summary>
        /// Will add all styles from a list to a gallery storage
        /// </summary>
        /// <param name="inputStyleGal">Gallery storage to clean</param>
        /// <returns></returns>
        public static void AddStylesToStorage(IStyleGalleryStorage inputStyleGal, List<string> userStyles)
        {

            //Iterage through all styles within storage
            foreach (string stylePaths in userStyles)
            {
                inputStyleGal.AddFile(stylePaths);
            }

        }

        #endregion

        #region SET methods

        /// <summary>
        /// Will apply default properties to an input geopoint renderer. If no renderer is passed, a new one will be created.
        /// Doesn't set the field and field numbers.
        /// </summary>
        /// <param name="inRenderer">The renderer to modify, can be null to get a new one</param>
        public static IUniqueValueRenderer SetGeopointRendererProperties(IUniqueValueRenderer geopointRenderer, string pathToStyle = "")
        {
            //Start with main default and then apply geoline defaults
            IUniqueValueRenderer defaultGeopointRenderer = SetDefaultRendererProperties(geopointRenderer, GetDefaultPointSymbol() as ISymbol);

            if (pathToStyle != string.Empty)
            {
                defaultGeopointRenderer.LookupStyleset = pathToStyle;
                //Set rotation renderer
                IRotationRenderer rotRender = defaultGeopointRenderer as IRotationRenderer;
                rotRender.RotationField = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointAzimuth;
                rotRender.RotationType = esriSymbolRotationType.esriRotateSymbolGeographic; 
            }

            return defaultGeopointRenderer;
        }

        /// <summary>
        /// Will apply default properties to an input geoline renderer. If no renderer is passed, a new one will be created.
        /// Doesn't set the field and field numbers.
        /// </summary>
        /// <param name="inRenderer">The renderer to modify, can be null to get a new one</param>
        public static IUniqueValueRenderer SetGeolineRendererProperties(IUniqueValueRenderer geolineRenderer, string pathToStyle = "")
        {
            //Start with main default and then apply geoline defaults
            IUniqueValueRenderer defaultGeolineRenderer = SetDefaultRendererProperties(geolineRenderer, GetDefaultLineSymbol() as ISymbol);

            if (pathToStyle != string.Empty)
            {
                defaultGeolineRenderer.LookupStyleset = pathToStyle;
            }

           

            return defaultGeolineRenderer;
        }

        /// <summary>
        /// Will apply default properties to input renderer. If no renderer is passed, a new one will be created.
        /// Doesn't set the field and field numbers.
        /// </summary>
        /// <param name="inRenderer">The renderer to modify, can be null to get a new one</param>
        public static IUniqueValueRenderer SetGeopolyRendererProperties(IUniqueValueRenderer geopolyRenderer, string pathToStyle = "")
        {
            //Start with main default and then apply geopolys defaults
            IUniqueValueRenderer defaultGeopolyRenderer = SetDefaultRendererProperties(geopolyRenderer, GetDefaultPolygonSymbol() as ISymbol);

            if (pathToStyle != string.Empty)
            {
                defaultGeopolyRenderer.LookupStyleset = pathToStyle;
            }

            return defaultGeopolyRenderer;
        }

        /// <summary>
        /// Will apply default properties to input renderer. If no renderer is passed, a new one will be created.
        /// Doesn't set the field and field numbers.
        /// Sets Color Scheme, field delimiter, default symbol and lpokup style sheet.
        /// </summary>
        /// <param name="inRenderer">The renderer to modify, can be null to get a new one</param>
        public static IUniqueValueRenderer SetDefaultRendererProperties(IUniqueValueRenderer uniqueRenderer, ISymbol defaultSymbol)
        {
            if (uniqueRenderer == null)
            {
                uniqueRenderer = new UniqueValueRenderer();
            }

            uniqueRenderer.ColorScheme = GSC_ProjectEditor.Constants.Styles.DefaultColorScheme;
            uniqueRenderer.FieldDelimiter = GSC_ProjectEditor.Constants.Symbol4Layers.fieldDelimeter; //Add a field delimiter
            uniqueRenderer.DefaultSymbol = defaultSymbol; //Give a symbol object to default symbol
            uniqueRenderer.UseDefaultSymbol = true; //Null will be the default

            return uniqueRenderer;
        }

        #endregion

    }
}
