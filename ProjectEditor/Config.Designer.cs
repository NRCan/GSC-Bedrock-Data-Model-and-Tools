//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GSC_ProjectEditor {
    using ESRI.ArcGIS.Framework;
    using ESRI.ArcGIS.ArcMapUI;
    using ESRI.ArcGIS.Editor;
    using ESRI.ArcGIS.esriSystem;
    using System;
    using System.Collections.Generic;
    using ESRI.ArcGIS.Desktop.AddIns;
    
    
    /// <summary>
    /// A class for looking up declarative information in the associated configuration xml file (.esriaddinx).
    /// </summary>
    internal static class ThisAddIn {
        
        internal static string Name {
            get {
                return "GSC Bedrock Project";
            }
        }
        
        internal static string AddInID {
            get {
                return "{02a0ca2e-8588-45e3-8f3a-3e011b10f973}";
            }
        }
        
        internal static string Company {
            get {
                return "NRCan-RNCan";
            }
        }
        
        internal static string Version {
            get {
                return "3.3";
            }
        }
        
        internal static string Description {
            get {
                return "Project Data Management Tools";
            }
        }
        
        internal static string Author {
            get {
                return "Geological Survey Canada";
            }
        }
        
        internal static string Date {
            get {
                return "2021-05-05";
            }
        }
        
        internal static ESRI.ArcGIS.esriSystem.UID ToUID(this System.String id) {
            ESRI.ArcGIS.esriSystem.UID uid = new ESRI.ArcGIS.esriSystem.UIDClass();
            uid.Value = id;
            return uid;
        }
        
        /// <summary>
        /// A class for looking up Add-in id strings declared in the associated configuration xml file (.esriaddinx).
        /// </summary>
        internal class IDs {
            
            /// <summary>
            /// Returns 'NRCan-RNCan_Addin_ArcMap_buttonAddSymbolLine', the id declared for Add-in Button class 'Button_CreateEdit_GeolineTemplate'
            /// </summary>
            internal static string Button_CreateEdit_GeolineTemplate {
                get {
                    return "NRCan-RNCan_Addin_ArcMap_buttonAddSymbolLine";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_Addin_ArcMap_ButtonAddSymbolPoint', the id declared for Add-in Button class 'Button_CreateEdit_GeopointTemplate'
            /// </summary>
            internal static string Button_CreateEdit_GeopointTemplate {
                get {
                    return "NRCan-RNCan_Addin_ArcMap_ButtonAddSymbolPoint";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_Addin_ArcMap_buttonAddLabel', the id declared for Add-in Button class 'Button_CreateEdit_LabelTemplate'
            /// </summary>
            internal static string Button_CreateEdit_LabelTemplate {
                get {
                    return "NRCan-RNCan_Addin_ArcMap_buttonAddLabel";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_Addin_ArcMap_buttonCreatePolyMapUnit', the id declared for Add-in Button class 'Button_CreateEdit_CreateMapUnits'
            /// </summary>
            internal static string Button_CreateEdit_CreateMapUnits {
                get {
                    return "NRCan-RNCan_Addin_ArcMap_buttonCreatePolyMapUnit";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_Addin_ArcMap_ButtonQualityControlGeoline', the id declared for Add-in Button class 'Button_CreateEdit_ValidateGeolineIntegrity'
            /// </summary>
            internal static string Button_CreateEdit_ValidateGeolineIntegrity {
                get {
                    return "NRCan-RNCan_Addin_ArcMap_ButtonQualityControlGeoline";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_Addin_ArcMap_buttonQualityControl', the id declared for Add-in Button class 'Button_CreateEdit_QualityControl'
            /// </summary>
            internal static string Button_CreateEdit_QualityControl {
                get {
                    return "NRCan-RNCan_Addin_ArcMap_buttonQualityControl";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_GSC_ProjectEditor_ButtonAddGeoEvent', the id declared for Add-in Button class 'Button_CreateEdit_GeologicalEvents'
            /// </summary>
            internal static string Button_CreateEdit_GeologicalEvents {
                get {
                    return "NRCan-RNCan_GSC_ProjectEditor_ButtonAddGeoEvent";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_Addin_ArcMap_ButtonCreateThematicLayers', the id declared for Add-in Button class 'Button_View_CreateThematicLayers'
            /// </summary>
            internal static string Button_View_CreateThematicLayers {
                get {
                    return "NRCan-RNCan_Addin_ArcMap_ButtonCreateThematicLayers";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_GSC_ProjectEditor_ButtonKeepCustomSymbols', the id declared for Add-in Button class 'Button_View_KeepCustomStyle'
            /// </summary>
            internal static string Button_View_KeepCustomStyle {
                get {
                    return "NRCan-RNCan_GSC_ProjectEditor_ButtonKeepCustomSymbols";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_Addin_ArcMap_ButtonCreateOverprintThematicLayer', the id declared for Add-in Button class 'Button_View_CreateMapUnitOverprintLayer'
            /// </summary>
            internal static string Button_View_CreateMapUnitOverprintLayer {
                get {
                    return "NRCan-RNCan_Addin_ArcMap_ButtonCreateOverprintThematicLayer";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_Addin_ArcMap_buttonLegendItems', the id declared for Add-in Button class 'Button_Legend_ItemsModification'
            /// </summary>
            internal static string Button_Legend_ItemsModification {
                get {
                    return "NRCan-RNCan_Addin_ArcMap_buttonLegendItems";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_Addin_ArcMap_ButtonManageCGMLegendItems', the id declared for Add-in Button class 'Button_Legend_IntersectItemsWithStudyArea'
            /// </summary>
            internal static string Button_Legend_IntersectItemsWithStudyArea {
                get {
                    return "NRCan-RNCan_Addin_ArcMap_ButtonManageCGMLegendItems";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_Addin_ArcMap_ButtonCGMDescription', the id declared for Add-in Button class 'Button_Legend_ItemDescription'
            /// </summary>
            internal static string Button_Legend_ItemDescription {
                get {
                    return "NRCan-RNCan_Addin_ArcMap_ButtonCGMDescription";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_Addin_ArcMap_ButtonCGM', the id declared for Add-in Button class 'Button_Legend_SurroundInformation'
            /// </summary>
            internal static string Button_Legend_SurroundInformation {
                get {
                    return "NRCan-RNCan_Addin_ArcMap_ButtonCGM";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_GSC_ProjectEditor_ButtonCGMLegendOrder', the id declared for Add-in Button class 'Button_Legend_ItemsOrder'
            /// </summary>
            internal static string Button_Legend_ItemsOrder {
                get {
                    return "NRCan-RNCan_GSC_ProjectEditor_ButtonCGMLegendOrder";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_Addin_ArcMap_ButtonCreateTempLegendGenerator', the id declared for Add-in Button class 'Button_Legend_CreateTempTableLegendGenerator'
            /// </summary>
            internal static string Button_Legend_CreateTempTableLegendGenerator {
                get {
                    return "NRCan-RNCan_Addin_ArcMap_ButtonCreateTempLegendGenerator";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_Addin_ArcMap_ButtonCreatePublishingDatabase', the id declared for Add-in Button class 'Button_Legend_CreateDataBundle'
            /// </summary>
            internal static string Button_Legend_CreateDataBundle {
                get {
                    return "NRCan-RNCan_Addin_ArcMap_ButtonCreatePublishingDatabase";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_GSC_ProjectEditor_ArcMap_ButtonLegendExport', the id declared for Add-in Button class 'Button_Legend_Export'
            /// </summary>
            internal static string Button_Legend_Export {
                get {
                    return "NRCan-RNCan_GSC_ProjectEditor_ArcMap_ButtonLegendExport";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_GSC_ProjectEditor_Button_Legend_Import', the id declared for Add-in Button class 'Button_Legend_Import'
            /// </summary>
            internal static string Button_Legend_Import {
                get {
                    return "NRCan-RNCan_GSC_ProjectEditor_Button_Legend_Import";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_Addin_ArcMap_combobox_SelectDataSource', the id declared for Add-in ComboBox class 'Combobox_SelectDataSource'
            /// </summary>
            internal static string Combobox_SelectDataSource {
                get {
                    return "NRCan-RNCan_Addin_ArcMap_combobox_SelectDataSource";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_Addin_ArcMap_Combobox_SelectParticipant', the id declared for Add-in ComboBox class 'Combobox_SelectParticipant'
            /// </summary>
            internal static string Combobox_SelectParticipant {
                get {
                    return "NRCan-RNCan_Addin_ArcMap_Combobox_SelectParticipant";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_Addin_ArcMap_ButtonRefreshSymbols', the id declared for Add-in Button class 'Button_RefreshSymbols'
            /// </summary>
            internal static string Button_RefreshSymbols {
                get {
                    return "NRCan-RNCan_Addin_ArcMap_ButtonRefreshSymbols";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_Addin_ArcCatalog_ButtonNewGeodatabase', the id declared for Add-in Button class 'Button_Environment_NewGeodatabase'
            /// </summary>
            internal static string Button_Environment_NewGeodatabase {
                get {
                    return "NRCan-RNCan_Addin_ArcCatalog_ButtonNewGeodatabase";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_Addin_ArcMap_ButtonCreateTopology', the id declared for Add-in Button class 'Button_Environment_CreateApplyTopologicalRules'
            /// </summary>
            internal static string Button_Environment_CreateApplyTopologicalRules {
                get {
                    return "NRCan-RNCan_Addin_ArcMap_ButtonCreateTopology";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_Addin_ArcCatalog_ButtonCreateMXD', the id declared for Add-in Button class 'Button_Environment_AddProjectLayers'
            /// </summary>
            internal static string Button_Environment_AddProjectLayers {
                get {
                    return "NRCan-RNCan_Addin_ArcCatalog_ButtonCreateMXD";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_Addin_ArcCatalog_ButtonUpgradeGDBVersion', the id declared for Add-in Button class 'Button_Environment_UpgradeGSCBedrockGeodatabase'
            /// </summary>
            internal static string Button_Environment_UpgradeGSCBedrockGeodatabase {
                get {
                    return "NRCan-RNCan_Addin_ArcCatalog_ButtonUpgradeGDBVersion";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_Addin_ArcCatalog_ButtonProject', the id declared for Add-in Button class 'Button_ProjectMetadata_Definition'
            /// </summary>
            internal static string Button_ProjectMetadata_Definition {
                get {
                    return "NRCan-RNCan_Addin_ArcCatalog_ButtonProject";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_Addin_ArcCatalog_ButtonActivity', the id declared for Add-in Button class 'Button_ProjectMetadata_Activities'
            /// </summary>
            internal static string Button_ProjectMetadata_Activities {
                get {
                    return "NRCan-RNCan_Addin_ArcCatalog_ButtonActivity";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_Addin_ArcCatalog_ButtonOrganisation', the id declared for Add-in Button class 'Button_ProjectMetadata_Organization'
            /// </summary>
            internal static string Button_ProjectMetadata_Organization {
                get {
                    return "NRCan-RNCan_Addin_ArcCatalog_ButtonOrganisation";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_Addin_ArcCatalog_ButtonPerson', the id declared for Add-in Button class 'Button_ProjectMetadata_Participants'
            /// </summary>
            internal static string Button_ProjectMetadata_Participants {
                get {
                    return "NRCan-RNCan_Addin_ArcCatalog_ButtonPerson";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_Addin_ArcCatalog_ButtonParticipant', the id declared for Add-in Button class 'Button_ProjectMetadata_Roles'
            /// </summary>
            internal static string Button_ProjectMetadata_Roles {
                get {
                    return "NRCan-RNCan_Addin_ArcCatalog_ButtonParticipant";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_Addin_ArcCatalog_ButtonAppendProjectData', the id declared for Add-in Button class 'Button_Load_LinesPoints'
            /// </summary>
            internal static string Button_Load_LinesPoints {
                get {
                    return "NRCan-RNCan_Addin_ArcCatalog_ButtonAppendProjectData";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_Addin_ArcCatalog_ButtonAddGanfeldData', the id declared for Add-in Button class 'Button_Load_FieldDataGanfeld'
            /// </summary>
            internal static string Button_Load_FieldDataGanfeld {
                get {
                    return "NRCan-RNCan_Addin_ArcCatalog_ButtonAddGanfeldData";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_Addin_ArcCatalog_ButtonAddSource', the id declared for Add-in Button class 'Button_Load_SourceInformation'
            /// </summary>
            internal static string Button_Load_SourceInformation {
                get {
                    return "NRCan-RNCan_Addin_ArcCatalog_ButtonAddSource";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_Addin_ArcCatalog_ButtonStudyArea', the id declared for Add-in Button class 'Button_Load_StudyAreas'
            /// </summary>
            internal static string Button_Load_StudyAreas {
                get {
                    return "NRCan-RNCan_Addin_ArcCatalog_ButtonStudyArea";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_GSC_ProjectEditor_ButtonAddCartoPoint', the id declared for Add-in Button class 'Button_Load_CartographicPoints'
            /// </summary>
            internal static string Button_Load_CartographicPoints {
                get {
                    return "NRCan-RNCan_GSC_ProjectEditor_ButtonAddCartoPoint";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_GSC_ProjectEditor_ButtonTranslateFStrucToGeoPoint', the id declared for Add-in Button class 'Button_Load_TranslateGanfeldPointStructure'
            /// </summary>
            internal static string Button_Load_TranslateGanfeldPointStructure {
                get {
                    return "NRCan-RNCan_GSC_ProjectEditor_ButtonTranslateFStrucToGeoPoint";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_Addin_ArcMap_dockablewindowAddGeoline', the id declared for Add-in DockableWindow class 'Dockablewindow_CreateEdit_GeolineTemplate+AddinImpl'
            /// </summary>
            internal static string Dockablewindow_CreateEdit_GeolineTemplate {
                get {
                    return "NRCan-RNCan_Addin_ArcMap_dockablewindowAddGeoline";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_Addin_ArcMap_dockablewindowQualityControl', the id declared for Add-in DockableWindow class 'Dockablewindow_CreateEdit_QualityControl+AddinImpl'
            /// </summary>
            internal static string Dockablewindow_CreateEdit_QualityControl {
                get {
                    return "NRCan-RNCan_Addin_ArcMap_dockablewindowQualityControl";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_Addin_ArcMap_dockablewindowAddLabel', the id declared for Add-in DockableWindow class 'Dockablewindow_CreateEdit_LabelTemplate+AddinImpl'
            /// </summary>
            internal static string Dockablewindow_CreateEdit_LabelTemplate {
                get {
                    return "NRCan-RNCan_Addin_ArcMap_dockablewindowAddLabel";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_Addin_ArcMap_DockableWindowCGM', the id declared for Add-in DockableWindow class 'DockableWindow_Legend_SurroundInformation+AddinImpl'
            /// </summary>
            internal static string DockableWindow_Legend_SurroundInformation {
                get {
                    return "NRCan-RNCan_Addin_ArcMap_DockableWindowCGM";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_Addin_ArcMap_DockableWindowCGMDescription', the id declared for Add-in DockableWindow class 'DockableWindow_Legend_ItemDescription+AddinImpl'
            /// </summary>
            internal static string DockableWindow_Legend_ItemDescription {
                get {
                    return "NRCan-RNCan_Addin_ArcMap_DockableWindowCGMDescription";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_Addin_ArcMap_DockableWindowAddGeopoint', the id declared for Add-in DockableWindow class 'DockableWindow_CreateEdit_GeopointTemplate+AddinImpl'
            /// </summary>
            internal static string DockableWindow_CreateEdit_GeopointTemplate {
                get {
                    return "NRCan-RNCan_Addin_ArcMap_DockableWindowAddGeopoint";
                }
            }
            
            /// <summary>
            /// Returns 'NRCan-RNCan_Addin_ArcMap_EditorExtension', the id declared for Add-in Extension class 'EditorExtension'
            /// </summary>
            internal static string EditorExtension {
                get {
                    return "NRCan-RNCan_Addin_ArcMap_EditorExtension";
                }
            }
        }
    }
    
internal static class ArcMap
{
  private static IApplication s_app = null;
  private static IDocumentEvents_Event s_docEvent;

  public static IApplication Application
  {
    get
    {
      if (s_app == null)
      {
        s_app = Internal.AddInStartupObject.GetHook<IMxApplication>() as IApplication;
        if (s_app == null)
        {
          IEditor editorHost = Internal.AddInStartupObject.GetHook<IEditor>();
          if (editorHost != null)
            s_app = editorHost.Parent;
        }
      }
      return s_app;
    }
  }

  public static IMxDocument Document
  {
    get
    {
      if (Application != null)
        return Application.Document as IMxDocument;

      return null;
    }
  }
  public static IMxApplication ThisApplication
  {
    get { return Application as IMxApplication; }
  }
  public static IDockableWindowManager DockableWindowManager
  {
    get { return Application as IDockableWindowManager; }
  }
  public static IDocumentEvents_Event Events
  {
    get
    {
      s_docEvent = Document as IDocumentEvents_Event;
      return s_docEvent;
    }
  }
  public static IEditor Editor
  {
    get
    {
      UID editorUID = new UID();
      editorUID.Value = "esriEditor.Editor";
      return Application.FindExtensionByCLSID(editorUID) as IEditor;
    }
  }
}

namespace Internal
{
  [StartupObjectAttribute()]
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
  [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
  public sealed partial class AddInStartupObject : AddInEntryPoint
  {
    private static AddInStartupObject _sAddInHostManager;
    private List<object> m_addinHooks = null;

    [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
    public AddInStartupObject()
    {
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override bool Initialize(object hook)
    {
      bool createSingleton = _sAddInHostManager == null;
      if (createSingleton)
      {
        _sAddInHostManager = this;
        m_addinHooks = new List<object>();
        m_addinHooks.Add(hook);
      }
      else if (!_sAddInHostManager.m_addinHooks.Contains(hook))
        _sAddInHostManager.m_addinHooks.Add(hook);

      return createSingleton;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override void Shutdown()
    {
      _sAddInHostManager = null;
      m_addinHooks = null;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
    internal static T GetHook<T>() where T : class
    {
      if (_sAddInHostManager != null)
      {
        foreach (object o in _sAddInHostManager.m_addinHooks)
        {
          if (o is T)
            return o as T;
        }
      }

      return null;
    }

    // Expose this instance of Add-in class externally
    public static AddInStartupObject GetThis()
    {
      return _sAddInHostManager;
    }
  }
}
}
