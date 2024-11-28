using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using ESRI.ArcGIS.Desktop.AddIns;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using System.Collections;

namespace GSC_ProjectEditor
{
    /// <summary>
    /// This combobox will be filled with domain values from domain SourceRef_PID, inside project database
    /// Needs to be filled only in an edit session.
    /// Selection will be saved within all new created features.
    /// </summary>
    public class Combobox_SelectDataSource : ESRI.ArcGIS.Desktop.AddIns.ComboBox
    {
        private IEditor _editor;
        private IEditEvents_Event _editEvents;

        
        /// <summary>
        /// Init. of combobox
        /// </summary>
        public Combobox_SelectDataSource()
        {

            //Catch event for dock window enabling.
            GSC_ProjectEditor.Properties.Settings.Default.SettingChanging += new System.Configuration.SettingChangingEventHandler(Default_SettingChanging);

            _editor = Utilities.EditSession.GetEditor();
            _editEvents = _editor as IEditEvents_Event;
            _editEvents.OnStartEditing += new IEditEvents_OnStartEditingEventHandler(edEv_OnStartEditing);

        }

        void edEv_OnStartEditing()
        {
            _editEvents.OnCreateFeature += new IEditEvents_OnCreateFeatureEventHandler(_editEvents_OnCreateFeature);
            _editEvents.OnChangeFeature += new IEditEvents_OnChangeFeatureEventHandler(_editEvents_OnChangeFeature);
        }

        void _editEvents_OnChangeFeature(IObject obj)
        {
            //string sourceField = GSC_ProjectEditor.Constants.DatabaseFields.SourceID;

            ////If user is within GSC Editor
            //if (GSC_ProjectEditor.Properties.Settings.Default.dwEnabling == true)
            //{
            //    //If user has selected a proper source of data
            //    if (this.Selected != -1)
            //    {
            //        //Validate if source field exists within feature
            //        int sourceIndex = obj.Fields.FindField(sourceField);

            //        if (sourceIndex != -1)
            //        {
            //            //Get current source code (for domain)
            //            string selectedDomCode = this.GetItem(this.Selected).Tag as String;
            //            //MessageBox.Show(selectedDomCode);
            //            //Write code within field
            //            obj.set_Value(sourceIndex, selectedDomCode);
            //            //obj.Store();
            //        }

            //    }
            //}
        }

        void _editEvents_OnCreateFeature(IObject obj)
        {
            string sourceField = GSC_ProjectEditor.Constants.DatabaseFields.SourceID;

            //If user is within GSC Editor
            if (GSC_ProjectEditor.Properties.Settings.Default.dwEnabling == true)
            {
                //If user has selected a proper source of data
                if (this.Selected != -1)
                {
                    //Validate if source field exists within feature
                    int sourceIndex = obj.Fields.FindField(sourceField);

                    if (sourceIndex != -1)
                    {
                        //Get current source code (for domain)
                        string selectedDomCode = this.GetItem(this.Selected).Tag as String;
                        //MessageBox.Show(selectedDomCode);
                        //Write code within field
                        obj.set_Value(sourceIndex, selectedDomCode);
                        //obj.Store();
                    }

                }
            }
        }

        /// <summary>
        /// Triggered when dwEnable internal setting is changed. Will serve to activate combobox and fill,
        /// it with sources from domain, within project database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">The setting itself</param>
        public void Default_SettingChanging(object sender, System.Configuration.SettingChangingEventArgs e)
        {
            
            //Manage Select Source Combobox fill here--------------------
            if (e.SettingName == "dwEnabling" && Properties.Settings.Default.SaveEditButton == false)
            {

                Utilities.Culture.SetCulture();

                try
                {
                    //Cast new value
                    bool getNewValue = Convert.ToBoolean(e.NewValue);
                    if (getNewValue == true)
                    {
                        List<string> restrictedDataset = new List<string>();
                        restrictedDataset.Add(Constants.Database.TSource);
                        string currentWorkspacePath = Properties.Settings.Default.PROJECT_DATABASE_PATH;
                        IWorkspace currentWorkspace = Workspace.AccessWorkspace(currentWorkspacePath);
                        bool isProjectValid = Workspace.isWorkspaceAProperProjectDatabase(currentWorkspace, restrictedDataset, false);

                        if (isProjectValid)
                        {
                            FillInSource();
                        }


                    }
                    else
                    {
                        this.Clear();
                    }
                }
                catch (Exception)
                {

                }
            }
            else if (e.SettingName == "refreshSource")
            {
                if ((bool)e.NewValue == true)
                {
                    FillInSource();

                    Properties.Settings.Default.refreshSource = false;
                    Properties.Settings.Default.Save();
                }
            }


        }

        public void FillInSource()
        {
            //Variables
            List<int> cookieList = new List<int>();
            Dictionary<string, int> sourceIDList = new Dictionary<string, int>();
            string sourceDom = GSC_ProjectEditor.Constants.DatabaseDomains.Source;

            //Other variables
            string currentSourceID = Properties.Settings.Default.SourceID;

            //Build list of actual values
            List<string> currenItemNames = new List<string>();
            foreach (Item item in this.items)
            {
                currenItemNames.Add(item.Caption);
            }

            //Fill in combobox
            Dictionary<string, string> sourceDico = GSC_ProjectEditor.Domains.GetDomDico(sourceDom, "Description");
            foreach (KeyValuePair<string, string> sources in sourceDico)
            {
                //Check for existing value
                if (!currenItemNames.Contains(sources.Key))
                {
                    int currentCookie = this.Add(sources.Key, sources.Value); //Add domain code as tag, and keep index within combobox (cookie)
                    cookieList.Add(currentCookie); //Add new item combobox index within list

                    //Build dico of personID list for futur selection purposes
                    sourceIDList[sources.Value] = currentCookie;

                }


            }

            //Check for any existing value within internal settings and select it if needed
            if (Properties.Settings.Default.SourceID != "Empty")
            {
                if (sourceIDList.ContainsKey(currentSourceID))
                {
                    this.Select(sourceIDList[currentSourceID]);
                }
            }
            else
            {
                //Select first item in combobox.
                if (cookieList.Count != 0)
                {
                    this.Select(cookieList[0]);
                }

            }
        }

        /// <summary>
        /// Will enable or disable current control class.
        /// </summary>
        /// <param name="enable"></param>
        public void EnableDisable(bool enable)
        {
            if (enable)
            {
                this.Enabled = true;
            }
            else
            {
                this.Enabled = false;
            }
        }

        protected override void OnUpdate()
        {
            this.Enabled = ArcMap.Application != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cookie">index</param>
        protected override void OnSelChange(int cookie)
        {

            //Update internal setting with proper combobox person id
            if (cookie >= 0)
            {
                GSC_ProjectEditor.Properties.Settings.Default.SourceID = this.GetItem(cookie).Tag.ToString();
                GSC_ProjectEditor.Properties.Settings.Default.Save();
            }

        }
    }

}
