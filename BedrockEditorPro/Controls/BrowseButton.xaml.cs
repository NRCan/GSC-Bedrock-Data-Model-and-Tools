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

        //TODO: Attempt at binding properties - it gets mixed up with the view model from the calling form

        //public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(
        //    nameof(BrowserButtonImage),
        //    typeof(ImageSource),
        //    typeof(BrowseButton),
        //    new UIPropertyMetadata(null)
        //);

        //public ImageSource BrowserButtonImage
        //{
        //    get { return (ImageSource)GetValue(ImageSourceProperty); }
        //    set { SetValue(ImageSourceProperty, value); }
        //}

    }
}
