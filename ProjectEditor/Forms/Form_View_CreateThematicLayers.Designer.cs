namespace GSC_ProjectEditor
{
    partial class Form_View_CreateThematicLayers
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_View_CreateThematicLayers));
            this.checkedListBox_ThemeLayers = new System.Windows.Forms.CheckedListBox();
            this.label_selectLayer = new System.Windows.Forms.Label();
            this.button_OK = new System.Windows.Forms.Button();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // checkedListBox_ThemeLayers
            // 
            this.checkedListBox_ThemeLayers.BackColor = System.Drawing.Color.White;
            this.checkedListBox_ThemeLayers.CheckOnClick = true;
            this.checkedListBox_ThemeLayers.FormattingEnabled = true;
            this.checkedListBox_ThemeLayers.Items.AddRange(new object[] {
            resources.GetString("checkedListBox_ThemeLayers.Items"),
            resources.GetString("checkedListBox_ThemeLayers.Items1")});
            resources.ApplyResources(this.checkedListBox_ThemeLayers, "checkedListBox_ThemeLayers");
            this.checkedListBox_ThemeLayers.Name = "checkedListBox_ThemeLayers";
            // 
            // label_selectLayer
            // 
            resources.ApplyResources(this.label_selectLayer, "label_selectLayer");
            this.label_selectLayer.Name = "label_selectLayer";
            // 
            // button_OK
            // 
            resources.ApplyResources(this.button_OK, "button_OK");
            this.button_OK.Name = "button_OK";
            this.button_OK.UseVisualStyleBackColor = true;
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // button_Cancel
            // 
            resources.ApplyResources(this.button_Cancel, "button_Cancel");
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.UseVisualStyleBackColor = true;
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // FormCreateThematicLayers
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.label_selectLayer);
            this.Controls.Add(this.checkedListBox_ThemeLayers);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", global::GSC_ProjectEditor.Properties.Settings.Default, "dwEnabling", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Enabled = global::GSC_ProjectEditor.Properties.Settings.Default.dwEnabling;
            this.Name = "Form_View_CreateThematicLayers";
            this.ShowIcon = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox checkedListBox_ThemeLayers;
        private System.Windows.Forms.Label label_selectLayer;
        private System.Windows.Forms.Button button_OK;
        private System.Windows.Forms.Button button_Cancel;
    }
}