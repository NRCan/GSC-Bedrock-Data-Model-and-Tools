using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
namespace GSC_ProjectEditor.Utilities
{
    public class Templates
    {

        /// <summary>
        /// Will remove a template from the edit session, sort of an update
        /// </summary>
        /// <param name="inName">The template name to remove from edit session</param>
        public static void RemoveFromTemplate(string inName)
        {
            //Variables
            IEditTemplate wantedTemplate = null;
            
            //Get the current edit session
            IEditor3 currentEditSession3 = Utilities.EditSession.GetEditor3();

            //Iterate through templates
            for (int templateIndex = 0; templateIndex < currentEditSession3.TemplateCount; templateIndex++)
            {
                //Current template
                IEditTemplate currentTemplate = currentEditSession3.get_Template(templateIndex);

                //Get current name
                if (currentTemplate.Name == inName)
                {
                    wantedTemplate = currentTemplate;
                    break;
                }
            }

            //Remove from editor
            if (wantedTemplate!=null)
            {
                currentEditSession3.RemoveTemplate(wantedTemplate);
            }
        }

    }
}
