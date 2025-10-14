using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using BedrockEditorPro.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static System.Net.WebRequestMethods;

namespace BedrockEditorPro.ProWindows
{

    public class Form_HelpViewModel: PropertyChangedBase
    {
        #region INIT
        private Form_Help _view = null;
        private WorkingEnvironment _workingEnvironment = new WorkingEnvironment();
        private enum webPageType { OnlineGuide, ReportIssue, DataModel, ProjectPage }
        #endregion

        #region PROPERTIES
        private string _addinVersion = string.Empty;
        public string AddinVersion
        {
            get { return _addinVersion; }
            set
            {
                SetProperty(ref _addinVersion, value, () => _addinVersion);
            }
        }

        private string _databaseVersion = string.Empty;
        public string DatabaseVersion
        {
            get { return string.Format("{0}: {1}", Properties.Resources.FormHelpDatabaseVersion, Constants.Database.CurrentDBVersion); }
        }

        #endregion

        #region RELAYS

        private ICommand _openOnlineGuide = null;
        public ICommand OpenOnlineGuide
        {
            get
            {
                if (_openOnlineGuide == null)
                {
                    _openOnlineGuide = new RelayCommand(() => OpenWebPage(webPageType.OnlineGuide), () => true);
                }
                return _openOnlineGuide;
            }
        }

        private ICommand _openOnlineModel = null;
        public ICommand OpenOnlineModel
        {
            get
            {
                if (_openOnlineModel == null)
                {
                    _openOnlineModel = new RelayCommand(() => OpenWebPage(webPageType.DataModel), () => true);
                }
                return _openOnlineModel;
            }
        }

        private ICommand _openOnlineIssue = null;
        public ICommand OpenOnlineIssue
        {
            get
            {
                if (_openOnlineIssue == null)
                {
                    _openOnlineIssue = new RelayCommand(() => OpenWebPage(webPageType.ReportIssue), () => true);
                }
                return _openOnlineIssue;
            }
        }

        private ICommand _openOnlineProject = null;
        public ICommand OpenOnlineProject
        {
            get
            {
                if (_openOnlineProject == null)
                {
                    _openOnlineProject = new RelayCommand(() => OpenWebPage(webPageType.ProjectPage), () => true);
                }
                return _openOnlineProject;
            }
        }

        #endregion

        #region METHODS
        public Form_HelpViewModel(Form_Help view)
        {
            _view = view;
            _addinVersion = string.Format("Version: {0}", _workingEnvironment.WorkingEnvironmentPath.Substring(_workingEnvironment.WorkingEnvironmentPath.LastIndexOf(" ")));  
        }

        /// <summary>
        /// Will open a web page in the default browser with the desire hyperlink passed as parameter.
        /// </summary>
        /// <param name="pageType"></param>
        private void OpenWebPage(webPageType pageType)
        {
            string url = "https://github.com/NRCan/GSC-Bedrock-Data-Model-and-Tools"; // Default URL

            switch (pageType)
            {
                case webPageType.OnlineGuide:
                    url = "https://doi.org/10.4095/314673";
                    break;
                case webPageType.ReportIssue:
                    url = "https://github.com/NRCan/GSC-Bedrock-Data-Model-and-Tools/issues";
                    break;
                case webPageType.DataModel:
                    url = "https://doi.org/10.4095/314673";
                    break;
                case webPageType.ProjectPage:
                    url = "https://github.com/NRCan/GSC-Bedrock-Data-Model-and-Tools/releases";
                    break;
                default:
                    break;
            }

            try
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }

            }
            catch (Exception ex)
            {
                new ErrorService(ex).WriteToFile();
            }
        }

        #endregion
    }
}