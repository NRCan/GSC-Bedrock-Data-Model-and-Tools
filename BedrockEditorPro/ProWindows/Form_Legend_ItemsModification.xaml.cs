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
        private List<Button> _listOfButtons = new List<Button>();

        public Form_Legend_ItemsModification()
        {
            InitializeComponent();

            this.DataContext = new Form_Legend_ItemsModificationViewModel(this);

            Form_Legend_ItemsModificationViewModel viewModel = this.DataContext as Form_Legend_ItemsModificationViewModel;
            if (viewModel != null)
            {
                _listOfButtons.Clear();
                _listOfButtons = FindChildrenOfType<Button>(this).ToList();

                viewModel.listOfAllButtons = _listOfButtons;
            }

        }


        /// <summary>
        /// Will output a list of all children controls of a special type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <returns></returns>
        public IEnumerable<T> FindChildrenOfType<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent is ContentControl contentControl)
            {
                if (contentControl.Content is T contentOfT)
                {
                    yield return contentOfT;
                }

                if (contentControl.Content is DependencyObject dependencyObjectContent)
                {
                    foreach (T grandChild in FindChildrenOfType<T>(dependencyObjectContent))
                    {
                        yield return grandChild;
                    }
                }
            }
            else
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                    if (child is T childOfT)
                    {
                        yield return childOfT;
                    }

                    foreach (T grandChild in FindChildrenOfType<T>(child))
                    {
                        yield return grandChild;
                    }
                }
            }
        }
    }
}
