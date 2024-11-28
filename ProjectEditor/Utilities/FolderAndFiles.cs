using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using xl = Microsoft.Office.Interop.Excel;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.GeoDatabaseUI;
using ESRI.ArcGIS.esriSystem;

namespace GSC_ProjectEditor
{
    public class FolderAndFiles
    {
        //Global variables
        public static int ColumnIterationsExcelSheet = 1;
        public static xl.Worksheet ExcelSheet;
        public static string parallelLoopSymbol = string.Empty;

        /// <summary>
        /// Will return a list of file path, from a given folder.
        /// </summary>
        /// <param name="folderPath">Folder path to list files from</param>
        /// <param name="fileExtension">A wildCard to filter the seach (ex: "*.shp")</param>
        /// <returns></returns>
        public static List<string> GetListOfFilesFromFolder(string folderPath, string wildCard)
        {
            //https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/file-system/how-to-iterate-through-a-directory-tree

            //Variables
            List<string> outputListOfFiles = new List<string>();

            Stack<string> dirs = new Stack<string>(200);

            if (!System.IO.Directory.Exists(folderPath))
            {
                throw new ArgumentException();
            }
            dirs.Push(folderPath);

            while (dirs.Count > 0)
            {
                string currentDir = dirs.Pop();
                string[] subDirs;
                try
                {
                    subDirs = System.IO.Directory.GetDirectories(currentDir);
                }
                // An UnauthorizedAccessException exception will be thrown if we do not have
                // discovery permission on a folder or file. It may or may not be acceptable 
                // to ignore the exception and continue enumerating the remaining files and 
                // folders. It is also possible (but unlikely) that a DirectoryNotFound exception 
                // will be raised. This will happen if currentDir has been deleted by
                // another application or thread after our call to Directory.Exists. The 
                // choice of which exceptions to catch depends entirely on the specific task 
                // you are intending to perform and also on how much you know with certainty 
                // about the systems on which this code will run.
                catch (UnauthorizedAccessException e)
                {
                    MessageBox.Show(e.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    continue;
                }
                catch (System.IO.DirectoryNotFoundException e)
                {
                    continue;
                }
                catch (Exception getlistOffilesFromFolderException)
                {
                    int lineNumber = GSC_ProjectEditor.Exceptions.LineNumber(getlistOffilesFromFolderException);
                    MessageBox.Show("getListofFilesFromFolder (" + lineNumber.ToString() + "):" + getlistOffilesFromFolderException.Message);
                    continue;
                }


                string[] files = null;
                try
                {
                    files = System.IO.Directory.GetFiles(currentDir,wildCard);
                }

                catch (UnauthorizedAccessException e)
                {
                    continue;
                }

                catch (System.IO.DirectoryNotFoundException e)
                {
                    continue;
                }

                // Perform the required action on each file here.
                // Modify this block to perform your required task.
                foreach (string file in files)
                {
                    try
                    {
                        outputListOfFiles.Add(file);
                    }
                    catch (System.IO.FileNotFoundException e)
                    {
                        continue;
                    }
                }

                // Push the subdirectories onto the stack for traversal.
                // This could also be done before handing the files.
                foreach (string str in subDirs)
                    dirs.Push(str);
            }

            return outputListOfFiles;

        }

        /// <summary>
        /// Will copy a input file or object into the output folder
        /// </summary>
        /// <param name="sourceFolder"></param>
        /// <param name="fileName"></param>
        /// <param name="outputFolder"></param>
        public static void CopyFiles(string sourceFolder, string fileName, string outputFolder, bool replaceAndRenameOriginal = false)
        {
            //Buidl some path with input information
            string sourcePath = System.IO.Path.Combine(sourceFolder, fileName);
            string outputPath = System.IO.Path.Combine(outputFolder, fileName);

            if (System.IO.Directory.Exists(outputFolder))
            {
                if (System.IO.File.Exists(sourcePath))
                {
                    if (replaceAndRenameOriginal)
                    {
                        if (!System.IO.File.Exists(outputPath))
                        {
                            //Validate if file exist before
                            System.IO.File.Copy(sourcePath, outputPath);
                        }
                        else
                        {
                            //Find a new name and rename existing file
                            string newCopyPath = outputPath;
                            int iteration = 0;
                            while (System.IO.File.Exists(newCopyPath))
                            {
                                string strIteration = iteration.ToString();
                                if (iteration == 0)
                                {
                                    strIteration = string.Empty;
                                }
                                if (fileName.Contains(DateTime.Today.ToShortDateString()))
                                {
                                    fileName = fileName.Split('.')[0] + "_" + strIteration + "." + fileName.Split('.')[1];
                                }
                                else
                                {
                                    fileName = fileName.Split('.')[0] + "_" + DateTime.Today.ToShortDateString() + "." + fileName.Split('.')[1];
                                }
                                
                                iteration++;
                                newCopyPath = System.IO.Path.Combine(outputFolder, fileName);
                                
                            }
                            System.IO.File.Move(outputPath, newCopyPath);
                            System.IO.File.Copy(sourcePath, outputPath);
                            MessageBox.Show(GSC_ProjectEditor.Properties.Resources.Warning_ExistingFile, GSC_ProjectEditor.Properties.Resources.Error_FileOrFolderDontExistsTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        //Validate if file exist before
                        System.IO.File.Copy(sourcePath, outputPath);
                    }


                }
                else
                {
                    string message = GSC_ProjectEditor.Properties.Resources.Error_FileOrFolderDontExists + sourcePath;
                    MessageBox.Show(message, GSC_ProjectEditor.Properties.Resources.Error_FileOrFolderDontExistsTitle, MessageBoxButtons.OK,MessageBoxIcon.Warning);
                }

            }
            else
            {
                string message = GSC_ProjectEditor.Properties.Resources.Error_FileOrFolderDontExists + outputPath;
                MessageBox.Show(message, GSC_ProjectEditor.Properties.Resources.Error_FileOrFolderDontExistsTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
            




        }

        /// <summary>
        /// Will append some text into an existing textfile
        /// </summary>
        public static void WriteToTextFile(string filePath, string textToAppend)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath, true))
            {
                file.WriteLine(textToAppend);
            }
            
        }

        /// <summary>
        /// Will return a string array coming from a textfile for all the lines inside it.
        /// </summary>
        /// <returns></returns>
        public static string[] ReadTextFile(string filePath)
        {
            return System.IO.File.ReadAllLines(filePath);
        }

        /// <summary>
        /// Will write a resource from a folder inside a given namepsace into an output folder.
        /// </summary>
        /// <param name="inFile">The file to extract and copy to, needs to reside inside project</param>
        /// <param name="inFolder">The folder inside the project containing the file to copy</param>
        /// <param name="inNameSpace">The namespace of the project that has the file and folder</param>
        /// <param name="outFolder">The output folder that will contain a copy of the file</param>
        public static void WriteResourceToFile(string inFile, string inFolder, string inNameSpace, string outFolder)
        {
            //Get the assembly object to access files
            Assembly ass = Assembly.GetCallingAssembly();  

            //Make sure output folder exists, else create it
            System.IO.Directory.CreateDirectory(outFolder);

            //Access the file with a stream
            using (Stream stream = ass.GetManifestResourceStream(inNameSpace + "." + (inFolder == "" ? "" : inFolder + ".") + inFile))
            {
                //Read the file with a reader
                using(BinaryReader binReader = new BinaryReader(stream))
                {
                    //Init the stream to write the file at the right place
                    using (System.IO.FileStream fileStream = new System.IO.FileStream(outFolder + "\\" + inFile, FileMode.OpenOrCreate))
                    {
                        //Write the file
                        using(BinaryWriter binWriter = new BinaryWriter(fileStream))
                        {
                            binWriter.Write(binReader.ReadBytes((int)stream.Length));
                            binWriter.Close();
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Will return a list of folder path, from a given folder.
        /// </summary>
        /// <param name="folderPath">The folder to search in</param>
        /// <param name="wildCard">A wildcard to refine the search</param>
        /// <returns></returns>
        public static List<string> GetListOfFoldersFromFolder(string folderPath, string wildCard)
        {
            //Variables
            List<string> outputListOfFolders = new List<string>();

            try
            {
                foreach (string files in Directory.GetDirectories(folderPath, wildCard, SearchOption.AllDirectories))
                {
                    outputListOfFolders.Add(files);
                }


            }
            catch (Exception getlistOffoldersFromFolderException)
            {
                int lineNumber = GSC_ProjectEditor.Exceptions.LineNumber(getlistOffoldersFromFolderException);
                MessageBox.Show("getlistOffoldersFromFolderException (" + lineNumber.ToString() + "):" + getlistOffoldersFromFolderException.Message);
            }

            return outputListOfFolders;
        }

        /// <summary>
        /// Will create an excel file at wanted place
        /// </summary>
        /// <param name="outputPath">The output folder path for the new excel file</param>
        /// <param name="outputName">The output name for the new excel file</param>
        /// <param name="outputExtension">The extension for the new file, it can be xls or xlsx, without the dot.</param>
        public static string CreateExcelFile(string outputPath, string outputName, string outputExtension)
        {
            return CreateExcelFileWithCustomSheet(outputPath, outputName, outputExtension, string.Empty);
        }

        /// <summary>
        /// Will create an excel file at wanted place with a new added custom sheet name
        /// </summary>
        /// <param name="outputPath">The output folder path for the new excel file</param>
        /// <param name="outputName">The output name for the new excel file</param>
        /// <param name="outputExtension">The extension for the new file, it can be xls or xlsx, without the dot.</param>
        /// <param name="newSheetName">New excel sheet name to add</param>
        public static string CreateExcelFileWithCustomSheet(string outputPath, string outputName, string outputExtension, string newSheetName)
        {
            //Return values and others
            string outputFullPath = System.IO.Path.Combine(outputPath, outputName + "." + outputExtension);
            object misValue = System.Reflection.Missing.Value;

            //Create the new excel file
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

            if (xlApp == null)
            {
                MessageBox.Show("Excel missing from computer");

            }
            //Create a new workbook
            Microsoft.Office.Interop.Excel.Workbook wb = xlApp.Workbooks.Add(misValue);

            //Create new sheet
            Microsoft.Office.Interop.Excel.Worksheet activeSheet = wb.ActiveSheet as Microsoft.Office.Interop.Excel.Worksheet;
            if (newSheetName != null && newSheetName != string.Empty)
            {
                activeSheet.Name = newSheetName;
            }

            //Save the new document
            wb.SaveAs(outputFullPath);
            wb.Close(true, misValue, misValue);
            xlApp.Quit();

            GSC_ProjectEditor.ObjectManagement.ReleaseObject(activeSheet);
            GSC_ProjectEditor.ObjectManagement.ReleaseObject(wb);
            GSC_ProjectEditor.ObjectManagement.ReleaseObject(xlApp);

            return outputFullPath;
        }

        /// <summary>
        /// From a given dictionnary, an excel workbook first sheet will be created with information.
        /// </summary>
        /// <param name="inputTableDictionary">The dictionnary to put the information in</param>
        /// <param name="inputWorkbook">The workbook to add information inside</param>
        /// <param name="inputFilePath"> The output file path for the new excel file</param>
        /// <param name="parallelLoopKey"> A string key that will be use to parallel loop the cell writing inside the excel sheet. The key must reside inside the dictionnary value string list. Will be used to split
        /// the string into two piece, one containing the cell id that the value must reside in and the other part the wanted value, ex: 2:::Lithology, 2 being the row ID and Lithology the value that we want in excel row=2.</param>
        public static void CopyESRITableToExcel(Dictionary<string, List<string>> inputTableDictionary, string inputFilePath, string parallelLoopKey)
        {
            //Variables
            int ColumnIterationsExcelSheet = 1;
            object missingValue = System.Reflection.Missing.Value;

            //Get the first sheet
            xl.Application excelApplication = new xl.Application();
            xl.Workbook inputWorkbook = excelApplication.Workbooks.Open(inputFilePath);
            ExcelSheet = inputWorkbook.Worksheets.get_Item(1) as Microsoft.Office.Interop.Excel.Worksheet;

            //Bold first row
            xl.Range range = ExcelSheet.Cells[1, 1] as xl.Range;
            range.EntireRow.Font.Bold = true;

            foreach (KeyValuePair<string, List<string>> tableKeyValues in inputTableDictionary)
            {
                //Add column field names
                ExcelSheet.Cells[1, ColumnIterationsExcelSheet] = tableKeyValues.Key;

                

                if (parallelLoopKey==string.Empty)
                {
                    int lineIterations = 2;

                    foreach (string rowValue in tableKeyValues.Value)
                    {
                        //Current iteration
                        string currentValue = rowValue.ToString();
                        if (currentValue == string.Empty)
                        {
                            currentValue = DBNull.Value.ToString();// Doesn't work... TODO
                        }

                        ExcelSheet.Cells[lineIterations, ColumnIterationsExcelSheet] = currentValue;
                        lineIterations = lineIterations + 1; //Go to next line
                    }
                }
                else
                {
                    parallelLoopSymbol = parallelLoopKey;
                    Parallel.ForEach(tableKeyValues.Value, ParallelExcelSheet);
                }


                


                ColumnIterationsExcelSheet = ColumnIterationsExcelSheet + 1; //Go to next column
            }


            inputWorkbook.Close(true, missingValue, missingValue);
            excelApplication.Quit();

            GSC_ProjectEditor.ObjectManagement.ReleaseObject(inputWorkbook);
            GSC_ProjectEditor.ObjectManagement.ReleaseObject(excelApplication);

        }

        /// <summary>
        /// Will rename a given folder path to an output folder path
        /// </summary>
        /// <param name="fromFolder">From folder path to rename</param>
        /// <param name="toFolder">To folder path to replace old</param>
        public static void RenameFolder(string fromFolder, string toFolder)
        {
            Directory.Move(fromFolder, toFolder);
        }

        /// <summary>
        /// Will execute a write to an excel sheet, from a parallel foreach loop.
        /// </summary>
        /// <param name="value"></param>
        public static void ParallelExcelSheet(string value)
        {
            //Current iteration
            string[] currentRawValue = Regex.Split(value.ToString(),parallelLoopSymbol);
            string currentValue = currentRawValue[1];
            if (currentValue == string.Empty)
            {
                currentValue = DBNull.Value.ToString();// Doesn't work... TODO
            }

            ExcelSheet.Cells[currentRawValue[0], ColumnIterationsExcelSheet] = currentValue;
        }
    }
}
