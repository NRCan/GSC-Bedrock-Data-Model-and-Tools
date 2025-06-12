using ArcGIS.Desktop.Framework.Controls;
using GSCFieldApp.Services;
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

namespace BedrockEditorPro.Controls
{
    /// <summary>
    /// Interaction logic for BrowseButton.xaml
    /// </summary>
    public partial class CancelButton : UserControl
    {

        public CancelButton()
        {
            InitializeComponent();
        }

        /// Event handler for the Cancel button click that will close the
        /// currently opened window.
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            //Get parent grid
            Grid parentGrid = this.Parent as Grid;
            if (parentGrid != null)
            {
                //Get parent window
                ProWindow parentWindow = parentGrid.Parent as ProWindow;

                if (parentWindow != null)
                {
                    parentWindow.Close();
                }
                else
                {
                    new ErrorToLogFile("Button Cancel: Parent window is null. Cannot close the window.").WriteToFile();
                }
            }
            
        }

    }
}
