using System.Windows.Forms;
using System;
namespace GSC_ProjectEditor.Properties {
    
    
    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.
    public sealed partial class Settings {
        

        /// <summary>
        /// When initialized, reset dockable window enable value to false.
        /// Prevents dw to be turned on when Arc is opened.
        /// </summary>
        public Settings() {

            try
            {
                //For each Arc Init. return dock window enabling to false
                this.dwEnabling = false;

                ////For each Arc Init. return culture to empty
                //this.Culture = "Empty";

            }
            catch(Exception AddinArcMapSettingError)
            {
                MessageBox.Show("AddinArcMapSettingError: " + AddinArcMapSettingError.Message);
            }


        }
        
        private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e) {
            // Add code to handle the SettingChangingEvent event here.
        }
        
        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e) {
            // Add code to handle the SettingsSaving event here.
        }
    }
}
