using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GSC_ProjectEditor
{
    public class Messages
    {
        /// <summary>
        /// Will show a default end of process message box
        /// </summary>
        /// <returns></returns>
        public static void ShowEndOfProcess(string extraText = "")
        {

            if (extraText != string.Empty)
            {
                MessageBox.Show(Properties.Resources.Message_ToolEnds + "\n" + extraText, Properties.Resources.Message_ToolEndsTitle, MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show(Properties.Resources.Message_ToolEnds, Properties.Resources.Message_ToolEndsTitle, MessageBoxButtons.OK);
            }
            
        }

        /// <summary>
        /// Will show a default "Not Yet Implemented" message box
        /// </summary>
        /// <returns></returns>
        public static void ShowNotYetImplemented()
        {
            MessageBox.Show(Properties.Resources.Warning_NotImplemented, "Warning",MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Will show a default warning for wrong value related to geopointIDs or geolinesIDs
        /// </summary>
        /// <param name="invalidValue"></param>
        public static void ShowInvalidGeopointGeolineID(string invalidValue)
        {
            MessageBox.Show(GSC_ProjectEditor.Properties.Resources.Error_NotInScientificLanguageDictionary + ": " + invalidValue, GSC_ProjectEditor.Properties.Resources.Error_GSC_MissingSymbol, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Will show a generic error pop-up message 
        /// </summary>
        /// <param name="text"></param>
        public static void ShowGenericErrorMessage(string text)
        {
            MessageBox.Show(text, GSC_ProjectEditor.Properties.Resources.Error_GenericTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Will show a generic warning with a yes no button, return user's answer.
        /// </summary>
        /// <param name="text">The text to show inside the pop up message</param>
        /// <returns></returns>
        public static DialogResult ShowQuestionYesNo(string text)
        {
            return  MessageBox.Show(text, GSC_ProjectEditor.Properties.Resources.Warning_BasicTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        /// <summary>
        /// Will show a generic warning pop-up message.
        /// </summary>
        /// <param name="text"></param>
        public static void ShowGenericWarning(string text)
        {
            MessageBox.Show(text, GSC_ProjectEditor.Properties.Resources.Warning_BasicTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
