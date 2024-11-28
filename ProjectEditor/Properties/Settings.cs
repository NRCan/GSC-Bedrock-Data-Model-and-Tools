using System.Windows.Forms;
using System;
namespace Addin_ArcMap.Properties {
    
    
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

            //For each Arc Init. return dock window enabling to false
            Addin_ArcMap.Properties.Settings.Default.dwEnabling = false;

            //For each Arc Init. return culture to empty
            Addin_ArcMap.Properties.Settings.Default.Culture = "Empty";

            // // To add event handlers for saving and changing settings, uncomment the lines below:
            //
            //this.SettingChanging += this.SettingChangingEventHandler;
            //
            // this.SettingsSaving += this.SettingsSavingEventHandler;
            //
        }
        
        private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e) {
            // Add code to handle the SettingChangingEvent event here.
        }
        
        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e) {
            // Add code to handle the SettingsSaving event here.
        }
    }
}
