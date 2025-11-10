using BedrockEditorPro.ProWindows;
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

namespace BedrockEditorPro.UI
{
    /// <summary>
    /// Interaction logic for SymbolStyleDialog.xaml
    /// </summary>
    public partial class SymbolStyleDialog : ArcGIS.Desktop.Framework.Controls.ProWindow
    {
        SymbolStyleDialogViewModel _vm; 
        public SymbolStyleDialog()
        {
            InitializeComponent();
            this.DataContext = _vm = new SymbolStyleDialogViewModel(this);
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// The selected style item based on the picker selection
        /// </summary>
        public GeometrySymbolItem SelectedSymbolItem
        {
            get
            {
                return _vm.SelectedItem;
            }
        }
    }
}
