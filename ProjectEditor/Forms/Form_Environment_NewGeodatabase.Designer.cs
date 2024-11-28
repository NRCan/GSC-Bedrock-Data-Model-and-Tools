namespace GSC_ProjectEditor
{
    partial class Form_Environment_NewGeodatabase
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Environment_NewGeodatabase));
            this.txtbox_XML = new System.Windows.Forms.TextBox();
            this.txtbox_DBName = new System.Windows.Forms.TextBox();
            this.txtbox_Prj = new System.Windows.Forms.TextBox();
            this.btn_SelectPrj = new System.Windows.Forms.Button();
            this.btn_SelectXML = new System.Windows.Forms.Button();
            this.label_ProjectXML = new System.Windows.Forms.Label();
            this.label_GDBProjection = new System.Windows.Forms.Label();
            this.label_OutputDBName = new System.Windows.Forms.Label();
            this.btn_CreateGDB = new System.Windows.Forms.Button();
            this.btn_CancelCreateGDB = new System.Windows.Forms.Button();
            this.btn_select_DBPath = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtbox_XML
            // 
            resources.ApplyResources(this.txtbox_XML, "txtbox_XML");
            this.txtbox_XML.Name = "txtbox_XML";
            // 
            // txtbox_DBName
            // 
            resources.ApplyResources(this.txtbox_DBName, "txtbox_DBName");
            this.txtbox_DBName.Name = "txtbox_DBName";
            // 
            // txtbox_Prj
            // 
            resources.ApplyResources(this.txtbox_Prj, "txtbox_Prj");
            this.txtbox_Prj.Name = "txtbox_Prj";
            // 
            // btn_SelectPrj
            // 
            resources.ApplyResources(this.btn_SelectPrj, "btn_SelectPrj");
            this.btn_SelectPrj.Name = "btn_SelectPrj";
            this.btn_SelectPrj.UseVisualStyleBackColor = true;
            this.btn_SelectPrj.Click += new System.EventHandler(this.btn_SelectPrj_Click);
            // 
            // btn_SelectXML
            // 
            resources.ApplyResources(this.btn_SelectXML, "btn_SelectXML");
            this.btn_SelectXML.Name = "btn_SelectXML";
            this.btn_SelectXML.UseVisualStyleBackColor = true;
            this.btn_SelectXML.Click += new System.EventHandler(this.btn_SelectXML_Click);
            // 
            // label_ProjectXML
            // 
            resources.ApplyResources(this.label_ProjectXML, "label_ProjectXML");
            this.label_ProjectXML.Name = "label_ProjectXML";
            // 
            // label_GDBProjection
            // 
            resources.ApplyResources(this.label_GDBProjection, "label_GDBProjection");
            this.label_GDBProjection.Name = "label_GDBProjection";
            // 
            // label_OutputDBName
            // 
            resources.ApplyResources(this.label_OutputDBName, "label_OutputDBName");
            this.label_OutputDBName.Name = "label_OutputDBName";
            // 
            // btn_CreateGDB
            // 
            resources.ApplyResources(this.btn_CreateGDB, "btn_CreateGDB");
            this.btn_CreateGDB.Name = "btn_CreateGDB";
            this.btn_CreateGDB.UseVisualStyleBackColor = true;
            this.btn_CreateGDB.Click += new System.EventHandler(this.btn_CreateGDB_Click);
            // 
            // btn_CancelCreateGDB
            // 
            this.btn_CancelCreateGDB.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btn_CancelCreateGDB, "btn_CancelCreateGDB");
            this.btn_CancelCreateGDB.Name = "btn_CancelCreateGDB";
            this.btn_CancelCreateGDB.UseVisualStyleBackColor = true;
            this.btn_CancelCreateGDB.Click += new System.EventHandler(this.btn_CancelCreateGDB_Click);
            // 
            // btn_select_DBPath
            // 
            resources.ApplyResources(this.btn_select_DBPath, "btn_select_DBPath");
            this.btn_select_DBPath.Name = "btn_select_DBPath";
            this.btn_select_DBPath.UseVisualStyleBackColor = true;
            this.btn_select_DBPath.Click += new System.EventHandler(this.btn_select_DBPath_Click);
            // 
            // FormNewGeodatabase
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btn_CancelCreateGDB;
            this.Controls.Add(this.btn_select_DBPath);
            this.Controls.Add(this.btn_CancelCreateGDB);
            this.Controls.Add(this.btn_CreateGDB);
            this.Controls.Add(this.label_OutputDBName);
            this.Controls.Add(this.label_GDBProjection);
            this.Controls.Add(this.label_ProjectXML);
            this.Controls.Add(this.btn_SelectXML);
            this.Controls.Add(this.btn_SelectPrj);
            this.Controls.Add(this.txtbox_Prj);
            this.Controls.Add(this.txtbox_DBName);
            this.Controls.Add(this.txtbox_XML);
            this.Name = "Form_Environment_NewGeodatabase";
            this.ShowIcon = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtbox_XML;
        private System.Windows.Forms.TextBox txtbox_DBName;
        private System.Windows.Forms.TextBox txtbox_Prj;
        private System.Windows.Forms.Button btn_SelectPrj;
        private System.Windows.Forms.Button btn_SelectXML;
        private System.Windows.Forms.Label label_ProjectXML;
        private System.Windows.Forms.Label label_GDBProjection;
        private System.Windows.Forms.Label label_OutputDBName;
        private System.Windows.Forms.Button btn_CreateGDB;
        private System.Windows.Forms.Button btn_CancelCreateGDB;
        private System.Windows.Forms.Button btn_select_DBPath;
    }
}