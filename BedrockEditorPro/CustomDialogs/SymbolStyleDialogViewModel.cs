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

namespace BedrockEditorPro.CustomDialogs
{
    public class SymbolStyleDialogViewModel : PropertyChangedBase
    {
        private Element _selectedElement;
        private List<Element> _selectedElements = new List<Element>();
        private SymbolStyleDialog _view = null;
        StyleProjectItem _styleProjectItem = null;
        IList<SymbolStyleItem> _searchList = null;
        IEnumerable<SymbolStyleItem[]> _chunkedList = null;
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

        public SymbolStyleDialogViewModel(SymbolStyleDialog view, StyleItemType styleItemType)
        {
            _view = view;
            //Subscribe to the SelectedElementsChangedEvent to know when the selection changes
            //SelectedElementsChangedEvent.Subscribe(OnSelectedElementsChanged);
            //Initialize the selected elements collection
            string stylePath = Utilities.Symbols.ManageStyleFile();
            _styleProjectItem = Utilities.Symbols.GetStyleItemProject(stylePath);
            _searchList = _styleProjectItem.SearchSymbols(styleItemType, null);
            _chunkedList = _searchList.Chunk(36);

            UpdateSymbolCollection(styleItemType, true);

        }

        /// <summary>
        /// This method is triggered when the Selected elements event is fired. It checks if the active pane is a layout. If not a layout, then it checks if it is a map view.
        /// The selected elements in these views checked if they are of the same type. Are they all point elements, for example? The first element determines "THE type" to compare against.
        /// If they are the same type, they get added to the _selectedElements member variable. SymbolStyleItemCollection is the MVVM Binding variable that gets updated with Point symbols,
        /// if _SelectedElements are all points.
        /// </summary>
        public async void UpdateSymbolCollection(StyleItemType itemstyle = StyleItemType.PolygonSymbol, bool quickViewMode = false)
        {
            _symbolStyleItemCollection.Clear();

            foreach (SymbolStyleItem[] item in _chunkedList)
            {
                foreach (var i in item)
                {
                    GeometrySymbolItem itemToAdd = new GeometrySymbolItem(i, itemstyle);
                    _symbolStyleItemCollection.Add(itemToAdd);
                }
                NotifyPropertyChanged(nameof(SymbolStyleItemCollection));

                if (quickViewMode)
                {
                    //Will break the loop after the first chunk to allow for quick viewing of symbols
                    break;
                }
                
            }

        }
    }
}
