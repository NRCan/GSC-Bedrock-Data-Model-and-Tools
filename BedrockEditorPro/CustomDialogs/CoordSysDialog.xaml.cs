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
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Framework.Controls;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using BedrockEditorPro.CustomDialogs;

namespace BedrockEditorPro.CustomDialogs
{
    /// <summary>
    /// Interaction logic for CoordSysDialog.xaml
    /// </summary>
    public partial class CoordSysDialog : ArcGIS.Desktop.Framework.Controls.ProWindow
    {
        private CoordSysViewModel _vm = new CoordSysViewModel();

        /// <summary>
        /// Default constructor
        /// </summary>
        public CoordSysDialog()
        {
            InitializeComponent();
            this.DataContext = _vm;
            this.CoordinateSystemsControl.SelectedSpatialReferenceChanged += (s, args) => {
                _vm.SelectedSpatialReference = args.SpatialReference;
            };
        }

        /// <summary>
        /// The selected Spatial Reference based on the picker selection
        /// </summary>
        public SpatialReference SpatialReference
        {
            get
            {
                return _vm.SelectedSpatialReference;
            }
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
