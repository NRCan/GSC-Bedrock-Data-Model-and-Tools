using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Layouts;
using ArcGIS.Desktop.Layouts.Events;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Core.Data.Exceptions;
using ArcGIS.Core.Data.LinearReferencing;
using ArcGIS.Core.Internal.CIM;
using ArcGIS.Desktop.Internal.Mapping;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
namespace BedrockEditorPro.UI
{
    public class SymbolStyleDialogViewModel : PropertyChangedBase
    {
        private Element _selectedElement;
        private List<Element> _selectedElements = new List<Element>();
        private SymbolStyleDialog _view = null;

        private ObservableCollection<GeometrySymbolItem> _symbolStyleItemCollection = new ObservableCollection<GeometrySymbolItem>();

        public ObservableCollection<GeometrySymbolItem> SymbolStyleItemCollection
        {
            get => _symbolStyleItemCollection;
        }

        private GeometrySymbolItem _selectedItem;
        public GeometrySymbolItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                SetProperty(ref _selectedItem, value, () => _selectedItem);
                //ApplySelectedStyle(); //Magic happens here. When the user selects a style, we apply it to the selected elements
            }
        }

        public SymbolStyleDialogViewModel(SymbolStyleDialog view)
        {
            _view = view;
            //Subscribe to the SelectedElementsChangedEvent to know when the selection changes
            //SelectedElementsChangedEvent.Subscribe(OnSelectedElementsChanged);
            //Initialize the selected elements collection
            string stylePath = Utilities.Symbols.ManageStyleFile();
            StyleProjectItem styleProjectItem = Utilities.Symbols.GetStyleItemProject(stylePath);
            UpdateSymbolCollection(styleProjectItem);
        }

        /// <summary>
        /// This method is triggered when the Selected elements event is fired. It checks if the active pane is a layout. If not a layout, then it checks if it is a map view.
        /// The selected elements in these views checked if they are of the same type. Are they all point elements, for example? The first element determines "THE type" to compare against.
        /// If they are the same type, they get added to the _selectedElements member variable. SymbolStyleItemCollection is the MVVM Binding variable that gets updated with Point symbols,
        /// if _SelectedElements are all points.
        /// </summary>
        private async void UpdateSymbolCollection(StyleProjectItem styleProjectItem, StyleItemType itemstyle = StyleItemType.PolygonSymbol)
        {

            //Now we populate the listbox with the appropriate style items 
            //If the _selectedElements collection contains a NorthArrow, we populate the listbox with NorthArrowStyleItems, etc
            SymbolStyleItemCollection.Clear();

            foreach (var item in styleProjectItem.SearchSymbols(itemstyle, ""))
            {
                GeometrySymbolItem itemToAdd = new GeometrySymbolItem(item, itemstyle);
                SymbolStyleItemCollection.Add(itemToAdd);
            }

        }
    }
}
