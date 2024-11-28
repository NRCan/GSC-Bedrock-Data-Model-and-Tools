using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Display;

namespace GSC_ProjectEditor.Utilities
{
    class ProjectSymbols
    {
        #region Main Variables

        //P_LABELS
        private const string plabels = GSC_ProjectEditor.Constants.Database.FLabel;
        private const string plabelField = GSC_ProjectEditor.Constants.DatabaseFields.FLabelID;
        private const string plabelFieldAlias = GSC_ProjectEditor.Constants.DatabaseFields.FLabelIDAlias;
        private const string plabelLayerName = GSC_ProjectEditor.Constants.Layers.label;

        //DOMAINS
        private const string muPID = GSC_ProjectEditor.Constants.DatabaseDomains.MapUnit;
        private const string agePrefixPID = GSC_ProjectEditor.Constants.DatabaseDomains.ageDesignator;
        private const string domYesNo = GSC_ProjectEditor.Constants.DatabaseDomains.BoolYesNo;
        private const string boolYesValue = GSC_ProjectEditor.Constants.DatabaseDomainsValues.BoolYes;
        private const string boolNoValue = GSC_ProjectEditor.Constants.DatabaseDomainsValues.BoolNo;
        private const string pointSymbolType = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypePoint;
        private const string lineSymbolType = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypeLine;

        //Legend generator table and fields
        private const string tLegendGen = GSC_ProjectEditor.Constants.Database.TLegendGene;
        private const string tLegendGenSym = GSC_ProjectEditor.Constants.DatabaseFields.LegendSymbol;
        private const string tLegendGenID = GSC_ProjectEditor.Constants.DatabaseFields.LegendLabelID;
        private const string tLegendGenType = GSC_ProjectEditor.Constants.DatabaseFields.LegendSymType;
        private const string lSymTypeFill = GSC_ProjectEditor.Constants.DatabaseDomainsValues.SymTypeFill;
        private const string tLegendGenName = GSC_ProjectEditor.Constants.DatabaseFields.LegendGISDisplay;
        private const string lSymMU = GSC_ProjectEditor.Constants.DatabaseFields.LegendMapUnit;
        private const string tLegendSymTheme = GSC_ProjectEditor.Constants.DatabaseFields.LegendItemType;

        //Symbol tables
        private const string tGeopoint = GSC_ProjectEditor.Constants.Database.TGeopointSymbol;
        private const string tGeopointID = GSC_ProjectEditor.Constants.DatabaseFields.TGeopointID;
        private const string tGeopointSelectCode = GSC_ProjectEditor.Constants.DatabaseFields.TGeopointSelectCode;
        private const string tGeopointLegendDesc = GSC_ProjectEditor.Constants.DatabaseFields.TGeopointLegendDesc;
        private const string tGeopointFGDC = GSC_ProjectEditor.Constants.DatabaseFields.TGeopointFGDC;

        //FEATURE GEO_POINT
        private const string geopoints = GSC_ProjectEditor.Constants.Database.FGeopoint;
        private const string geopointID = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointID;
        private const string geopointType = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointType;
        private const string geopointSubset = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointSubset;
        private const string geopointStrucAtt = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointStrucAtt;
        private const string geopointStrucGene = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointStrucGene;
        private const string geopointStrucYoung = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointStrucYoung;
        private const string geopointStrucMethod = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointStrucMethod;
        private const string geopointFGDC = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointFGDC;
        private const string geopointAzim = GSC_ProjectEditor.Constants.DatabaseFields.FGeopointAzimuth;
        private const string geopointLayerName = GSC_ProjectEditor.Constants.Layers.geopoint;

        //M_GEOLINE_SYMBOL
        private const string mGeolineSymbol = GSC_ProjectEditor.Constants.Database.TGeolineSymbol;
        private const string mGeolineLegendDesc = GSC_ProjectEditor.Constants.DatabaseFields.MGeolineLegendDescription;
        private const string mSelectCode = GSC_ProjectEditor.Constants.DatabaseFields.MGeolineSelectCode;
        private const string mGeolineID = GSC_ProjectEditor.Constants.DatabaseFields.MGeolineID;
        private const string mGeolineFGDC = GSC_ProjectEditor.Constants.DatabaseFields.MGeolineFGDC;


        //FEATURE GEO_LINES
        private const string geoline = GSC_ProjectEditor.Constants.Database.FGeoline;
        private const string geolineSubtype = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineSubtype;
        private const string geolineD1 = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineQualif;
        private const string geolineD2 = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineConf;
        private const string geolineD3 = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineAtt;
        private const string geolineD4 = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineGeneration;
        private const string geolineSym = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineFGDC;
        private const string geolineID = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineID;
        private const string geolineBound = GSC_ProjectEditor.Constants.DatabaseFields.FGeolineBoundary;
        private const string geolineSubContactCode = GSC_ProjectEditor.Constants.DatabaseSubtypes.FGeolineSubContact;
        private const string geolineSubThinUnitCode = GSC_ProjectEditor.Constants.DatabaseSubtypes.FGeolineSubThinUnit;
        private const string geolineSubUnitConstructCode = GSC_ProjectEditor.Constants.DatabaseSubtypes.FGeolineSubUnitConstruct;
        private const string geolineSubFaultCode = GSC_ProjectEditor.Constants.DatabaseSubtypes.FGeolineSubFault;


        //Other
        private Dictionary<string, object> inFieldValues = new Dictionary<string, object>(); //Will be used to add info in legend generator table
        private const string overprintKeyWord = GSC_ProjectEditor.Constants.ValueKeywords.labelOverprint;
        private string dicoMain = "Main";

        //Delegates and events
        public delegate void newLabelEventHandler(object sender, EventArgs e); //A delegate for execution events
        public event newLabelEventHandler newLabelAdded; //This event is triggered when a new label has been added within database
        public delegate void refreshPointStyleEventHandler(); //A delegate for execution events
        public event refreshPointStyleEventHandler refreshPointHasStarted; //This event is triggered when a new process is started with the execution button
        public event refreshPointStyleEventHandler refreshPointHasEnded; //This event is triggered when a QC process has ended.
        public delegate void refreshLineStyleEventHandler(); //A delegate for execution events
        public event refreshLineStyleEventHandler refreshLineHasStarted; //This event is triggered when a new process is started with the execution button
        public event refreshLineStyleEventHandler refreshLineHasEnded; //This event is triggered when a QC process has ended.

        #endregion

        #region LABELS

        /// <summary>
        /// Create a label template, from user map unit informations
        /// </summary>
        /// <param name="m_doc">Current mxd document</param>
        public void CreateLabelTemplate(IMxDocument m_doc)
        {
            //Validate style
            string labelStyle = GSC_ProjectEditor.Symbols.ValidateStyleFile2(ArcMap.Document);
            if (labelStyle != string.Empty)
            {
                string interGroupLayer = GSC_ProjectEditor.Properties.Resources.GroupLayerInterpretation;

                //Get the editor extension.
                IEditor3 gsc_editor = Utilities.EditSession.GetEditor3();

                //Create templates for the selected layer.
                ILayer editLayer = Utilities.MapDocumentFeatureLayers.GetFeatureLayerPreProcessing(plabels, interGroupLayer); //TODO validate if layer exist, if not add it, if yes select it?
                if (editLayer != null)
                {
                    gsc_editor.RemoveAllTemplatesInLayer(editLayer); //Clear all previous templates before building new ones
                    IEditTemplateFactory editTemplateFact = new EditTemplateFactoryClass();

                    //Create a new array to contain all templates.
                    IArray templateArray = new ArrayClass();

                    //Get domain values for map units
                    Dictionary<string, string> muDico = GSC_ProjectEditor.Domains.GetDomDico(muPID, "Description");

                    //Get list of GIS display name
                    string mapUnitQuery = GSC_ProjectEditor.Constants.DatabaseFields.LegendItemType + " = '" + GSC_ProjectEditor.Constants.DatabaseDomainsValues.legendItemMapUnit + "'";
                    Dictionary<string, List<string>> uniqueDicoValues = GSC_ProjectEditor.Tables.GetUniqueFieldValues(GSC_ProjectEditor.Constants.Database.TLegendGene, GSC_ProjectEditor.Constants.DatabaseFields.LegendLabelID, mapUnitQuery, true, GSC_ProjectEditor.Constants.DatabaseFields.LegendGISDisplay);

                    //Add all selected lines from database
                    foreach (KeyValuePair<string, string> muKeyValue in muDico)
                    {
                        string templateDisplayName = muKeyValue.Key;
                        if (uniqueDicoValues["Main"].Contains(muKeyValue.Value))
                        {
                            int indexGISDisplayName = uniqueDicoValues["Main"].IndexOf(muKeyValue.Value);
                            templateDisplayName = uniqueDicoValues["Tag"][indexGISDisplayName];
                        }

                        IEditTemplate newEditTemplate = editTemplateFact.Create(templateDisplayName, editLayer);
                        templateArray.Add(newEditTemplate);

                        #region manage subytpes and domains within layer

                        //Manage label
                        newEditTemplate.SetDefaultValue(plabelField, muKeyValue.Value, false); //Add value for given field

                        #endregion

                    }

                    //Add all templates to editor.
                    gsc_editor.AddTemplates(templateArray);

                    //Update style
                    updateLabelStyle(labelStyle);

                }
                else
                {
                    MessageBox.Show(Properties.Resources.Error_GSC_NoFeaturePlabel);
                } 
            }


        }

        /// <summary>
        /// Update label symbols from project style
        /// </summary>
        public void updateLabelStyle(string labelStylePath)
        {
            try
            {
                string interGroupLayer = GSC_ProjectEditor.Properties.Resources.GroupLayerInterpretation;

                //Get geoline feature layer
                IFeatureLayer getLayer = Utilities.MapDocumentFeatureLayers.GetFeatureLayerPreProcessing(plabels, interGroupLayer);

                //Access other attributes of layer with geofeaturelayer
                IGeoFeatureLayer getGeoFL = (IGeoFeatureLayer)getLayer;

                //Create a renderer as for style
                IUniqueValueRenderer renderer = new UniqueValueRenderer();

                //Get current renderer and list all values
                IUniqueValueRenderer currentRenderer = getGeoFL.Renderer as IUniqueValueRenderer;
                List<string> currentValues = new List<string>();
                if (currentRenderer != null)
                {
                    renderer = currentRenderer;

                    for (int v = 0; v < currentRenderer.ValueCount; v++)
                    {
                        currentValues.Add(currentRenderer.Value[v]);
                    }
                }

                //Create point symbol for default symbol (blue dot)
                ISimpleMarkerSymbol pointSym = new SimpleMarkerSymbol();
                pointSym.Style = esriSimpleMarkerStyle.esriSMSCircle;
                pointSym.Size = 4.0;
                RgbColor rgbDefault = new RgbColor();
                rgbDefault.Red = 0;
                rgbDefault.Green = 92;
                rgbDefault.Blue = 230;
                pointSym.Color = rgbDefault;

                //Set some renderer properties
                renderer.ColorScheme = GSC_ProjectEditor.Constants.Styles.DefaultColorScheme;
                renderer.FieldCount = 1; //Only one field will determine symbol
                renderer.Field[0] = plabelField; //Set the field to sym field 
                renderer.DefaultSymbol = pointSym as ISymbol; //Give a symbol object to default symbol
                renderer.UseDefaultSymbol = true; //Null will be the default

                //Assign values into unique renderer
                string mapUnitQuery = GSC_ProjectEditor.Constants.DatabaseFields.LegendItemType + " = '" + GSC_ProjectEditor.Constants.DatabaseDomainsValues.legendItemMapUnit + "'";
                List<Tuple<string, string>> uniqueDicoValues = GSC_ProjectEditor.Tables.GetUniqueDoubleFieldValues(GSC_ProjectEditor.Constants.Database.TLegendGene, new Tuple<string, string>(GSC_ProjectEditor.Constants.DatabaseFields.LegendLabelID, GSC_ProjectEditor.Constants.DatabaseFields.LegendGISDisplay), mapUnitQuery);

                foreach (Tuple<string, string> valueToAdd in uniqueDicoValues)
                {
                    if (!currentValues.Contains(valueToAdd.Item1))
                    {
                        //Add new value
                        renderer.AddValue(valueToAdd.Item1, plabelFieldAlias, pointSym as ISymbol);
                        renderer.set_Label(valueToAdd.Item1, valueToAdd.Item2);
                        renderer.set_Symbol(valueToAdd.Item1, pointSym as ISymbol);
                    }
                    else
                    {
                        //Refresh only if user wants it
                        if (!Properties.Settings.Default.KeepCustomSymbols)
                        {
                            //renderer.AddValue(valueToAdd.Item1, plabelFieldAlias, pointSym as ISymbol);
                            renderer.set_Label(valueToAdd.Item1, valueToAdd.Item2);
                            renderer.set_Symbol(valueToAdd.Item1, pointSym as ISymbol);
                        }
                    }

                }

                //Get list of map unit codes and their related FGDC
                Dictionary<string, string> codeSymbol = GSC_ProjectEditor.Tables.GetUniqueDicoValues(tLegendGen, tLegendGenID, tLegendGenSym, null);

                for (int j = 0; j <= renderer.ValueCount - 1; j++)
                {
                    //Get current value
                    string currentItem = renderer.get_Value(j);

                    if (currentItem != "")
                    {
                        //Get current value related CMYK color
                        if (codeSymbol.ContainsKey(currentItem))
                        {
                            //try catch because some empty cells values can enter the dictionary and crashing the sequence.
                            try
                            {
                                //Get Color object
                                IFillSymbol relatedFillSymbol = Utilities.MapDocumentSymbol.GetMatchingPolygonSymbol(codeSymbol[currentItem], labelStylePath);

                                //Refresh symbol only if user wants it.
                                if (relatedFillSymbol != null && !Properties.Settings.Default.KeepCustomSymbols)
                                {
                                    IColor currentColor = relatedFillSymbol.Color;
                                    ISimpleMarkerSymbol newSimpleColor = new SimpleMarkerSymbol();
                                    newSimpleColor.Style = esriSimpleMarkerStyle.esriSMSCircle;
                                    newSimpleColor.Size = 4.0;
                                    newSimpleColor.Color = currentColor;
                                    renderer.set_Symbol(currentItem, newSimpleColor as ISymbol);
                                }


                            }
                            catch (Exception e)
                            {
                                //MessageBox.Show(e.StackTrace);
                            }

                        }
                        else
                        {
                            //Do nothing, the blue symbol will stay as default in those cases
                        }

                    }
                }

                //Set renderer into layer
                getGeoFL.Renderer = (IFeatureRenderer)renderer;
                getGeoFL.DisplayField = plabelField;

                //Refresh TOC
                ArcMap.Document.ActivatedView.Refresh();
                ArcMap.Document.ActivatedView.ContentsChanged();
                ArcMap.Document.UpdateContents();

            }
            catch (Exception updateStyleException)
            {
                int lineNumber = GSC_ProjectEditor.Exceptions.LineNumber(updateStyleException);
                MessageBox.Show("updateStyleException (" + lineNumber.ToString() + "): " + updateStyleException.Message);
            }


        }

        /// <summary>
        /// Will refresh the label layer symbols and templates
        /// </summary>
        public void RefreshLabelSymbols()
        {
            //Recreate templates; it'll also trigger layer refresh from within
            CreateLabelTemplate(ArcMap.Application.Document as IMxDocument);
        }

        #endregion

        #region GEOPOINTS

        /// <summary>
        /// Will create the templates for the geopoints
        /// </summary>
        /// <param name="iMxDocument"></param>
        public void CreatePointTemplate(IMxDocument iMxDocument)
        {
            try
            {
                //Validate style
                string pointStyle = GSC_ProjectEditor.Symbols.ValidateStyleFile2(ArcMap.Document);
                if (pointStyle != string.Empty)
                {
                    string interGroupLayer = GSC_ProjectEditor.Properties.Resources.GroupLayerInterpretation;

                    //Get a list of selected geolines from legend table
                    string queryToSelectGeopoints = tLegendGenType + " LIKE '%" + pointSymbolType + "%'";
                    Dictionary<string, Tuple<string, string>> currentPoint = GSC_ProjectEditor.Tables.GetUniqueDicoTripleFieldValues(tLegendGen, new Tuple<string, string, string>(tLegendGenID, tLegendGenName, tLegendGenSym), queryToSelectGeopoints);

                    //Get the editor extension.
                    IEditor3 gsc_editor = Utilities.EditSession.GetEditor3();

                    //Create templates for the selected layer.
                    ILayer editLayer = Utilities.MapDocumentFeatureLayers.GetFeatureLayerPreProcessing(geopoints, interGroupLayer); //TODO validate if layer exist, if not add it, if yes select it?
                    if (editLayer != null)
                    {
                        gsc_editor.RemoveAllTemplatesInLayer(editLayer); //Clear all previous templates before building new ones
                        IEditTemplateFactory editTemplateFact = new EditTemplateFactoryClass();

                        //Create a new array to contain all templates.
                        IArray templateArray = new ArrayClass();

                        //Add all selected points from database
                        foreach (KeyValuePair<string, Tuple<string, string>> kv in currentPoint)
                        {


                            #region manage subytpes and domains within layer
                            if (kv.Key != GSC_ProjectEditor.Constants.FieldDefaults.invalidGeopointIDValue && kv.Key.Length == 13)
                            {
                                //Get legend description and symbol code
                                string getDescription = kv.Value.Item1;
                                string getSymCode = kv.Value.Item2;

                                //Validate description
                                if (getDescription == GSC_ProjectEditor.Properties.Resources.Error_NotInScientificLanguageDictionary)
                                {
                                    getDescription = getDescription + ", " + kv.Key;
                                }

                                //Create the new template
                                IEditTemplate newEditTemplate = editTemplateFact.Create(getDescription, editLayer);
                                templateArray.Add(newEditTemplate);

                                //Parse geolineID to extract information about domains
                                string getType = kv.Key.Substring(0, 1);
                                string getSubset = kv.Key.Substring(1, 4);
                                string getAtt = kv.Key.Substring(5, 2);
                                string getGene = kv.Key.Substring(7, 2);
                                string getYoung = kv.Key.Substring(9, 2);
                                string getMethod = kv.Key.Substring(11, 2);

                                //Manage subtype
                                try
                                {
                                    newEditTemplate.SetDefaultValue(geopointType, getType, true); //Add value for given field
                                }
                                catch (Exception)
                                {

                                    MessageBox.Show(geopointType + "; " + getType);
                                }


                                //Manage subset
                                newEditTemplate.SetDefaultValue(geopointSubset, getSubset, false);

                                //Manage attitude
                                newEditTemplate.SetDefaultValue(geopointStrucAtt, getAtt, false);

                                //Manage generation
                                newEditTemplate.SetDefaultValue(geopointStrucGene, getGene, false);

                                //Manage younging
                                newEditTemplate.SetDefaultValue(geopointStrucYoung, getYoung, false);

                                //Manage method
                                newEditTemplate.SetDefaultValue(geopointStrucMethod, getMethod, false);

                                //Manage GeopointID (force value or else it'll stays like preset from subtype)
                                newEditTemplate.SetDefaultValue(geopointID, kv.Key, false);

                                //Manage FGDC Symbol
                                newEditTemplate.SetDefaultValue(geopointFGDC, getSymCode, false);
                            }


                            #endregion

                        }


                        //Add all templates to editor.
                        gsc_editor.AddTemplates(templateArray);

                        //Update style
                        updatePointStyle(pointStyle);
                    }
                    else
                    {
                        MessageBox.Show(Properties.Resources.Error_GSC_NoGeoline);
                    }  
                }


            }
            catch (Exception CreateTemplateError)
            {
                MessageBox.Show("DockableWindowAddSymbolPoint.CreateTemplate:" + CreateTemplateError);
            }
        }

        /// <summary>
        /// Update geopoint symbols from project style file.
        /// </summary>
        public void updatePointStyle(string pointStylePath)
        {
            //Get unique list of geoline ids within geoline
            SortedDictionary<string, List<string>> uniqueGeopointList = GSC_ProjectEditor.Tables.GetAllDoubleUniqueFieldValues(geopoints, geopointID, geopointFGDC, null);

            //Get geopoint feature layer
            IFeatureLayer getLayer = Utilities.MapDocumentFeatureLayers.GetFeatureLayerPreProcessing(geopoints, GSC_ProjectEditor.Properties.Resources.GroupLayerInterpretation);

            //Access other attributes of layer with geofeaturelayer
            IGeoFeatureLayer getGeoFL = getLayer as IGeoFeatureLayer;

            //Verify if layer is already symbolized with wanted style, if not, will create new renderer for it
            bool validated = validatePointRenderer(getGeoFL, pointStylePath);

            //Clean current set of renderer or values not in project or digitized
            if (validated)
            {
                //Variables
                bool emptySymbolTrigger = false;

                //Get the unique renderer style from 
                IUniqueValueRenderer getUniqueRenderPoint = getGeoFL.Renderer as UniqueValueRenderer;

                //Get number of current symbols within layer
                int idCountPnt = getUniqueRenderPoint.ValueCount;

                //Get list of values
                List<string> currentGeopointsValues = new List<string>();
                if (getUniqueRenderPoint != null)
                {
                    for (int v = 0; v < getUniqueRenderPoint.ValueCount; v++)
                    {
                        currentGeopointsValues.Add(getUniqueRenderPoint.Value[v]);
                    }
                }

                //Detect if symbol exists
                for (int idxValue = 0; idxValue < idCountPnt; idxValue++)
                {
                    //Get full value
                    string fullValuePnt = getUniqueRenderPoint.get_Value(idxValue);

                    //Filter value to retrieve geolineID
                    char[] charValue = GSC_ProjectEditor.Constants.Symbol4Layers.fieldDelimeter.ToCharArray();
                    string[] splitedValue = fullValuePnt.Split(charValue);
                    string geopointIDValue = splitedValue[splitedValue.Count() - 1];
                    string geopointSymbolValue = splitedValue[0];

                    if (uniqueGeopointList.Count >= 1 && !currentGeopointsValues.Contains(fullValuePnt))
                    {
                        //Remove
                        getUniqueRenderPoint.RemoveValue(fullValuePnt);

                        //Reset count
                        idCountPnt = getUniqueRenderPoint.ValueCount;
                        idxValue = idxValue - 1;

                    }
                    else
                    {
                        //The id might be right, but not the symbol
                        if (!Properties.Settings.Default.KeepCustomSymbols && uniqueGeopointList.ContainsKey(geopointIDValue) && !uniqueGeopointList[geopointIDValue].Contains(geopointSymbolValue))
                        {
                            //Remove
                            getUniqueRenderPoint.RemoveValue(fullValuePnt);

                            //Reset count
                            idCountPnt = getUniqueRenderPoint.ValueCount;
                            idxValue = idxValue - 1;
                        }
                    }
                }

                //Iterate through all values
                foreach (KeyValuePair<string, List<string>> geopointDefinition in uniqueGeopointList)
                {

                    foreach (string symbolPoints in geopointDefinition.Value)
                    {

                        //keep current value for symbol and description, needs to be update if empty or missing
                        string geopointSymbol = symbolPoints;
                        string geopointDescription = string.Empty;


                        //Create the new unique value
                        string newUniqueValuePoint = geopointSymbol + GSC_ProjectEditor.Constants.Symbol4Layers.fieldDelimeter + geopointDefinition.Key;

                        //Calculate label for new symbol
                        Tuple<string, string> matchLegendPoint;
                        try
                        {
                            //Try a match with legend generator table first
                            matchLegendPoint = GSC_ProjectEditor.Tables.GetUniqueDoubleFieldValues(tLegendGen, new Tuple<string, string>(tLegendGenSym, tLegendGenName), tLegendGenID + " = '" + geopointDefinition.Key + "'")[0];
                        }
                        catch (Exception)
                        {
                            //If value isn't found inside legend table, get information from symbol table
                            matchLegendPoint = GSC_ProjectEditor.Tables.GetUniqueDoubleFieldValues(tGeopoint, new Tuple<string, string>(tGeopointFGDC, tGeopointLegendDesc), tGeopointID + " = '" + geopointDefinition.Key + "'")[0];
                            matchLegendPoint = new Tuple<string, string>(geopointSymbol, matchLegendPoint.Item2);
                        }

                        //Validate empty symbol code
                        if (geopointSymbol == string.Empty)
                        {
                            newUniqueValuePoint = matchLegendPoint.Item1 + GSC_ProjectEditor.Constants.Symbol4Layers.fieldDelimeter + geopointDefinition.Key;
                            geopointSymbol = GSC_ProjectEditor.Constants.Styles.InvalidPoint_FGDC; //Add invalid symbol style
                            emptySymbolTrigger = true;
                        }

                        //Validate missing description, add geopointID if missing.
                        if (matchLegendPoint.Item2 == GSC_ProjectEditor.Properties.Resources.Error_NotInScientificLanguageDictionary)
                        {
                            geopointDescription = matchLegendPoint.Item2 + ", " + geopointDefinition.Key;

                        }
                        else
                        {
                            geopointDescription = matchLegendPoint.Item2;
                        }


                        //Get match symbol
                        IMarkerSymbol symPoint = null; //Create a global line symbol object (it can takes whatever line symbol object type)
                        symPoint = Utilities.MapDocumentSymbol.GetMatchingPointSymbol(geopointSymbol, pointStylePath);

                        //Set symbol to the match
                        if (symPoint != null)
                        {
                            if (!Properties.Settings.Default.KeepCustomSymbols)
                            {
                                getUniqueRenderPoint.AddValue(newUniqueValuePoint, GSC_ProjectEditor.Constants.Symbol4Layers.geopointLabelFieldName, symPoint as ISymbol);
                                getUniqueRenderPoint.Label[newUniqueValuePoint] = geopointDescription; //Apply legend description as label
                                //getUniqueRenderPoint.Symbol[newUniqueValuePoint] = symPoint as ISymbol;
                            }
                            else
                            {
                                if (!currentGeopointsValues.Contains(newUniqueValuePoint))
                                {
                                    //Create a new empty line symbol
                                    ISimpleMarkerSymbol symPointDef = null;
                                    symPointDef = GSC_ProjectEditor.Symbols.GetDefaultPointSymbol();
                                    getUniqueRenderPoint.AddValue(newUniqueValuePoint, GSC_ProjectEditor.Constants.Symbol4Layers.geopointLabelFieldName, symPointDef as ISymbol);
                                    getUniqueRenderPoint.Label[newUniqueValuePoint] = geopointDescription; //Apply legend description as label

                                }
                            }

                        }
                        else
                        {
                            //If symbol is not the right one, just do like an empty symbol and show invalid symbol
                            geopointSymbol = GSC_ProjectEditor.Constants.Styles.InvalidPoint_FGDC;
                            symPoint = Utilities.MapDocumentSymbol.GetMatchingPointSymbol(geopointSymbol, pointStylePath);

                            if (!Properties.Settings.Default.KeepCustomSymbols && symPoint != null)
                            {
                                getUniqueRenderPoint.Symbol[newUniqueValuePoint] = symPoint as ISymbol;
                            }

                            MessageBox.Show(Properties.Resources.Error_SymbolMistmatchWithStyle + " " + newUniqueValuePoint, Properties.Resources.Error_GSC_MissingSymbol, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }

                }

                if (emptySymbolTrigger)
                {
                    MessageBox.Show(GSC_ProjectEditor.Properties.Resources.Error_SymbolMistmatch2, GSC_ProjectEditor.Properties.Resources.Error_SymbolMistmatchMessageboxTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            //Refresh TOC
            try
            {
                ArcMap.Document.ActivatedView.ContentsChanged();
                ArcMap.Document.UpdateContents();
                ArcMap.Document.ActivatedView.Refresh();
            }
            catch (Exception)
            {

            }


            ////Call event to change cusor icon.
            //refreshPointHasEnded();
        }

        /// <summary>
        /// Will refresht the template and the layer symbols in the TOC
        /// </summary>
        public void RefreshGeopointSymbols()
        {
            ////Call event to change cusor icon.
            //refreshPointHasStarted();

            //Validate geolineIDs to make sure everything is correctly synchronised
            GSC_ProjectEditor.IDs.CalculateGeopointID();

            //Validate if all feature geopoint ids are in geoline_symbol table
            List<string> geolpointIDList = GSC_ProjectEditor.Tables.GetUniqueFieldValues(geopoints, geopointID, null, false, null)[dicoMain];
            UpdateGeopointSymbolInLegend(geolpointIDList);

            //Recreate templates; it'll also trigger layer refresh from within
            CreatePointTemplate(ArcMap.Application.Document as IMxDocument);
        }

        /// <summary>
        /// This method will help detect if current kayer is symbolized correctly.
        /// Will add a custom renderer if nothing exists.
        /// </summary>
        /// <param name="getGeoFL">The layer object to verify renderer from</param>
        public bool validatePointRenderer(IGeoFeatureLayer layerToAddRenderer, string pointStylePath)
        {
            try
            {
                //Get the unique renderer style from 
                IUniqueValueRenderer getUniqueRender = layerToAddRenderer.Renderer as UniqueValueRenderer;

                //Create line symbol for default symbol
                ISimpleMarkerSymbol pointSym = GSC_ProjectEditor.Symbols.GetDefaultPointSymbol();

                //Get proper interface unique id
                string currentUID = layerToAddRenderer.RendererPropertyPageClassID.Value.ToString();
                string wantedUID = GSC_ProjectEditor.Constants.GUIDs.UIDSymbolUniqueValuesMultipleField;

                //Count entities from current renderer
                int count = 0;
                int fieldCount = 0;
                bool noRenderer = false; //A joker bool value, this will be used in case arc map says currentUID and wantedUID are the same, when they are not...
                try
                {
                    count = getUniqueRender.ValueCount;
                    fieldCount = getUniqueRender.FieldCount;
                }
                catch
                {
                    noRenderer = true;
                }

                //If the renderer is not set on the right interface redo from scratch, or Arc tells that the UID is the right one but the renderer doesn't exists.
                if (wantedUID != currentUID || noRenderer == true)
                {

                    //Force interface to unique values multiple fields renderer
                    UID pUID = new UIDClass();
                    pUID.Value = wantedUID;
                    layerToAddRenderer.RendererPropertyPageClassID = pUID as UIDClass;

                    //Create a renderer for style
                    getUniqueRender = new UniqueValueRenderer();

                    //Set some renderer properties
                    SetPointRendererProperties(getUniqueRender, pointStylePath);

                }
                else
                {

                    //Detect if the renderer is set on the right field
                    if (getUniqueRender.FieldCount != 2)
                    {
                        //Set some renderer properties
                        SetPointRendererProperties(getUniqueRender, pointStylePath);
                    }

                }

                //Persist change
                layerToAddRenderer.Renderer = getUniqueRender as IFeatureRenderer;

                return true;
            }
            catch (Exception validatePointRendererError)
            {
                MessageBox.Show("validatePointRendererError: " + validatePointRendererError.StackTrace);

                return false;
            }
        }

        /// <summary>
        /// Will apply geopoint specific properties to input renderer
        /// </summary>
        /// <param name="inRenderer">The renderer to modify</param>
        /// <param name="defaultSymbol">The default symbol for default values</param>
        public void SetPointRendererProperties(IUniqueValueRenderer inRenderer, string pointStylePath)
        {
            IUniqueValueRenderer geopointRenderer = GSC_ProjectEditor.Symbols.SetGeopointRendererProperties(inRenderer, pointStylePath);

            if (geopointRenderer != null)
            {
                geopointRenderer.FieldCount = 2;//Only one field will determine symbol
                geopointRenderer.set_Field(0, geopointFGDC); //Set the field to sym field 
                geopointRenderer.set_Field(1, geopointID); //Set the second field to be geolineID
            }


        }

        /// <summary>
        /// Will update symbole geoline table "selectCode" field to 1, for each given geolineIDs in list.
        /// </summary>
        /// <param name="geolineIDList">List of ids to turn on in symbol geoline table</param>
        public void UpdateGeopointSymbolInLegend(List<string> geolpointIDList)
        {
            //Variable
            List<Dictionary<string, object>> newRowsToAdd = new List<Dictionary<string, object>>();

            //Get a list of unique ids stored in legend table
            //string geolineQuery = tLegendGenType + " = '" + pointSymbolType + "'";
            List<string> legendIds = GSC_ProjectEditor.Tables.GetUniqueFieldValues(tLegendGen, tLegendGenID, null, false, null)[dicoMain];

            //Build a new row dictionnary if ids are not found in legend table
            foreach (string geo in geolpointIDList)
            {
                if (!legendIds.Contains(geo) && geo != string.Empty)
                {
                    //Get an initialization symbol code from the feature itself since it was found inside it
                    string currentFGDC = "";

                    //Get an initialization desription in symbol table
                    string currentDescription = "";
                    try
                    {
                        currentDescription = GSC_ProjectEditor.Tables.GetFieldValues(tGeopoint, tGeopointLegendDesc, tGeopointID + " = '" + geo + "'")[0];
                    }
                    catch (Exception)
                    {

                        currentDescription = GSC_ProjectEditor.Properties.Resources.Error_NotInScientificLanguageDictionary;
                    }


                    try
                    {
                        //It could crash if value is null
                        List<string> currentFGDCList = GSC_ProjectEditor.Tables.GetFieldValues(geopoints, geopointFGDC, geopointID + " = '" + geo + "'");
                        List<string> currentUniqueFGDCList = currentFGDCList.Distinct().ToList();
                        currentFGDC = currentFGDCList[0];

                        //Check whether multiple code were found inside the feature
                        if (currentUniqueFGDCList.Count > 1)
                        {
                            //Build full string of unique value to show user
                            string uniqueListString = "";
                            List<string> uniqueList = new List<string>();
                            foreach (string fgdc in currentFGDCList)
                            {
                                if (!uniqueList.Contains(fgdc))
                                {
                                    uniqueList.Add(fgdc);
                                    uniqueListString = uniqueListString + fgdc + "; ";
                                }
                            }

                            //Show warning to user
                            string warningMessage = Properties.Resources.Warning_MultipleFGDCFound + " " + currentDescription + ": " + uniqueListString + ".";
                            MessageBox.Show(warningMessage, Properties.Resources.Warning_MultiFGDCFoundTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        }
                    }
                    catch (Exception)
                    {
                        //Get value from symbol table instead
                        currentFGDC = GSC_ProjectEditor.Tables.GetFieldValues(tGeopoint, tGeopointFGDC, tGeopointID + " = '" + geo + "'")[0];
                    }

                    //Init sym type
                    string symType = pointSymbolType;

                    //Add to list
                    Dictionary<string, object> rowDico = new Dictionary<string, object>();
                    if (currentFGDC != null)
                    {
                        rowDico[tLegendGenID] = geo;
                        rowDico[tLegendGenName] = currentDescription;
                        rowDico[tLegendGenSym] = currentFGDC;
                        rowDico[tLegendGenType] = symType;

                        newRowsToAdd.Add(rowDico);
                    }
                }

            }

            //Add new rows in legend table if needed
            if (newRowsToAdd.Count != 0)
            {
                GSC_ProjectEditor.Tables.AddRowsWithValues(tLegendGen, newRowsToAdd);
            }


        }

        #endregion

        #region GEOLINES

        /// <summary>
        /// Creates template of geoline
        /// </summary>
        /// <param name="m_doc"></param>
        public void CreateLineTemplate(IMxDocument m_doc)
        {
            try
            {
                //Validate style
                string lineStyle = GSC_ProjectEditor.Symbols.ValidateStyleFile2(ArcMap.Document);
                if (lineStyle != string.Empty)
                {
                    string interGroupLayer = GSC_ProjectEditor.Properties.Resources.GroupLayerInterpretation;

                    //Get a list of selected geolines from legend table
                    string queryToSelectGeolines = tLegendGenType + " LIKE '%" + lineSymbolType + "%'";
                    Dictionary<string, Tuple<string, string>> currentLines = GSC_ProjectEditor.Tables.GetUniqueDicoTripleFieldValues(tLegendGen, new Tuple<string, string, string>(tLegendGenID, tLegendGenName, tLegendGenSym), queryToSelectGeolines);

                    //Get the editor extension.
                    IEditor3 gsc_editor = Utilities.EditSession.GetEditor3();

                    //Create templates for the selected layer.
                    ILayer editLayer = Utilities.MapDocumentFeatureLayers.GetFeatureLayerPreProcessing(geoline, interGroupLayer); //TODO validate if layer exist, if not add it, if yes select it?
                    if (editLayer != null)
                    {
                        gsc_editor.RemoveAllTemplatesInLayer(editLayer); //Clear all previous templates before building new ones
                        IEditTemplateFactory editTemplateFact = new EditTemplateFactoryClass();

                        //Create a new array to contain all templates.
                        IArray templateArray = new ArrayClass();

                        //Add all selected lines from database
                        foreach (KeyValuePair<string, Tuple<string, string>> kv in currentLines)
                        {

                            #region manage subytpes and domains within layer
                            if (kv.Key != GSC_ProjectEditor.Constants.FieldDefaults.invalidGeolineIDValue && kv.Key.Length == 12)
                            {
                                //Get legend description and symbol code
                                string getDescription = kv.Value.Item1;
                                string getSymCode = kv.Value.Item2;


                                //Validate description
                                if (getDescription == GSC_ProjectEditor.Properties.Resources.Error_NotInScientificLanguageDictionary)
                                {
                                    getDescription = getDescription + ", " + kv.Key;
                                }

                                //Create the new template
                                IEditTemplate newEditTemplate = editTemplateFact.Create(getDescription, editLayer);
                                templateArray.Add(newEditTemplate);

                                //Parse geolineID to extract information about domains
                                string getType = kv.Key.Substring(0, 2);
                                string getQualifier = kv.Key.Substring(2, 4);
                                string getConfidence = kv.Key.Substring(6, 2);
                                string getAttitude = kv.Key.Substring(8, 2);
                                string getGeneration = kv.Key.Substring(10, 2);

                                //Manage subtype
                                newEditTemplate.SetDefaultValue(geolineSubtype, getType, true); //Add value for given field

                                //Manage Qualifier
                                newEditTemplate.SetDefaultValue(geolineD1, getQualifier, false);

                                //Manage Confidence
                                newEditTemplate.SetDefaultValue(geolineD2, getConfidence, false);

                                //Manage Attitude
                                newEditTemplate.SetDefaultValue(geolineD3, getAttitude, false);

                                //Manage Generation
                                newEditTemplate.SetDefaultValue(geolineD4, getGeneration, false);

                                //Manage GeolineID (force value or else it'll stays like preset from subtype)
                                newEditTemplate.SetDefaultValue(geolineID, kv.Key, false);

                                //Manage FGDC Symbol
                                newEditTemplate.SetDefaultValue(geolineSym, getSymCode, false);
                            }

                            #endregion

                        }


                        //Add all templates to editor.
                        gsc_editor.AddTemplates(templateArray);


                        //Update style
                        updateLineStyle(lineStyle);
                    }
                    else
                    {
                        MessageBox.Show(Properties.Resources.Error_GSC_NoGeoline);
                    }
                }

            }
            catch (Exception CreateTemplateError)
            {
                MessageBox.Show("dockableWindowSelectGeoline.CreateTemplate:" + CreateTemplateError.StackTrace);
            }


        }

        /// <summary>
        /// Update geoline symbols from project style file.
        /// </summary>
        public void updateLineStyle(string stylePath)
        {
            string interGroupLayer = GSC_ProjectEditor.Properties.Resources.GroupLayerInterpretation;

            //Get unique list of geoline ids within geoline
            SortedDictionary<string, List<string>> uniqueGeolineList = GSC_ProjectEditor.Tables.GetAllDoubleUniqueFieldValues(geoline, geolineID, geolineSym, null);

            //Get geoline feature layer
            IFeatureLayer getLayer = Utilities.MapDocumentFeatureLayers.GetFeatureLayerPreProcessing(geoline, interGroupLayer);

            //Access other attributes of layer with geofeaturelayer
            IGeoFeatureLayer getGeoFL = getLayer as IGeoFeatureLayer;

            //Verify if layer is already symbolized with wanted style, if not, will create new renderer for it
            bool validated = validateLineRenderer(getGeoFL, stylePath);

            //Clean current set of renderer or values not in project or digitized
            if (validated)
            {
                //Variables
                bool emptySymbolTrigger = false;

                //Get the unique renderer style from 
                IUniqueValueRenderer getUniqueRender = getGeoFL.Renderer as UniqueValueRenderer;

                //Get number of current symbols within layer
                int idCount = getUniqueRender.ValueCount;

                //Get list of values
                List<string> currentGeolineValues = new List<string>();
                if (getUniqueRender != null)
                {
                    for (int v = 0; v < getUniqueRender.ValueCount; v++)
                    {
                        currentGeolineValues.Add(getUniqueRender.Value[v]);
                    }
                }


                //Detect if symbol exists
                for (int idxValueLine = 0; idxValueLine < idCount; idxValueLine++)
                {
                    //Get full value
                    string fullValue = getUniqueRender.get_Value(idxValueLine);

                    //Filter value to retrieve geolineID
                    char[] charValue = GSC_ProjectEditor.Constants.Symbol4Layers.fieldDelimeter.ToCharArray();
                    string[] splitedValue = fullValue.Split(charValue);
                    string geolineIDValue = splitedValue[splitedValue.Count() - 1];
                    string geolineSymbolValue = splitedValue[0];

                    if (uniqueGeolineList.Count > 1 && !currentGeolineValues.Contains(fullValue))
                    {
                        //Remove
                        getUniqueRender.RemoveValue(fullValue);

                        //Reset count
                        idCount = getUniqueRender.ValueCount;
                        idxValueLine = idxValueLine - 1;

                    }
                    else
                    {
                        //The id might be right, but not the symbol
                        if (!Properties.Settings.Default.KeepCustomSymbols && uniqueGeolineList.ContainsKey(geolineIDValue) && !uniqueGeolineList[geolineIDValue].Contains(geolineSymbolValue))
                        {
                            //Remove
                            getUniqueRender.RemoveValue(fullValue);

                            //Reset count
                            idCount = getUniqueRender.ValueCount;
                            idxValueLine = idxValueLine - 1;
                        }
                    }
                }

                //Iterate through all values
                foreach (KeyValuePair<string, List<string>> geolineDefinition in uniqueGeolineList)
                {

                    foreach (string symbols in geolineDefinition.Value)
                    {
                        //keep current value for symbol and description, needs to be update if empty or missing
                        string geolineSymbol = symbols;
                        string geolineDescription = string.Empty;

                        //Create the new unique value
                        string newUniqueValue = geolineSymbol + GSC_ProjectEditor.Constants.Symbol4Layers.fieldDelimeter + geolineDefinition.Key;

                        //Create label for new symbol
                        Tuple<string, string> matchLegend;
                        try
                        {
                            matchLegend = GSC_ProjectEditor.Tables.GetUniqueDoubleFieldValues(tLegendGen, new Tuple<string, string>(tLegendGenSym, tLegendGenName), tLegendGenID + " = '" + geolineDefinition.Key + "'")[0];
                        }
                        catch (Exception)
                        {
                            //If value isn't found inside legend table, get information from symbol table
                            matchLegend = GSC_ProjectEditor.Tables.GetUniqueDoubleFieldValues(mGeolineSymbol, new Tuple<string, string>(mGeolineFGDC, mGeolineLegendDesc), mGeolineID + " = '" + geolineDefinition.Key + "'")[0];
                            matchLegend = new Tuple<string, string>(geolineSymbol, matchLegend.Item2);
                        }

                        //Validate empty symbol code
                        if (geolineSymbol == string.Empty)
                        {
                            newUniqueValue = matchLegend.Item1 + GSC_ProjectEditor.Constants.Symbol4Layers.fieldDelimeter + geolineDefinition.Key;
                            geolineSymbol = GSC_ProjectEditor.Constants.Styles.InvalidLine_FGDC; //Add invalid symbol style
                            emptySymbolTrigger = true;
                        }

                        //Validate missing description, add geopointID if missing.
                        if (matchLegend.Item2 == GSC_ProjectEditor.Properties.Resources.Error_NotInScientificLanguageDictionary)
                        {
                            geolineDescription = matchLegend.Item2 + ", " + geolineDefinition.Key;

                        }
                        else
                        {
                            geolineDescription = matchLegend.Item2;
                        }



                        //Get match symbol
                        ILineSymbol symy = null; //Create a global line symbol object (it can takes whatever line symbol object type)
                        symy = Utilities.MapDocumentSymbol.GetMatchingSymbol(geolineSymbol, stylePath);

                        //Set symbol to the match
                        if (symy != null)
                        {
                            if (!Properties.Settings.Default.KeepCustomSymbols)
                            {
                                getUniqueRender.AddValue(newUniqueValue, GSC_ProjectEditor.Constants.Symbol4Layers.geolineLabelFieldName, symy as ISymbol);
                                getUniqueRender.Label[newUniqueValue] = geolineDescription; //Apply legend description as label
                                //getUniqueRender.Symbol[newUniqueValue] = symy as ISymbol;
                            }
                            else
                            {
                                if (!currentGeolineValues.Contains(newUniqueValue))
                                {
                                    //Create a new empty line symbol
                                    ISimpleLineSymbol symx = null;
                                    symx = GSC_ProjectEditor.Symbols.GetDefaultLineSymbol();
                                    getUniqueRender.AddValue(newUniqueValue, GSC_ProjectEditor.Constants.Symbol4Layers.geolineLabelFieldName, symx as ISymbol);
                                    getUniqueRender.Label[newUniqueValue] = geolineDescription; //Apply legend description as label
                                }

                            }

                        }
                        else
                        {
                            //If symbol is not the right one, just do like an empty symbol and show invalid symbol
                            geolineSymbol = GSC_ProjectEditor.Constants.Styles.InvalidLine_FGDC;
                            symy = Utilities.MapDocumentSymbol.GetMatchingSymbol(geolineSymbol, stylePath);

                            if (!Properties.Settings.Default.KeepCustomSymbols && symy != null)
                            {
                                getUniqueRender.Symbol[newUniqueValue] = symy as ISymbol;
                            }

                            MessageBox.Show(Properties.Resources.Error_SymbolMistmatchWithStyle + " " + newUniqueValue, Properties.Resources.Error_GSC_MissingSymbol, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }

                }

                if (emptySymbolTrigger)
                {
                    MessageBox.Show(GSC_ProjectEditor.Properties.Resources.Error_SymbolMistmatch2, GSC_ProjectEditor.Properties.Resources.Error_SymbolMistmatchMessageboxTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            //Refresh TOC
            ArcMap.Document.ActivatedView.ContentsChanged();
            ArcMap.Document.UpdateContents();
            ArcMap.Document.ActivatedView.Refresh();


        }

        ///<summary>
        ///This method will help detect if current layer is symbolized correctly.
        ///Will add a custom renderer if nothing exists
        ///</summary>
        /// <param name="layerToAddRenderer">the layer object to verify renderer from</param>
        public bool validateLineRenderer(IGeoFeatureLayer layerToAddRenderer, string lineStylePath)
        {
            try
            {
                //Get the unique renderer style from 
                IUniqueValueRenderer getUniqueRender = layerToAddRenderer.Renderer as UniqueValueRenderer;

                //Get proper interface unique id
                string currentUID = layerToAddRenderer.RendererPropertyPageClassID.Value.ToString();
                string wantedUID = GSC_ProjectEditor.Constants.GUIDs.UIDSymbolUniqueValuesMultipleField;

                //Count entities from current renderer
                int count = 0;
                int fieldCount = 0;
                bool noRenderer = false; //A joker bool value, this will be used in case arc map says currentUID and wantedUID are the same, when they are not...
                try
                {
                    count = getUniqueRender.ValueCount;
                    fieldCount = getUniqueRender.FieldCount;
                }
                catch
                {
                    noRenderer = true;
                }

                //If the renderer is not set on the right interface redo from scratch, or Arc tells that the UID is the right one but the renderer doesn't exists.
                if (wantedUID != currentUID || noRenderer == true)
                {

                    //Force interface to unique values multiple fields renderer
                    UID pUID = new UIDClass();
                    pUID.Value = wantedUID;
                    layerToAddRenderer.RendererPropertyPageClassID = pUID as UIDClass;

                    //Create a renderer for style
                    getUniqueRender = new UniqueValueRenderer();

                    //Get symbol field indexes
                    int symIndex = layerToAddRenderer.FeatureClass.FindField(geolineSym);
                    int geolineIDIndex = layerToAddRenderer.FeatureClass.FindField(geolineID);

                    //Set some renderer properties
                    SetLineRendererProperties(getUniqueRender, lineStylePath);
                }
                else
                {

                    //Detect if the renderer is set on the right field
                    if (getUniqueRender.FieldCount != 2)
                    {
                        SetLineRendererProperties(getUniqueRender, lineStylePath);
                    }

                }

                //Persist change
                layerToAddRenderer.Renderer = getUniqueRender as IFeatureRenderer;

                return true;
            }
            catch (Exception validateLineRendererError)
            {
                MessageBox.Show("validateLineRendererError: " + validateLineRendererError.Message);

                return false;
            }



        }

        /// <summary>
        /// Will update symbole geoline table "selectCode" field to 1, for each given geolineIDs in list.
        /// </summary>
        /// <param name="geolineIDList">List of ids to turn on in symbol geoline table</param>
        public void UpdateGeolineSymbolInLegend(List<string> geolineIDList)
        {
            //Variable
            List<Dictionary<string, object>> newRowsToAdd = new List<Dictionary<string, object>>();

            //Get a list of unique ids stored in legend table
            //string geolineQuery = tLegendGenType + " = '" + lineSymbolType + "'";
            List<string> legendIds = GSC_ProjectEditor.Tables.GetUniqueFieldValues(tLegendGen, tLegendGenID, null, false, null)[dicoMain];

            //Build a new row dictionnary if ids are not found in legend table
            foreach (string geo in geolineIDList)
            {
                if (!legendIds.Contains(geo))
                {
                    //Get an initialization symbol code from the feature itself since it was found inside it, and desription in symbol table
                    string currentFGDC = "";
                    string currentDescription = "";

                    try
                    {
                        currentDescription = GSC_ProjectEditor.Tables.GetFieldValues(mGeolineSymbol, mGeolineLegendDesc, mGeolineID + " = '" + geo + "'")[0];
                    }
                    catch (Exception)
                    {

                        currentDescription = GSC_ProjectEditor.Properties.Resources.Error_NotInScientificLanguageDictionary;

                    }

                    try
                    {
                        //It could crash if value is null
                        List<string> currentFGDCList = GSC_ProjectEditor.Tables.GetFieldValues(geoline, geolineSym, geolineID + " = '" + geo + "'");
                        List<string> currentUniqueFGDCList = currentFGDCList.Distinct().ToList();
                        currentFGDC = currentFGDCList[0];

                        //Check whether multiple code were found inside the feature
                        if (currentUniqueFGDCList.Count > 1)
                        {
                            //Build full string of unique value to show user
                            string uniqueListString = "";
                            List<string> uniqueList = new List<string>();
                            foreach (string fgdc in currentFGDCList)
                            {
                                if (!uniqueList.Contains(fgdc))
                                {
                                    uniqueList.Add(fgdc);
                                    uniqueListString = uniqueListString + fgdc + "; ";
                                }
                            }

                            //Show warning to user
                            string warningMessage = Properties.Resources.Warning_MultipleFGDCFound + " " + currentDescription + ": " + uniqueListString + ".";
                            MessageBox.Show(warningMessage, Properties.Resources.Warning_MultiFGDCFoundTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        }
                    }
                    catch (Exception)
                    {
                        //Get value from symbol table instead
                        currentFGDC = GSC_ProjectEditor.Tables.GetFieldValues(mGeolineSymbol, mGeolineFGDC, mGeolineID + " = '" + geo + "'")[0];
                    }



                    //Init sym type
                    string symType = lineSymbolType;

                    //Add to list
                    Dictionary<string, object> rowDico = new Dictionary<string, object>();
                    if (currentFGDC != null)
                    {
                        rowDico[tLegendGenID] = geo;
                        rowDico[tLegendGenName] = currentDescription;
                        rowDico[tLegendGenSym] = currentFGDC;
                        rowDico[tLegendGenType] = symType;

                        newRowsToAdd.Add(rowDico);
                    }
                }
            }

            //Add new rows in legend table if needed
            GSC_ProjectEditor.Tables.AddRowsWithValues(tLegendGen, newRowsToAdd);

        }

        /// <summary>
        /// Will refresh the symbols from the layer and the templates
        /// </summary>
        public void RefreshGeolineSymbols()
        {

            //Validate geolineIDs to make sure everything is correctly synchronised
            GSC_ProjectEditor.IDs.CalculateGeolineID();

            //Validate if all feature geoline ids are in legend table
            List<string> geolineIDList = GSC_ProjectEditor.Tables.GetUniqueFieldValues(geoline, geolineID, null, false, null)[dicoMain];
            UpdateGeolineSymbolInLegend(geolineIDList);

            //Recreate templates; it'll also trigger layer refresh from within
            CreateLineTemplate(ArcMap.Application.Document as IMxDocument);
        }

        /// <summary>
        /// Will apply geoline specific properties to input renderer
        /// </summary>
        /// <param name="inRenderer">The renderer to modify</param>
        /// <param name="defaultSymbol">The default symbol for default values</param>
        public void SetLineRendererProperties(IUniqueValueRenderer inRenderer, string lineStylePath)
        {
            IUniqueValueRenderer geolineRenderer = GSC_ProjectEditor.Symbols.SetGeolineRendererProperties(inRenderer, lineStylePath);
            if (geolineRenderer != null)
            {
                geolineRenderer.FieldCount = 2;//Only one field will determine symbol
                geolineRenderer.set_Field(0, geolineSym); //Set the field to sym field 
                geolineRenderer.set_Field(1, geolineID); //Set the second field to be geolineID 
            }
        }

        #endregion

    }
}
