using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Mapping.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BedrockEditorPro.UI
{
    /// <summary>
    /// Source: https://github.com/Esri/arcgis-pro-sdk-community-samples/blob/master/Geometry/CoordinateSystemDialog/UI/CoordSysViewModel.cs
    /// </summary>

    internal class CoordSysViewModel : INotifyPropertyChanged
    {
        private bool _showVCS = false;
        private SpatialReference _sr;
        private CoordinateSystemsControlProperties _props = null;

        public CoordSysViewModel()
        {
            UpdateCoordinateControlProperties();
        }

        public string SelectedCoordinateSystemName => _sr != null ? _sr.Name : "";

        public SpatialReference SelectedSpatialReference
        {
            get
            {
                return _sr;
            }
            set
            {
                _sr = value;
                NotifyPropertyChanged();
            }
        }

        public bool ShowVCS
        {
            get
            {
                return _showVCS;
            }
            set
            {
                if (_showVCS != value)
                {
                    _showVCS = value;
                    UpdateCoordinateControlProperties();
                    NotifyPropertyChanged();
                }
            }
        }

        public CoordinateSystemsControlProperties ControlProperties
        {
            get
            {
                return _props;
            }
            set
            {
                _props = value;
                NotifyPropertyChanged();
            }
        }

        private void UpdateCoordinateControlProperties()
        {
            var map = MapView.Active?.Map;
            var props = new CoordinateSystemsControlProperties()
            {
                Map = map,
                SpatialReference = this._sr,
                ShowVerticalCoordinateSystems = this.ShowVCS
            };
            this.ControlProperties = props;
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void NotifyPropertyChanged([CallerMemberName] string propName = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
