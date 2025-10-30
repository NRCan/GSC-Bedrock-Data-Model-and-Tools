using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Data.Knowledge;
using ArcGIS.Core.Events;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Editing.Events;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Internal.Editing;
using ArcGIS.Desktop.KnowledgeGraph;
using ArcGIS.Desktop.Layouts;
using ArcGIS.Desktop.Mapping;
using BedrockEditorPro.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace BedrockEditorPro.Comboboxes
{
    /// <summary>
    /// Represents the ComboBox
    /// </summary>
    internal class Combobox_SelectParticipant : ComboBox 
    {

        private bool _isInitialized;

        //For the events
        private Dictionary<string, List<SubscriptionToken>> _rowevents = new Dictionary<string, List<SubscriptionToken>>();
        private List<long> _createdOIDList = new List<long>(); //Will hold the list of OIDs created during an edit session to prevent them from firing modify events 
        private Guid _editSessionID = Guid.Empty; //Will hold the edit session ID to identify when a new edit session starts
        private List<SubscriptionToken> _tokens = new List<SubscriptionToken>();

        /// <summary>
        /// Combo Box constructor
        /// </summary>
        public Combobox_SelectParticipant() 
        {
            UpdateCombo();
        }

        /// <summary>
        /// Updates the combo box with all the items.
        /// </summary>
 
        private async void UpdateCombo()
        {
            if (_isInitialized)
            {
                SelectedItem = null;
            }
                

            if (!_isInitialized)
            {
                Clear();

                //Get list of layers and find if there is any matches for the bedrock geodatabase
                try
                {
                    await QueuedTask.Run(() =>
                    {
                        if (MapView.Active != null && MapView.Active.Map != null)
                        {
                            List<FeatureLayer> layerEnum = MapView.Active.Map.GetLayersAsFlattenedList().OfType<FeatureLayer>().ToList();
                            if (layerEnum != null)
                            {
                                foreach (FeatureLayer fl in layerEnum)
                                {
                                    if (fl != null)
                                    {
                                        Uri flWorkspace = Workspace.GetWorkspacePathFromFeatureLayer(fl);
                                        if (flWorkspace != null && flWorkspace.ToString().Contains(".gdb"))
                                        {
                                            using (Geodatabase sourceGeodatabase = new Geodatabase(new FileGeodatabaseConnectionPath(flWorkspace)))
                                            {
                                                //Quick check to see if the geodatabase contains the bedrock tables
                                                bool containsLegend = Workspace.TableExists(sourceGeodatabase, Constants.Database.TLegendGene);

                                                if (containsLegend)
                                                {
                                                    //Get the domain dictionary for the participant domain
                                                    SortedList<object, string> participantDico = Domains.GetDomDicoFromWorkspace(sourceGeodatabase, Constants.DatabaseDomains.participant);

                                                    if (participantDico != null && participantDico.Count() > 0)
                                                    {
                                                        foreach (KeyValuePair<object, string> kvp in participantDico)
                                                        {
                                                            //Add new participant in the combobox
                                                            string participantName = kvp.Value;
                                                            string participantCode = kvp.Key.ToString();
                                                            ComboBoxItem participantItem = new ComboBoxItem(participantName, "", participantCode);
                                                            if (!this.ItemCollection.Contains(participantItem))
                                                            {
                                                                Add(participantItem);
                                                            }
                                                        }
                                                        SelectedItem = null;
                                                        break; //exit the loop if we found the bedrock gdb and added the particpants
                                                    }
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }

                    });

                }
                catch (Exception ex)
                {
                    new ErrorService(ex).WriteToFile("", false);
                }

                _isInitialized = true;


            }
  
            Enabled = true; //enables the ComboBox
            SelectedItem = null;

        }
       
        /// <summary>
        /// The on comboBox selection change event. 
        /// </summary>
        /// <param name="item">The newly selected combo box item</param>
        protected override async void OnSelectionChange(ComboBoxItem item) 
        {

            if (item == null)
            {
                Unregister();
                return;
            }


            if (string.IsNullOrEmpty(item.Text))
            {
                Unregister();
                return;
            }

            // Start listening to editing events on features classes that can track participants
            try
            {
                await QueuedTask.Run(() =>
                {
                    List<FeatureLayer> layerEnum = MapView.Active.Map.GetLayersAsFlattenedList().OfType<FeatureLayer>().ToList();
                    if (layerEnum != null)
                    {
                        foreach (FeatureLayer fl in layerEnum)
                        {

                            FeatureClass fClass = fl.GetFeatureClass();
                            if (fClass != null && EditionTracker.ETListOfFeatureClasses.Contains(fClass.GetName()) && !_rowevents.ContainsKey(fl.Name))
                            {
                                var tokens = new List<SubscriptionToken>();

                                //These events are fired once ~per feature~,
                                //per table
                                _tokens.Add(RowCreatedEvent.Subscribe((rc) => PunchPersonInTable(rc, fl), fClass));
                                _tokens.Add(RowChangedEvent.Subscribe((re) => PunchPersonInTable(re, fl), fClass));

                                _rowevents[fl.Name] = _tokens;

                            }



                        }
                    }
                });

            }
            catch (Exception ex)
            {
                new ErrorService(ex).WriteToFile("", false);
                Unregister();
            }


        }

        /// <summary>
        /// Make sure to unregister from all events
        /// </summary>
        /// <returns></returns>
        private bool Unregister()
        {
            //Careful here - events have to be unregistered on the same
            //thread they were registered on...hence the use of the
            //Queued Task
            QueuedTask.Run(() =>
            {
                try
                {
                    //One kvp per layer....of which there is only one in the sample
                    //out of the box but you can add others and register for events
                    foreach (var kvp in _rowevents)
                    {
                        RowCreatedEvent.Unsubscribe(kvp.Value[0]);
                        RowChangedEvent.Unsubscribe(kvp.Value[1]);
                        kvp.Value.Clear();
                    }
                    _rowevents.Clear();

                }
                catch (Exception)
                {

                }

            });

            return false;
        }

        /// <summary>
        /// Will update the table in which a create or edit event occured, if given table has a creator or editor field in it.
        /// </summary>
        /// <param name="inObject">The object that is being edited by the user</param>
        /// <param name="fieldName">The field name to update with the new value</param>
        public void PunchPersonInTable(RowChangedEventArgs rc, FeatureLayer fl)
        {
            if (!_createdOIDList.Contains(rc.Row.GetObjectID()))
            {
                //Make sure key fieldsexists
                int creatorIndex = rc.Row.FindField(Constants.DatabaseFields.ETCreatorID);
                int createdDateIndex = rc.Row.FindField(Constants.DatabaseFields.ETCreateDate);
                int editorIndex = rc.Row.FindField(Constants.DatabaseFields.ETEditorID);
                int editedDateIndex = rc.Row.FindField(Constants.DatabaseFields.ETEditDate);
                if (creatorIndex != -1 && createdDateIndex != -1 && editorIndex != -1 && editedDateIndex != -1)
                {
                    ComboBoxItem selectedPerson = this.SelectedItem as ComboBoxItem;
                    Dictionary<string, object> attributes = new Dictionary<string, object>();

                    //Punch creator in the table
                    if (rc.EditType == EditType.Create)
                    {
                        attributes[Constants.DatabaseFields.ETCreatorID] = selectedPerson.Tooltip;
                        rc.Operation.Modify(fl, rc.Row.GetObjectID(), attributes);
                        _createdOIDList.Add(rc.Row.GetObjectID()); //Add the OID to the list so we don't process it again
                    }
                    else if (rc.EditType == EditType.Change && rc.Guid != _editSessionID)
                    {
                        attributes[Constants.DatabaseFields.ETEditorID] = selectedPerson.Tooltip;
                        rc.Operation.Modify(fl, rc.Row.GetObjectID(), attributes);
                        _editSessionID = rc.Guid; //Set the edit session ID so we don't process again
                    }

                    
                }
            }

        }

    }
}
