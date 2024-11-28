using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace GSC_ProjectEditor
{
    public class Form_Generic
    {
        /// <summary>
        /// Will be used for list box generic form
        /// </summary>
        public class ListBoxDiplayItems
        {
            public string displayItem { get; set; }
            public object displayValue { get; set; }
        }

        /// <summary>
        /// Will show user a generic form with an only textbox entry.
        /// </summary>
        /// <param name="inTitle">The title for the new form</param>
        /// <param name="inLabel">The desire text to show above the textbox</param>
        /// <param name="inIcon">An icon for the form, if needed.</param>
        /// <returns></returns>
        public static string ShowGenericTextboxForm(string inTitle, string inLabel, Icon inIcon, string defaultTextBoxText)
        {
            //Create new form
            Form_Generic_InputTextbox newGenericTextboxForm = new Form_Generic_InputTextbox();
            newGenericTextboxForm.Text = inTitle;

            //Set label
            Label titleLabel = newGenericTextboxForm.Controls.Find("label_Generic", true)[0] as Label;
            titleLabel.Text = inLabel;

            //Set Icon
            if (inIcon != null)
            {
                newGenericTextboxForm.Icon = inIcon;
            }
            else
            {
                newGenericTextboxForm.ShowIcon = false;
            }

            if (defaultTextBoxText != null || defaultTextBoxText != string.Empty)
            {
                TextBox textbox = newGenericTextboxForm.Controls.Find("textBox_Generic", true)[0] as TextBox;
                textbox.Text = defaultTextBoxText;
            }

            //Show the form
            using (newGenericTextboxForm)
            {
                var formOutput = newGenericTextboxForm.ShowDialog();
                if (formOutput == DialogResult.OK)
                {
                    return newGenericTextboxForm.returnValue;
                }
                else
                {
                    return string.Empty;
                }
            }

        }

        /// <summary>
        /// Will show user a generic pop-up form with a default list of objects that 
        /// he needs to chose from.
        /// </summary>
        /// <param name="inTitle">The title of the new form</param>
        /// <param name="inLabel">The label over the list box control</param>
        /// <param name="inIcon">The icon of the form</param>
        /// <param name="defaultObjectList">A default list of object to show user</param>
        /// <returns></returns>
        public static List<Tuple<string, object>> ShowGenericListBoxForm(string inTitle, string inLabel, Icon inIcon, List<Tuple<string, object>> defaultObjectList)
        {
            //Variables
            List<Tuple<string, object>> outputSelection = new List<Tuple<string, object>>();

            //Create new form
            Form_Generic_InputListBox newGenericListBoxForm = new Form_Generic_InputListBox();
            newGenericListBoxForm.Text = inTitle;

            //Set label
            Label titleLabel = newGenericListBoxForm.Controls.Find("label_Generic", true)[0] as Label;
            titleLabel.Text = inLabel;

            //Set Icon
            if (inIcon != null)
            {
                newGenericListBoxForm.Icon = inIcon;
            }
            else
            {
                newGenericListBoxForm.ShowIcon = false;
            }

            if (defaultObjectList != null || defaultObjectList.Count != 0)
            {
                ListBox listbox = newGenericListBoxForm.Controls.Find("listBox_Generic", true)[0] as ListBox;
                List<ListBoxDiplayItems> itemsToShow = new List<ListBoxDiplayItems>();
                foreach (Tuple<string, object> objects in defaultObjectList)
                {
                    itemsToShow.Add(new ListBoxDiplayItems { displayItem = objects.Item1, displayValue = objects.Item2});
                }
                listbox.DataSource = null;
                listbox.DataSource = itemsToShow;
                listbox.ValueMember = "displayValue";
                listbox.DisplayMember = "displayItem";
                
            }

            //Show the form
            using (newGenericListBoxForm)
            {
                var formOutput = newGenericListBoxForm.ShowDialog();
                if (formOutput == DialogResult.OK)
                {
                    ListBox.SelectedObjectCollection selectedItems = newGenericListBoxForm.returnValues;
                    foreach (Object item in selectedItems)
                    {
                        //Cast
                        ListBoxDiplayItems currentItem = item as ListBoxDiplayItems;
                        outputSelection.Add(new Tuple<string, object>(currentItem.displayItem, currentItem.displayValue));
                    }

                    return outputSelection;
                }
                else
                {
                    return outputSelection;
                }
            }
        }

        /// <summary>
        /// Will show user a generic pop-up form with a unique combobox containing objects that he needs
        /// to chose one from. Will return the combobox value
        /// </summary>
        /// <param name="inTitle">The title of the new form</param>
        /// <param name="inLabel">The label over the list box control</param>
        /// <param name="inIcon">The icon of the form</param>
        /// <param name="cboxDico">A default list of combobox items</param>
        /// <returns></returns>
        public static object ShowGenericComboboxForm(string inTitle, string inLabel, Icon inIcon, Dictionary<string, object> cboxDico)
        {
            //Variables
            Dictionary<string, object> outputSelection = new Dictionary<string, object>();

            //Create new form
            Form_Generic_InputCombobox newGenericCbox = new Form_Generic_InputCombobox();
            newGenericCbox.Text = inTitle;

            //Set label
            Label titleLabel = newGenericCbox.Controls.Find("label_Generic", true)[0] as Label;
            titleLabel.Text = inLabel;

            //Set Icon
            if (inIcon != null)
            {
                newGenericCbox.Icon = inIcon;
            }
            else
            {
                newGenericCbox.ShowIcon = false;
            }

            if (cboxDico != null || cboxDico.Count != 0)
            {
                ComboBox listbox = newGenericCbox.Controls.Find("comboBox_generic", true)[0] as ComboBox;
                List<ListBoxDiplayItems> itemsToShow = new List<ListBoxDiplayItems>();
                foreach (KeyValuePair<string, object> objects in cboxDico)
                {
                    itemsToShow.Add(new ListBoxDiplayItems { displayItem = objects.Key, displayValue = objects.Value });
                }
                listbox.DataSource = null;
                listbox.DataSource = itemsToShow;
                listbox.ValueMember = "displayValue";
                listbox.DisplayMember = "displayItem";

            }

            //Show the form
            using (newGenericCbox)
            {
                var formOutput = newGenericCbox.ShowDialog();
                if (formOutput == DialogResult.OK)
                {
                    //Cast
                    ListBoxDiplayItems selectedItem = newGenericCbox.returnValues as ListBoxDiplayItems;
                    return selectedItem.displayValue;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
