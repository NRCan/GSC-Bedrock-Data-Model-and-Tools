using ArcGIS.Core.CIM;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BedrockEditorPro.Utilities
{
    /// <summary>
    /// A class that will help build some user interfaces with layers and their symbols as icons
    /// Viewmodel of the arc pro forms should inherit this class
    /// </summary>
    public class Layers: PropertyChangedBase
    {
        public class LayerDisplay
        {
            public string Name { get; set; }
            public FeatureLayer FLayer { get; set; }
            public BitmapSource Icon { get; set; }
            public bool IsChecked { get; set; }
        }

        /// <summary>
        /// Will create a combobox item with a layer file type along a little bitmap image of its symbols
        /// </summary>
        /// <param name="cimFeatureLayer"></param>
        /// <returns></returns>
        public static LayerDisplay MakeComboBoxItemWithSymbolIcons(CIMFeatureLayer cimFeatureLayer, FeatureLayer fl)
        {
            CIMSymbol sym = null;
            SymbolStyleItem si = null;
            BitmapSource bm = null;

            //Check for single renderer first
            CIMSimpleRenderer cimRenderer = cimFeatureLayer.Renderer as CIMSimpleRenderer;
            if (cimRenderer != null)
            {
                sym = cimRenderer.Symbol.Symbol;
            }
            else
            {
                //Get first symbol of first class instead
                CIMUniqueValueRenderer cimURenderer = cimFeatureLayer.Renderer as CIMUniqueValueRenderer;
                if (cimURenderer != null && cimURenderer.Groups.Count() > 0 && cimURenderer.Groups[0].Classes.Count() > 0 &&
                    cimURenderer.Groups[0].Classes[0].Symbol != null)
                {
                    sym = cimURenderer.Groups[0].Classes[0].Symbol.Symbol;
                }
            }

            //Create a bitmap image for the icon in the combobox, if a symbol was detected
            if (sym != null)
            {
                si = new SymbolStyleItem()
                {
                    Symbol = sym,
                    PatchHeight = 15,
                    PatchWidth = 15
                };
                bm = si.PreviewImage as BitmapSource;
                bm.Freeze();
            }

            //Create the combobox item
            LayerDisplay newLayerDisplay = new LayerDisplay
            {
                Name = fl.Name,
                FLayer = fl,
                Icon = bm,
                IsChecked = false
            };

            return newLayerDisplay;
        }

    }
}
