using BedrockEditorPro.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSCFieldApp.Services
{
    internal class ErrorToLogFile
    {
        private WorkingEnvironment WorkingEnvironment
        {
            get
            {
                return new WorkingEnvironment();
            }
        }

        public string Message { get; set; }
        public Exception Exception { get; set; }
        public string DefaultPath
        {
            get
            {

                var folder = WorkingEnvironment.WorkingEnvironmentPath;
                var fileName = Constants.Debug.debugFileName;

                return Path.Combine(folder, fileName);
            }
        }

        public ErrorToLogFile(string message) { Message = message; }
        public ErrorToLogFile(Exception ex) { Exception = ex; }


        public bool WriteToFile(string path = "")
        {
            if (string.IsNullOrEmpty(path))
            {
                path = DefaultPath;
            }

            try
            {
                using (var writer = new StreamWriter(path, true))
                {
                    writer.WriteLine("-----------------------------------------------------------------------------");
                    writer.WriteLine("Date : " + DateTime.Now.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine();

                    if (Exception != null)
                    {
                        writer.WriteLine(Exception.GetType().FullName);
                        writer.WriteLine("Source : " + Exception.Source);
                        writer.WriteLine("Message : " + Exception.Message);
                        writer.WriteLine("StackTrace : " + Exception.StackTrace);
                        writer.WriteLine("InnerException : " + Exception.InnerException?.Message);
                    }

                    if (!string.IsNullOrEmpty(Message))
                    {
                        writer.WriteLine(Message);
                    }

                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                new ErrorToLogFile(ex).WriteToFile();
                return false;
            }

            return true;
        }
    }
}
