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

        public Form_Environment_NewGeodatabase()
        {
            InitializeComponent();

            this.DataContext = new Form_Environment_NewGeodatabaseViewModel(this);
        }
    }
}
