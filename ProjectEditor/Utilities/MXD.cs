using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Maplex;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Display;

namespace GSC_ProjectEditor
{
    public class MXD
    {
        /// <summary>
        /// Will create an empty mxd and return it
        /// </summary>
        public static IMapDocument CreateNewMXD(string path)
        {
            //Create the new mxd
            IMapDocument newMapDocument = new MapDocumentClass();
            newMapDocument.New(path);

            return newMapDocument;

        }

        /// <summary>
        /// Will save an MXD with default config of relative paths
        /// </summary>
        /// <param name="inMXD"></param>
        public static void SaveAnMXD(IMapDocument inMXD)
        {
            inMXD.Save(true, false);
        }


        /// <summary>
        /// Will save an MXD with default config of relative paths
        /// </summary>
        /// <param name="inMXD"></param>
        public static void SaveAsMXD(IMapDocument inMXD, string outMXDPath)
        {
            inMXD.SaveAs(outMXDPath, true, false);
        }

        /// <summary>
        /// Will create an empty mxd and return it
        /// </summary>
        public static IMapDocument OpenMXD(string path)
        {
            //Create the new mxd
            IMapDocument mapDoc = new MapDocumentClass();
            mapDoc.Open(path);
            return mapDoc;

        }

        /// <summary>
        /// THIS DOESN'T WORK UNLESS A SAVE BUTTON IS EXECUTE IN ARC MAP, BECAUSE THE ANNOTATION ENGINE NEEDS PERSISTANCE
        /// Will add the maplex extension label engine inside a given mxd document
        /// </summary>
        /// <param name="inMXD">The map document in which the new label engine will be set.</param>
        public static IMapDocument SetMaplexEngine(IMapDocument inMXD)
        {
            //Iterate through all the maps (data frames) inside document
            int mapCount = inMXD.MapCount;
            for (int i = 0; i < mapCount; i++)
            {
                //Get current map
                IMap currentMap = inMXD.Map[i];

                //Create a new maplex engine
                IAnnotateMap currentAnnoMap = new MaplexAnnotateMap();
                currentMap.AnnotationEngine = currentAnnoMap;

            }

           return inMXD;
        }

        /// <summary>
        /// Will take a given resource file path and save in the temp folder and open it as an map document
        /// </summary>
        /// <param name="resourceFile"></param>
        /// <param name="outRootPath"></param>
        /// <param name="outName"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static IMapDocument OpenMXDFromResource(string outputFolderPath)
        {
            string newMapDocumentPath = System.IO.Path.Combine(outputFolderPath, Constants.Templates.mxdCGMEmbeddedFile);

            if (!System.IO.File.Exists(newMapDocumentPath))
            {
                FolderAndFiles.WriteResourceToFile(Constants.Templates.mxdCGMEmbeddedFile, Constants.Templates.mxdEmbeddedFolder, Constants.NameSpaces.arcMap, outputFolderPath);
            }
            

            //Get new workspace
            IMapDocument newMapDocument = OpenMXD(newMapDocumentPath);

            return newMapDocument;
        }


    }
}
