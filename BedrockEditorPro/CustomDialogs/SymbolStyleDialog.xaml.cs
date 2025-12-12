using ArcGIS.Desktop.Mapping;
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

namespace BedrockEditorPro.CustomDialogs
{
    /// <summary>
    /// Interaction logic for SymbolStyleDialog.xaml
    /// </summary>
    public partial class SymbolStyleDialog : ArcGIS.Desktop.Framework.Controls.ProWindow
    {
        SymbolStyleDialogViewModel _vm;
        StyleItemType _styleItemType;
        public SymbolStyleDialog(StyleItemType styleItemType)
        {
            InitializeComponent();
            _styleItemType = styleItemType;
            this.DataContext = _vm = new SymbolStyleDialogViewModel(this, _styleItemType);

            this.ContentRendered += SymbolStyleDialog_ContentRendered;
        }

        private void SymbolStyleDialog_ContentRendered(object sender, EventArgs e)
        {
            Task.Delay(100);
            _vm.UpdateSymbolCollection(_styleItemType);
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
