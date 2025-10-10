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
        private List<SubscriptionToken> _editevents = new List<SubscriptionToken>();

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
            // TODO – customize this method to populate the combobox with your desired items  

            if (!_isInitialized)
            {
                Clear();

                //Get list of layers and find if there is any matches for the bedrock geodatabase
                try
                {
                    await QueuedTask.Run(() =>
                    {
                        List<FeatureLayer> layerEnum = MapView.Active.Map.GetLayersAsFlattenedList().OfType<FeatureLayer>().ToList();
                        if (layerEnum != null)
                        {
                            foreach (FeatureLayer fl in layerEnum)
                            {

                                Uri flWorkspace = Workspace.GetWorkspacePathFromFeatureLayer(fl);
                                if (flWorkspace.ToString().Contains(".gdb"))
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
                                                    ComboBoxItem participantItem = new ComboBoxItem(participantName, participantCode);
                                                    if (!this.ItemCollection.Contains(participantItem))
                                                    {
                                                        Add(participantItem);
                                                    }
                                                }

                                                break; //exit the loop if we found the bedrock gdb and added the particpants
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
            SelectedItem = ItemCollection.FirstOrDefault(); //set the default item in the comboBox

          }
       
        /// <summary>
        /// The on comboBox selection change event. 
        /// </summary>
        /// <param name="item">The newly selected combo box item</param>
        protected override async void OnSelectionChange(ComboBoxItem item) 
        {

            if (item == null)
                return;

            if (string.IsNullOrEmpty(item.Text))
                return;

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
                            if (fClass != null && EditionTracker.ETListOfFeatureClasses.Contains(fClass.GetName()))
                            {
                                var tokens = new List<SubscriptionToken>();

                                //These events are fired once ~per feature~,
                                //per table
                                tokens.Add(RowCreatedEvent.Subscribe((rc) => PunchPersonInTable(), fClass));
                                tokens.Add(RowChangedEvent.Subscribe((rc) => PunchPersonInTable(), fClass));
                                _rowevents[fl.Name] = tokens;

                            }



                        }
                    }
                });

            }
            catch (Exception ex)
            {
                new ErrorService(ex).WriteToFile("", false);
            }


        }

        private bool Unregister()
        {
            //Careful here - events have to be unregistered on the same
            //thread they were registered on...hence the use of the
            //Queued Task
            QueuedTask.Run(() =>
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

                //Editing and Edit Completed.
                EditCompletingEvent.Unsubscribe(_editevents[0]);
                EditCompletedEvent.Unsubscribe(_editevents[1]);
                _editevents.Clear();
            });

            return false;
        }

        /// <summary>
        /// Will update the table in which a create or edit event occured, if given table has a creator or editor field in it.
        /// </summary>
        /// <param name="inObject">The object that is being edited by the user</param>
        /// <param name="fieldName">The field name to update with the new value</param>
        public void PunchPersonInTable()
        {
            MessageBox.Show("Participant created new row");

            ////If user is within GSC Editor
            //if (GSC_ProjectEditor.Properties.Settings.Default.dwEnabling == true)
            //{
            //    //If user has selected a proper source of data
            //    if (this.Selected != -1)
            //    {
            //        //Validate if source field exists within feature
            //        int fieldIndex = inObject.Fields.FindField(fieldName);

            //        if (fieldIndex != -1)
            //        {
            //            //Get current source code (for domain)
            //            //string selectedPerson = this.GetItem(this.Selected).Tag as String;
            //            string selectedPerson = Properties.Settings.Default.ParticipantID;

            //            //Write new code within field
            //            if (inObject.get_Value(fieldIndex).ToString() != selectedPerson)
            //            {
            //                inObject.set_Value(fieldIndex, selectedPerson);
            //                inObject.Store();
            //            }


            //        }

            //    }
            //}
        }

    }
}
