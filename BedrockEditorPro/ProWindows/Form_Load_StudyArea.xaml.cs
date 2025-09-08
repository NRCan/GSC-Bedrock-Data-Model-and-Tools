using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Internal.Mapping;
using ArcGIS.Desktop.Mapping;
using BedrockEditorPro.Utilities;
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

namespace BedrockEditorPro.ProWindows
{
    /// <summary>
    /// Interaction logic for Form_Load_StudyArea.xaml
    /// </summary>
    public partial class Form_Load_StudyArea : ArcGIS.Desktop.Framework.Controls.ProWindow
    {
        public List<Layer> mapLayers = new List<Layer>();

        public Form_Load_StudyArea()
        {
            InitializeComponent();
            this.DataContext = new Form_Load_StudyAreaViewModel(this);
        }

    }
}
