using ArcGIS.Core.CIM;
using ArcGIS.Core.Internal.CIM;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Internal.Mapping;
using ArcGIS.Desktop.Mapping;
using BedrockEditorPro.Models;
using BedrockEditorPro.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xaml;

namespace BedrockEditorPro.Utilities
{
    public class Symbols
    {
        /// <summary>
        /// Creates a point symbol renderer
        /// </summary>
        /// <param name="pointRGB">A list containing red green blue numerical codes for point color</param>
        /// <param name="pointSize">A point size</param>
        /// <returns></returns>
        public static CIMPointSymbol GetLabelDefaultRenderer(CIMColor inColor)
        {
            CIMPointSymbol pntSym = SymbolFactory.Instance.ConstructPointSymbol(ColorFactory.Instance.RedRGB, 4, SimpleMarkerStyle.Circle);

            //Create color object to apply to symbol
            CIMColor pntColor = CIMColor.CreateRGBColor(inColor.GetColorComponent(0), inColor.GetColorComponent(1), inColor.GetColorComponent(2));

            ///Note: We used to remove the outline of the circle in version 3.X
            ///it's actually easier to validate when keeping it but still coloring it
            ///just like the map unit

            //Add color and width
            pntSym.SetColor(pntColor);

            return pntSym;
        }

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

        /// <summary>
        /// Will validate the existance of the default style file from
        /// the embedded resource and will return it's path
        /// </summary>
        /// <returns></returns>
        public static string ManageStyleFile()
        {
            WorkingEnvironment workingEnvironment = new WorkingEnvironment();
            string StyleFilePath = System.IO.Path.Combine(workingEnvironment.WorkingEnvironmentPath, nameof(Properties.Resources.GSC_SymbolStandard) + ".stylx");

            if (!File.Exists(StyleFilePath))
            {
                try
                {
                    FileService.WriteStreamResource(Properties.Resources.GSC_SymbolStandard, StyleFilePath);
                }
                catch (Exception e)
                {
                    new ErrorService(e).WriteToFile();
                }
                
            }
            
            return StyleFilePath;

        }

        /// <summary>
        /// Will return a default arcGIS color ramp for layer symbolization fall back
        /// </summary>
        /// <returns></returns>
        public static CIMColorRamp GetDefaultColorRamp(Project inProject)
        {
            StyleProjectItem style = inProject.GetItems<StyleProjectItem>().FirstOrDefault(x => x.Name == "ArcGIS Colors");
            if (style != null)
            {
                List<ColorRampStyleItem> colorRampSI = style.SearchColorRamps("Viridis").ToList();

                if (colorRampSI != null && colorRampSI.Count() != 0)
                {
                    return colorRampSI[0].ColorRamp;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Will return the style object and even add it to the current project if it's not already there.
        /// </summary>
        /// <param name="stylePath"></param>
        /// <returns></returns>
        public static StyleProjectItem GetStyleItemProject(string stylePath)
        {
            List<StyleProjectItem> styleItems = Project.Current.GetItems<StyleProjectItem>().Where(x => x.Path == stylePath).ToList();
            if (styleItems == null || styleItems.Count() == 0)
            {
                Project.Current.AddStyle(stylePath);
            }
            StyleProjectItem workingStyle = Project.Current.GetItems<StyleProjectItem>().FirstOrDefault(x => x.Path == stylePath);

            return workingStyle;
        }

        /// <summary>
        /// Creates templates of geoline
        /// </summary>
        /// <param name="m_doc"></param>
        public static void CreateLineTemplate(FeatureLayer featureLayer, GeoLines geoLines, bool forceUpdate = false)
        {
            try
            {
                //get the CIM layer definition
                CIMFeatureLayer layerDef = featureLayer.GetDefinition() as CIMFeatureLayer;

                //set new template values
                CIMRowTemplate geolineTemplateDef = new CIMRowTemplate();
                geolineTemplateDef.Name = string.Format("{0},{1}", geoLines.GSCSymbol, geoLines.Name);
                geolineTemplateDef.Description = geoLines.GeolineID;

                // set some default attributes
                geolineTemplateDef.DefaultValues = new Dictionary<string, object>();
                geolineTemplateDef.DefaultValues.Add(geoLines.GetPropertyAttributeName(nameof(geoLines.GeolineID)),geoLines.GeolineID);

                //Manage subtype
                geolineTemplateDef.DefaultValues.Add(geoLines.GetPropertyAttributeName(nameof(geoLines.GeolineType)), geoLines.GeolineType);

                //Manage Qualifier
                geolineTemplateDef.DefaultValues.Add(geoLines.GetPropertyAttributeName(nameof(geoLines.Qualifier)), geoLines.Qualifier);

                //Manage Confidence
                geolineTemplateDef.DefaultValues.Add(geoLines.GetPropertyAttributeName(nameof(geoLines.Confidence)), geoLines.Confidence);

                //Manage Attitude
                geolineTemplateDef.DefaultValues.Add(geoLines.GetPropertyAttributeName(nameof(geoLines.Attitude)), geoLines.Attitude);

                //Manage Generation
                geolineTemplateDef.DefaultValues.Add(geoLines.GetPropertyAttributeName(nameof(geoLines.Generation)), geoLines.Generation);

                //Manage FGDC Symbol
                geolineTemplateDef.DefaultValues.Add(geoLines.GetPropertyAttributeName(nameof(geoLines.GSCSymbol)), geoLines.GSCSymbol);

                //Manage CreatorID
                geolineTemplateDef.DefaultValues.Add(geoLines.GetPropertyAttributeName(nameof(geoLines.CreatorID)), geoLines.CreatorID);

                //get all templates on this layer
                // NOTE - layerDef.FeatureTemplates could be null 
                //    if Create Features window hasn't been opened
                var layerTemplates = layerDef.FeatureTemplates?.ToList();
                if (layerTemplates == null)
                    layerTemplates = new List<CIMEditingTemplate>();

                //check if the template already exists and remplace it if so
                if (forceUpdate)
                {
                    CIMEditingTemplate templateToUpdate = null;
                    foreach (CIMEditingTemplate templates in layerTemplates)
                    {
                        if (templates.Name.Contains(geoLines.GeolineID))
                        {
                            templateToUpdate = templates;
                            break;
                        }
                    }
                    if (templateToUpdate != null)
                    {
                        layerTemplates.Remove(templateToUpdate);
                    }
                }

                //add the new template to the layer template list
                layerTemplates.Add(geolineTemplateDef);

                //update the layerdefinition with the templates
                layerDef.FeatureTemplates = layerTemplates.ToArray();

                // check the AutoGenerateFeatureTemplates flag, 
                //     set to false so our changes will stick
                if (layerDef.AutoGenerateFeatureTemplates)
                    layerDef.AutoGenerateFeatureTemplates = false;

                //and commit
                featureLayer.SetDefinition(layerDef);

            }
            catch (Exception CreateTemplateError)
            {
                new ErrorService(CreateTemplateError).WriteToFile();
            }

        }

        /// <summary>
        /// Creates templates of geoline
        /// </summary>
        /// <param name="m_doc"></param>
        public static void CreatePointTemplate(FeatureLayer featureLayer, GeoPoints geoPoints, bool forceUpdate = false)
        {
            try
            {
                //get the CIM layer definition
                CIMFeatureLayer layerDef = featureLayer.GetDefinition() as CIMFeatureLayer;

                //set new template values
                CIMRowTemplate geopointTemplateDef = new CIMRowTemplate();
                geopointTemplateDef.Name = string.Format("{0},{1}", geoPoints.GSCSymbol, geoPoints.Name);
                geopointTemplateDef.Description = geoPoints.GeopointID;

                // set some default attributes
                geopointTemplateDef.DefaultValues = new Dictionary<string, object>();
                geopointTemplateDef.DefaultValues.Add(geoPoints.GetPropertyAttributeName(nameof(geoPoints.GeopointID)), geoPoints.GeopointID);

                //Manage type
                geopointTemplateDef.DefaultValues.Add(geoPoints.GetPropertyAttributeName(nameof(geoPoints.GeopointType)), geoPoints.GeopointType);

                //Manage subset
                geopointTemplateDef.DefaultValues.Add(geoPoints.GetPropertyAttributeName(nameof(geoPoints.Subset)), geoPoints.Subset);

                //Manage attitude
                geopointTemplateDef.DefaultValues.Add(geoPoints.GetPropertyAttributeName(nameof(geoPoints.Attitude)), geoPoints.Attitude);

                //Manage generation
                geopointTemplateDef.DefaultValues.Add(geoPoints.GetPropertyAttributeName(nameof(geoPoints.Generation)), geoPoints.Generation);

                //Manage youging
                geopointTemplateDef.DefaultValues.Add(geoPoints.GetPropertyAttributeName(nameof(geoPoints.Younging)), geoPoints.Younging);

                //Manage method
                geopointTemplateDef.DefaultValues.Add(geoPoints.GetPropertyAttributeName(nameof(geoPoints.Method)), geoPoints.Method);

                //Manage FGDC Symbol
                geopointTemplateDef.DefaultValues.Add(geoPoints.GetPropertyAttributeName(nameof(geoPoints.GSCSymbol)), geoPoints.GSCSymbol);

                //Manage CreatorID
                geopointTemplateDef.DefaultValues.Add(geoPoints.GetPropertyAttributeName(nameof(geoPoints.CreatorID)), geoPoints.CreatorID);

                //get all templates on this layer
                // NOTE - layerDef.FeatureTemplates could be null 
                //    if Create Features window hasn't been opened
                var layerTemplates = layerDef.FeatureTemplates?.ToList();
                if (layerTemplates == null)
                    layerTemplates = new List<CIMEditingTemplate>();

                //check if the template already exists and remplace it if so
                if (forceUpdate)
                {
                    CIMEditingTemplate templateToUpdate = null;
                    foreach (CIMEditingTemplate templates in layerTemplates)
                    {
                        if (templates.Name.Contains(geoPoints.GeopointID))
                        {
                            templateToUpdate = templates;
                            break;
                        }
                    }
                    if (templateToUpdate != null)
                    {
                        layerTemplates.Remove(templateToUpdate);
                    }
                }

                //add the new template to the layer template list
                layerTemplates.Add(geopointTemplateDef);

                //update the layerdefinition with the templates
                layerDef.FeatureTemplates = layerTemplates.ToArray();

                // check the AutoGenerateFeatureTemplates flag, 
                //     set to false so our changes will stick
                if (layerDef.AutoGenerateFeatureTemplates)
                    layerDef.AutoGenerateFeatureTemplates = false;

                //and commit
                featureLayer.SetDefinition(layerDef);

            }
            catch (Exception CreateTemplateError)
            {
                new ErrorService(CreateTemplateError).WriteToFile();
            }

        }

        /// <summary>
        /// Creates templates of geoline
        /// </summary>
        /// <param name="m_doc"></param>
        public static void CreateLabelTemplate(FeatureLayer featureLayer, Labels labels, bool forceUpdate = false)
        {
            try
            {
                //get the CIM layer definition
                CIMFeatureLayer layerDef = featureLayer.GetDefinition() as CIMFeatureLayer;

                //set new template values
                CIMRowTemplate labelTemplateDef = new CIMRowTemplate();
                labelTemplateDef.Name = labels.Name;
                labelTemplateDef.Description = labels.LabelID;

                // set some default attributes
                labelTemplateDef.DefaultValues = new Dictionary<string, object>();
                labelTemplateDef.DefaultValues.Add(labels.GetPropertyAttributeName(nameof(labels.LabelID)), labels.LabelID);

                //Manage FGDC Symbol
                labelTemplateDef.DefaultValues.Add(labels.GetPropertyAttributeName(nameof(labels.GSCSymbol)), labels.GSCSymbol);

                //Manage CreatorID
                labelTemplateDef.DefaultValues.Add(labels.GetPropertyAttributeName(nameof(labels.CreatorID)), labels.CreatorID);

                //get all templates on this layer
                // NOTE - layerDef.FeatureTemplates could be null 
                //    if Create Features window hasn't been opened
                var layerTemplates = layerDef.FeatureTemplates?.ToList();
                if (layerTemplates == null)
                    layerTemplates = new List<CIMEditingTemplate>();

                //check if the template already exists and remplace it if so
                if (forceUpdate)
                {
                    CIMEditingTemplate templateToUpdate = null;
                    foreach (CIMEditingTemplate templates in layerTemplates)
                    {
                        if (templates.Name.Contains(labels.LabelID))
                        {
                            templateToUpdate = templates;
                            break;
                        }
                    }
                    if (templateToUpdate != null)
                    {
                        layerTemplates.Remove(templateToUpdate);
                    }
                }

                //add the new template to the layer template list
                layerTemplates.Add(labelTemplateDef);

                //update the layerdefinition with the templates
                layerDef.FeatureTemplates = layerTemplates.ToArray();

                // check the AutoGenerateFeatureTemplates flag, 
                //     set to false so our changes will stick
                if (layerDef.AutoGenerateFeatureTemplates)
                    layerDef.AutoGenerateFeatureTemplates = false;

                //and commit
                featureLayer.SetDefinition(layerDef);

            }
            catch (Exception CreateTemplateError)
            {
                new ErrorService(CreateTemplateError).WriteToFile();
            }

        }


    }
}
