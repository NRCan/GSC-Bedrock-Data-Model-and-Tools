namespace GSC_ProjectEditor
{
    partial class Form_Legend_Import
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Legend_Import));
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_Import_Legend = new System.Windows.Forms.Button();
            this.btn_BrowseData = new System.Windows.Forms.Button();
            this.label_SelectData = new System.Windows.Forms.Label();
            this.txtbox_DataPath = new System.Windows.Forms.TextBox();
            this.combobox_legend_tables = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btn_Cancel, "btn_Cancel");
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_Import_Legend
            // 
            resources.ApplyResources(this.btn_Import_Legend, "btn_Import_Legend");
            this.btn_Import_Legend.Name = "btn_Import_Legend";
            this.btn_Import_Legend.UseVisualStyleBackColor = true;
            this.btn_Import_Legend.Click += new System.EventHandler(this.btn_ImportLegend_Click);
            // 
            // btn_BrowseData
            // 
            resources.ApplyResources(this.btn_BrowseData, "btn_BrowseData");
            this.btn_BrowseData.Name = "btn_BrowseData";
            this.btn_BrowseData.UseVisualStyleBackColor = true;
            this.btn_BrowseData.Click += new System.EventHandler(this.btn_BrowseData_Click);
            // 
            // label_SelectData
            // 
            resources.ApplyResources(this.label_SelectData, "label_SelectData");
            this.label_SelectData.Name = "label_SelectData";
            // 
            // txtbox_DataPath
            // 
            resources.ApplyResources(this.txtbox_DataPath, "txtbox_DataPath");
            this.txtbox_DataPath.Name = "txtbox_DataPath";
            // 
            // combobox_legend_tables
            // 
            this.combobox_legend_tables.FormattingEnabled = true;
            resources.ApplyResources(this.combobox_legend_tables, "combobox_legend_tables");
            this.combobox_legend_tables.Name = "combobox_legend_tables";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // Form_Legend_Import
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.combobox_legend_tables);
            this.Controls.Add(this.btn_BrowseData);
            this.Controls.Add(this.label_SelectData);
            this.Controls.Add(this.txtbox_DataPath);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_Import_Legend);
            this.Name = "Form_Legend_Import";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Button btn_Import_Legend;
        private System.Windows.Forms.Button btn_BrowseData;
        private System.Windows.Forms.Label label_SelectData;
        private System.Windows.Forms.TextBox txtbox_DataPath;
        private System.Windows.Forms.ComboBox combobox_legend_tables;
        private System.Windows.Forms.Label label1;
    }
}