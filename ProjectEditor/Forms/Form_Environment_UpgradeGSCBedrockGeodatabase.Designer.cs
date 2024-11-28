namespace GSC_ProjectEditor
{
    partial class Form_Environment_UpgradeGSCBedrockGeodatabase
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Environment_UpgradeGSCBedrockGeodatabase));
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_Upgrade = new System.Windows.Forms.Button();
            this.label_OutputDBName = new System.Windows.Forms.Label();
            this.label_InDB = new System.Windows.Forms.Label();
            this.btn_SelectDB = new System.Windows.Forms.Button();
            this.txtbox_DBName = new System.Windows.Forms.TextBox();
            this.txtbox_inDBPath = new System.Windows.Forms.TextBox();
            this.label_upgradedSchema = new System.Windows.Forms.Label();
            this.button_PromptXML = new System.Windows.Forms.Button();
            this.textBox_upgradedSchemaPath = new System.Windows.Forms.TextBox();
            this.btn_select_DBPath = new System.Windows.Forms.Button();
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
            // btn_Upgrade
            // 
            resources.ApplyResources(this.btn_Upgrade, "btn_Upgrade");
            this.btn_Upgrade.Name = "btn_Upgrade";
            this.btn_Upgrade.UseVisualStyleBackColor = true;
            this.btn_Upgrade.Click += new System.EventHandler(this.btn_Upgrade_Click);
            // 
            // label_OutputDBName
            // 
            resources.ApplyResources(this.label_OutputDBName, "label_OutputDBName");
            this.label_OutputDBName.Name = "label_OutputDBName";
            // 
            // label_InDB
            // 
            resources.ApplyResources(this.label_InDB, "label_InDB");
            this.label_InDB.Name = "label_InDB";
            // 
            // btn_SelectDB
            // 
            resources.ApplyResources(this.btn_SelectDB, "btn_SelectDB");
            this.btn_SelectDB.Name = "btn_SelectDB";
            this.btn_SelectDB.UseVisualStyleBackColor = true;
            this.btn_SelectDB.Click += new System.EventHandler(this.btn_SelectDB_Click);
            // 
            // txtbox_DBName
            // 
            resources.ApplyResources(this.txtbox_DBName, "txtbox_DBName");
            this.txtbox_DBName.Name = "txtbox_DBName";
            // 
            // txtbox_inDBPath
            // 
            resources.ApplyResources(this.txtbox_inDBPath, "txtbox_inDBPath");
            this.txtbox_inDBPath.Name = "txtbox_inDBPath";
            // 
            // label_upgradedSchema
            // 
            resources.ApplyResources(this.label_upgradedSchema, "label_upgradedSchema");
            this.label_upgradedSchema.Name = "label_upgradedSchema";
            // 
            // button_PromptXML
            // 
            resources.ApplyResources(this.button_PromptXML, "button_PromptXML");
            this.button_PromptXML.Name = "button_PromptXML";
            this.button_PromptXML.UseVisualStyleBackColor = true;
            this.button_PromptXML.Click += new System.EventHandler(this.button_PromptXML_Click);
            // 
            // textBox_upgradedSchemaPath
            // 
            resources.ApplyResources(this.textBox_upgradedSchemaPath, "textBox_upgradedSchemaPath");
            this.textBox_upgradedSchemaPath.Name = "textBox_upgradedSchemaPath";
            // 
            // btn_select_DBPath
            // 
            resources.ApplyResources(this.btn_select_DBPath, "btn_select_DBPath");
            this.btn_select_DBPath.Name = "btn_select_DBPath";
            this.btn_select_DBPath.UseVisualStyleBackColor = true;
            this.btn_select_DBPath.Click += new System.EventHandler(this.btn_select_DBPath_Click);
            // 
            // FormUpgradeGDBVersion
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btn_select_DBPath);
            this.Controls.Add(this.label_upgradedSchema);
            this.Controls.Add(this.button_PromptXML);
            this.Controls.Add(this.textBox_upgradedSchemaPath);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_Upgrade);
            this.Controls.Add(this.label_OutputDBName);
            this.Controls.Add(this.label_InDB);
            this.Controls.Add(this.btn_SelectDB);
            this.Controls.Add(this.txtbox_DBName);
            this.Controls.Add(this.txtbox_inDBPath);
            this.Name = "Form_Environment_UpgradeGSCBedrockGeodatabase";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Button btn_Upgrade;
        private System.Windows.Forms.Label label_OutputDBName;
        private System.Windows.Forms.Label label_InDB;
        private System.Windows.Forms.Button btn_SelectDB;
        private System.Windows.Forms.TextBox txtbox_DBName;
        private System.Windows.Forms.TextBox txtbox_inDBPath;
        private System.Windows.Forms.Label label_upgradedSchema;
        private System.Windows.Forms.Button button_PromptXML;
        private System.Windows.Forms.TextBox textBox_upgradedSchemaPath;
        private System.Windows.Forms.Button btn_select_DBPath;
    }
}