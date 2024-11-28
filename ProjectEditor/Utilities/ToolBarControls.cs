using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Desktop.AddIns;

namespace GSC_ProjectEditor.Utilities
{
    class ToolBarControls
    {

        public static void EnableDisableAllControls(bool enable)
        {
            #region Comboboxes
            string cboParticipantID = ThisAddIn.IDs.Combobox_SelectParticipant;
            Combobox_SelectParticipant cboPart = AddIn.FromID<Combobox_SelectParticipant>(cboParticipantID);
            cboPart.EnableDisable(enable);

            string cboSourceID = ThisAddIn.IDs.Combobox_SelectDataSource;
            Combobox_SelectDataSource cboSource = AddIn.FromID<Combobox_SelectDataSource>(cboSourceID);
            cboSource.EnableDisable(enable);

            #endregion

            #region Toolbar buttons
            try
            {
                string btnRefreshID = ThisAddIn.IDs.Button_RefreshSymbols;
                Button_RefreshSymbols btnRefresh = AddIn.FromID<Button_RefreshSymbols>(btnRefreshID);
                if (btnRefresh!= null)
                {
                    btnRefresh.EnableDisable(enable);
                }
                
            }
            catch (Exception)
            {

            }


            #endregion

            #region Edit Menu buttons

            string btnAddSymbolLineID = ThisAddIn.IDs.Button_CreateEdit_GeolineTemplate;
            Button_CreateEdit_GeolineTemplate btnAddSymbolLine = AddIn.FromID<Button_CreateEdit_GeolineTemplate>(btnAddSymbolLineID);
            btnAddSymbolLine.EnableDisable(enable);

            string btnAddSymbolPointID = ThisAddIn.IDs.Button_CreateEdit_GeopointTemplate;
            Button_CreateEdit_GeopointTemplate btnAddSymbolPoint = AddIn.FromID<Button_CreateEdit_GeopointTemplate>(btnAddSymbolPointID);
            btnAddSymbolPoint.EnableDisable(enable);

            string btnAddLabelID = ThisAddIn.IDs.Button_CreateEdit_LabelTemplate;
            Button_CreateEdit_LabelTemplate btnAddLabel = AddIn.FromID<Button_CreateEdit_LabelTemplate>(btnAddLabelID);
            btnAddLabel.EnableDisable(enable);

            string btnCreatePolyMapUnitID = ThisAddIn.IDs.Button_CreateEdit_CreateMapUnits;
            Button_CreateEdit_CreateMapUnits btnCreatePolyMapUnit = AddIn.FromID<Button_CreateEdit_CreateMapUnits>(btnCreatePolyMapUnitID);
            btnCreatePolyMapUnit.EnableDisable(enable);

            string btnQualityControlGeolineID = ThisAddIn.IDs.Button_CreateEdit_ValidateGeolineIntegrity;
            Button_CreateEdit_ValidateGeolineIntegrity btnQualityControlGeoline = AddIn.FromID<Button_CreateEdit_ValidateGeolineIntegrity>(btnQualityControlGeolineID);
            btnQualityControlGeoline.EnableDisable(enable);

            string btnQualityControlID = ThisAddIn.IDs.Button_CreateEdit_QualityControl;
            Button_CreateEdit_QualityControl btnQualityControl = AddIn.FromID<Button_CreateEdit_QualityControl>(btnQualityControlID);
            btnQualityControl.EnableDisable(enable);

            string btnLegendItemsID = ThisAddIn.IDs.Button_Legend_ItemsModification;
            Button_Legend_ItemsModification btnLegendItems = AddIn.FromID<Button_Legend_ItemsModification>(btnLegendItemsID);
            btnLegendItems.EnableDisable(enable);

            string btnAddGeoEventID = ThisAddIn.IDs.Button_CreateEdit_GeologicalEvents;
            Button_CreateEdit_GeologicalEvents btnAddGeoEvent = AddIn.FromID<Button_CreateEdit_GeologicalEvents>(btnAddGeoEventID);
            btnAddGeoEvent.EnableDisable(enable);

            #endregion

            #region View Menu buttons

            string btnThemeLayerID = ThisAddIn.IDs.Button_View_CreateThematicLayers;
            Button_View_CreateThematicLayers btnThemeLayer = AddIn.FromID<Button_View_CreateThematicLayers>(btnThemeLayerID);
            btnThemeLayer.EnableDisable(enable);

            string btnOverlayLayerID = ThisAddIn.IDs.Button_View_CreateMapUnitOverprintLayer;
            Button_View_CreateMapUnitOverprintLayer btnOverlayLayer = AddIn.FromID<Button_View_CreateMapUnitOverprintLayer>(btnOverlayLayerID);
            btnOverlayLayer.EnableDisable(enable);

            string btnCustomSymbolID = ThisAddIn.IDs.Button_View_KeepCustomStyle;
            Button_View_KeepCustomStyle btnCustomSymbol = AddIn.FromID<Button_View_KeepCustomStyle>(btnCustomSymbolID);
            btnCustomSymbol.EnableDisable(enable);

            #endregion

            #region Dissemination Menu buttons

            string btnCGMItemsID = ThisAddIn.IDs.Button_Legend_IntersectItemsWithStudyArea;
            Button_Legend_IntersectItemsWithStudyArea btnCGMItems = AddIn.FromID<Button_Legend_IntersectItemsWithStudyArea>(btnCGMItemsID);
            btnCGMItems.EnableDisable(enable);

            string btnCGMDescID = ThisAddIn.IDs.Button_Legend_ItemDescription;
            Button_Legend_ItemDescription btnCGMDesc = AddIn.FromID<Button_Legend_ItemDescription>(btnCGMDescID);
            btnCGMDesc.EnableDisable(enable);

            string btnCGMID = ThisAddIn.IDs.Button_Legend_SurroundInformation;
            Button_Legend_SurroundInformation btnCGM = AddIn.FromID<Button_Legend_SurroundInformation>(btnCGMID);
            btnCGM.EnableDisable(enable);

            string btnCGMOrderID = ThisAddIn.IDs.Button_Legend_ItemsOrder;
            Button_Legend_ItemsOrder btnCGMOrder = AddIn.FromID<Button_Legend_ItemsOrder>(btnCGMOrderID);
            btnCGMOrder.EnableDisable(enable);

            string btnCGMLegendGenID = ThisAddIn.IDs.Button_Legend_CreateTempTableLegendGenerator;
            Button_Legend_CreateTempTableLegendGenerator btnCGMLegendGen = AddIn.FromID<Button_Legend_CreateTempTableLegendGenerator>(btnCGMLegendGenID);
            btnCGMLegendGen.EnableDisable(enable);

            string btnCreatePubID = ThisAddIn.IDs.Button_Legend_CreateDataBundle;
            Button_Legend_CreateDataBundle btnCreatePub = AddIn.FromID<Button_Legend_CreateDataBundle>(btnCreatePubID);
            btnCreatePub.EnableDisable(enable); //TODO replace when tool is done.

            string btnExportLegendID = ThisAddIn.IDs.Button_Legend_Export;
            Button_Legend_Export btnExportLegend = AddIn.FromID<Button_Legend_Export>(btnExportLegendID);
            btnExportLegend.EnableDisable(enable);

            string btnImportLegendID = ThisAddIn.IDs.Button_Legend_Import;
            Button_Legend_Import btnImportLegend = AddIn.FromID<Button_Legend_Import>(btnImportLegendID);
            btnImportLegend.EnableDisable(enable);

            #endregion

            //#region Environment Menu buttons

            //string btnNewGDBID = ThisAddIn.IDs.ButtonNewGeodatabase;
            //ButtonNewGeodatabase btnNewGDB = AddIn.FromID<ButtonNewGeodatabase>(btnNewGDBID);
            //btnNewGDB.EnableDisable(enable);

            //string btnCreateTopoID = ThisAddIn.IDs.ButtonCreateTopology;
            //ButtonCreateTopology btCreateTopo = AddIn.FromID<ButtonCreateTopology>(btnCreateTopoID);
            //btCreateTopo.EnableDisable(enable);

            //string btnCreateMXDID = ThisAddIn.IDs.ButtonCreateMXD;
            //ButtonCreateMXD btnCreateMXD = AddIn.FromID<ButtonCreateMXD>(btnCreateMXDID);
            //btnCreateMXD.EnableDisable(enable);

            //string btnUpgradeID = ThisAddIn.IDs.ButtonUpgradeGDBVersion;
            //ButtonUpgradeGDBVersion btnUpgrade = AddIn.FromID<ButtonUpgradeGDBVersion>(btnUpgradeID);
            //btnUpgrade.EnableDisable(enable);

            //#endregion

            #region Metadata Menu buttons

            string btnProjectID = ThisAddIn.IDs.Button_ProjectMetadata_Definition;
            Button_ProjectMetadata_Definition btnProject = AddIn.FromID<Button_ProjectMetadata_Definition>(btnProjectID);
            btnProject.EnableDisable(enable);

            string btnActivityID = ThisAddIn.IDs.Button_ProjectMetadata_Activities;
            Button_ProjectMetadata_Activities btnActivity = AddIn.FromID<Button_ProjectMetadata_Activities>(btnActivityID);
            btnActivity.EnableDisable(enable);

            string btnOrgID = ThisAddIn.IDs.Button_ProjectMetadata_Organization;
            Button_ProjectMetadata_Organization btnOrg = AddIn.FromID<Button_ProjectMetadata_Organization>(btnOrgID);
            btnOrg.EnableDisable(enable);

            string btnPartID = ThisAddIn.IDs.Button_ProjectMetadata_Roles;
            Button_ProjectMetadata_Roles btnPart = AddIn.FromID<Button_ProjectMetadata_Roles>(btnPartID);
            btnPart.EnableDisable(enable);

            string btnPersonID = ThisAddIn.IDs.Button_ProjectMetadata_Participants;
            Button_ProjectMetadata_Participants btnPerson = AddIn.FromID<Button_ProjectMetadata_Participants>(btnPersonID);
            btnPerson.EnableDisable(enable);

            #endregion

            #region Load Menu buttons

            string btnSourceID = ThisAddIn.IDs.Button_Load_SourceInformation;
            Button_Load_SourceInformation btnSource = AddIn.FromID<Button_Load_SourceInformation>(btnSourceID);
            btnSource.EnableDisable(enable);

            string btnStudyID = ThisAddIn.IDs.Button_Load_StudyAreas;
            Button_Load_StudyAreas btnStudy = AddIn.FromID<Button_Load_StudyAreas>(btnStudyID);
            btnStudy.EnableDisable(enable);

            string btnGanfeldID = ThisAddIn.IDs.Button_Load_FieldDataGanfeld;
            Button_Load_FieldDataGanfeld btnGanfeld = AddIn.FromID<Button_Load_FieldDataGanfeld>(btnGanfeldID);
            btnGanfeld.EnableDisable(enable);

            string btnAppendID = ThisAddIn.IDs.Button_Load_LinesPoints;
            Button_Load_LinesPoints btnAppend = AddIn.FromID<Button_Load_LinesPoints>(btnAppendID);
            btnAppend.EnableDisable(enable);

            string btnCartoID = ThisAddIn.IDs.Button_Load_CartographicPoints;
            Button_Load_CartographicPoints btnCarto = AddIn.FromID<Button_Load_CartographicPoints>(btnCartoID);
            btnCarto.EnableDisable(enable);

            string btnTranslateID = ThisAddIn.IDs.Button_Load_TranslateGanfeldPointStructure;
            Button_Load_TranslateGanfeldPointStructure btnTranslate = AddIn.FromID<Button_Load_TranslateGanfeldPointStructure>(btnTranslateID);
            btnTranslate.EnableDisable(enable);

            #endregion
        }

    }
}
