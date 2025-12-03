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
    /// Interaction logic for Form_Legend_ItemsModification.xaml
    /// </summary>
    public partial class Form_Legend_ItemsModification : ArcGIS.Desktop.Framework.Controls.ProWindow
    {
        public Form_Legend_ItemsModification()
        {
            InitializeComponent();

            this.DataContext = new Form_Legend_ItemsModificationViewModel(this);
        }
    }
}
