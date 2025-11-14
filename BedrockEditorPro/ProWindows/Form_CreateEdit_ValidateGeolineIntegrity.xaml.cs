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
    /// Interaction logic for Form_CreateEdit_ValidateGeolineIntegrity.xaml
    /// </summary>
    public partial class Form_CreateEdit_ValidateGeolineIntegrity : ArcGIS.Desktop.Framework.Controls.ProWindow
    {
        public Form_CreateEdit_ValidateGeolineIntegrity()
        {
            InitializeComponent();

            this.DataContext = new Form_CreateEdit_ValidateGeolineIntegrityViewModel(this);
        }
    }
}
