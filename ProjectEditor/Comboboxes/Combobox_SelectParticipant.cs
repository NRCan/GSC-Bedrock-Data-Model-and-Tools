using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Desktop.AddIns;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;

namespace GSC_ProjectEditor
{
    public class Combobox_SelectParticipant : ESRI.ArcGIS.Desktop.AddIns.ComboBox
    {
        #region Main Variables

        //P_PERSON table
        private const string tPerson = GSC_ProjectEditor.Constants.Database.TPerson;
        private const string tPersonAbbre = GSC_ProjectEditor.Constants.DatabaseFields.PersonAbbr;
        private const string tPersonID = GSC_ProjectEditor.Constants.DatabaseFields.PersonID;

        //Participant table and domain
        private const string tParticipant = GSC_ProjectEditor.Constants.Database.TParticipant;
        private const string tPartID = GSC_ProjectEditor.Constants.DatabaseFields.ParticipantID;
        private const string partDomain = GSC_ProjectEditor.Constants.DatabaseDomains.participant;

        private IEditor _editorParticipant;
        private IEditEvents_Event _editEventsParticipant;

        public bool isProjectValid = false;

        #endregion

        public Combobox_SelectParticipant()
        {

            //Catch event for dock window enabling.
            GSC_ProjectEditor.Properties.Settings.Default.SettingChanging += new System.Configuration.SettingChangingEventHandler(Default_SettingChanging);

            _editorParticipant = Utilities.EditSession.GetEditor();
            _editEventsParticipant = _editorParticipant as IEditEvents_Event;
            _editEventsParticipant.OnStartEditing += new IEditEvents_OnStartEditingEventHandler(_editEventsParticipant_OnStartEditing);

        }

        void _editEventsParticipant_OnStartEditing()
        {
            _editEventsParticipant.OnCreateFeature += new IEditEvents_OnCreateFeatureEventHandler(_editEventsParticipant_OnCreateFeature);
            _editEventsParticipant.OnChangeFeature += new IEditEvents_OnChangeFeatureEventHandler(_editEventsParticipant_OnChangeFeature);
        }

        void _editEventsParticipant_OnChangeFeature(IObject obj)
        {
            if (!Properties.Settings.Default.RefreshButton)
            {
                //Get proper field name to update
                string editorField = GSC_ProjectEditor.Constants.DatabaseFields.ETEditorID;

                //udpdate table with requested value
                punchPersonInTable(obj, editorField); 
            }


        }

        void _editEventsParticipant_OnCreateFeature(IObject obj)
        {
            //Get proper field name to update
            string creatorField = GSC_ProjectEditor.Constants.DatabaseFields.ETCreatorID;
            string editorField = GSC_ProjectEditor.Constants.DatabaseFields.ETEditorID;

            //udpdate table with requested value
            punchPersonInTable(obj, creatorField);
            punchPersonInTable(obj, editorField);
        }


        /// <summary>
        /// Triggered when dwEnable internal setting is changed. Will serve to activate combobox and fill,
        /// it with person from person table, within project database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">The setting itself</param>
        public void Default_SettingChanging(object sender, System.Configuration.SettingChangingEventArgs e)
        {

            //Manage Select participant Combobox fill here--------------------
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
                        restrictedDataset.Add(Constants.Database.TParticipant);
                        string currentWorkspacePath = Properties.Settings.Default.PROJECT_DATABASE_PATH;
                        IWorkspace currentWorkspace = Workspace.AccessWorkspace(currentWorkspacePath);
                        isProjectValid = Workspace.isWorkspaceAProperProjectDatabase(currentWorkspace, restrictedDataset, false);
                        if (isProjectValid)
                        {
                            FillInCombobox();
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
            else if (e.SettingName == "refreshParticipant")
            {
                if ((bool)e.NewValue == true)
                {
                    FillInCombobox();

                    Properties.Settings.Default.refreshParticipant = false;
                    Properties.Settings.Default.Save();
                }
            }


        }

        public void FillInCombobox()
        {


            if (isProjectValid)
            {
                //Init variables
                List<int> cookieList = new List<int>();
                Dictionary<string, int> personIDList = new Dictionary<string, int>();

                //Other variables
                string currentParticipantID = Properties.Settings.Default.ParticipantID;

                //Build list of actual values
                List<string> currenItemNames = new List<string>();
                foreach (Item item in this.items)
                {
                    currenItemNames.Add(item.Caption);
                }

                //Fill in combobox
                Dictionary<string, string> participantDico = GSC_ProjectEditor.Domains.GetDomDico(partDomain, "Description");
                foreach (KeyValuePair<string, string> sources in participantDico)
                {
                    //Check for existing value
                    if (!currenItemNames.Contains(sources.Key))
                    {
                        int currentCookie = this.Add(sources.Key, sources.Value); //Add domain code as tag, and keep index within combobox (cookie)
                        cookieList.Add(currentCookie); //Add new item combobox index within list
                                                       //Build dico of personID list for futur selection purposes
                        personIDList[sources.Value] = currentCookie;
                    }

                }

                //Check for any existing value within internal settings and select it if needed
                if (Properties.Settings.Default.ParticipantID != "Empty")
                {
                    if (personIDList.ContainsKey(currentParticipantID))
                    {
                        this.Select(personIDList[currentParticipantID]);
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
            else
            {
                this.Clear();
            }
        }

        /// <summary>
        /// Will update the table in which a create or edit event occured, if given table has a creator or editor field in it.
        /// </summary>
        /// <param name="inObject">The object that is being edited by the user</param>
        /// <param name="fieldName">The field name to update with the new value</param>
        public void punchPersonInTable(IObject inObject, string fieldName)
        {
            //If user is within GSC Editor
            if (GSC_ProjectEditor.Properties.Settings.Default.dwEnabling == true)
            {
                //If user has selected a proper source of data
                if (this.Selected != -1)
                {
                    //Validate if source field exists within feature
                    int fieldIndex = inObject.Fields.FindField(fieldName);

                    if (fieldIndex != -1)
                    {
                        //Get current source code (for domain)
                        //string selectedPerson = this.GetItem(this.Selected).Tag as String;
                        string selectedPerson = Properties.Settings.Default.ParticipantID;

                        //Write new code within field
                        if (inObject.get_Value(fieldIndex).ToString() != selectedPerson)
                        {
                            inObject.set_Value(fieldIndex, selectedPerson);
                            inObject.Store();
                        }


                    }

                }
            }
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
                GSC_ProjectEditor.Properties.Settings.Default.ParticipantID = this.GetItem(cookie).Tag.ToString();
                GSC_ProjectEditor.Properties.Settings.Default.Save();
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
    }

}
