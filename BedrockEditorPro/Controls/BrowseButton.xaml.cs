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
    public partial class BrowseButton : UserControl
    {
        public BrowseButton()
        {
            InitializeComponent();
        }

        public ImageSource BrowserButtonImage
        {
            get
            {
                var imageSource = System.Windows.Application.Current.Resources["FolderOpenState16"] as ImageSource;
                return imageSource;
            }

        }
    }
}
