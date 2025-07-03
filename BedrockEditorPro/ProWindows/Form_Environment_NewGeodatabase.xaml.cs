using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BedrockEditorPro.ProWindows;

namespace BedrockEditorPro.ProWindows
{
    /// <summary>
    /// Interaction logic for Form_Environment_NewGeodatabase.xaml
    /// </summary>
    public partial class Form_Environment_NewGeodatabase : ArcGIS.Desktop.Framework.Controls.ProWindow
    {
        Form_Environment_NewGeodatabaseViewModel viewModel = new Form_Environment_NewGeodatabaseViewModel();

        public Form_Environment_NewGeodatabase()
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }

        /// <summary>
        /// Disable the whole XML file block if user wants to use it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (this.NewGeodatabaseXMLBrowse != null)
            {
                NewGeodatabaseXMLLabel.IsEnabled = NewGeodatabaseXMLTextbox.IsEnabled = NewGeodatabaseXMLBrowse.IsEnabled = false;
            }
            
        }

        /// <summary>
        /// Enable the whole XML file block if user wants to use it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (this.NewGeodatabaseXMLBrowse != null)
            {
                NewGeodatabaseXMLLabel.IsEnabled = NewGeodatabaseXMLTextbox.IsEnabled = NewGeodatabaseXMLBrowse.IsEnabled = true;
            }
        }
    }
}
