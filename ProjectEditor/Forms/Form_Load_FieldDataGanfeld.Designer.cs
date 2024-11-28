namespace GSC_ProjectEditor
{
    partial class Form_Load_FieldDataGanfeld
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Load_FieldDataGanfeld));
            this.btn_CancelAddGanfeldData = new System.Windows.Forms.Button();
            this.btn_ImportGanfeld = new System.Windows.Forms.Button();
            this.label_GanDB = new System.Windows.Forms.Label();
            this.label_GanShapes = new System.Windows.Forms.Label();
            this.btn_SelectGanShapeFolder = new System.Windows.Forms.Button();
            this.txtbox_GanDB = new System.Windows.Forms.TextBox();
            this.txtbox_GanShapeFolder = new System.Windows.Forms.TextBox();
            this.btn_SelectGanDB = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_CancelAddGanfeldData
            // 
            this.btn_CancelAddGanfeldData.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btn_CancelAddGanfeldData, "btn_CancelAddGanfeldData");
            this.btn_CancelAddGanfeldData.Name = "btn_CancelAddGanfeldData";
            this.btn_CancelAddGanfeldData.UseVisualStyleBackColor = true;
            this.btn_CancelAddGanfeldData.Click += new System.EventHandler(this.btn_CancelAddGanfeldData_Click);
            // 
            // btn_ImportGanfeld
            // 
            resources.ApplyResources(this.btn_ImportGanfeld, "btn_ImportGanfeld");
            this.btn_ImportGanfeld.Name = "btn_ImportGanfeld";
            this.btn_ImportGanfeld.UseVisualStyleBackColor = true;
            this.btn_ImportGanfeld.Click += new System.EventHandler(this.btn_ImportGanfeld_Click);
            // 
            // label_GanDB
            // 
            resources.ApplyResources(this.label_GanDB, "label_GanDB");
            this.label_GanDB.Name = "label_GanDB";
            // 
            // label_GanShapes
            // 
            resources.ApplyResources(this.label_GanShapes, "label_GanShapes");
            this.label_GanShapes.Name = "label_GanShapes";
            // 
            // btn_SelectGanShapeFolder
            // 
            resources.ApplyResources(this.btn_SelectGanShapeFolder, "btn_SelectGanShapeFolder");
            this.btn_SelectGanShapeFolder.Name = "btn_SelectGanShapeFolder";
            this.btn_SelectGanShapeFolder.UseVisualStyleBackColor = true;
            this.btn_SelectGanShapeFolder.Click += new System.EventHandler(this.btn_SelectGanShapeFolder_Click);
            // 
            // txtbox_GanDB
            // 
            resources.ApplyResources(this.txtbox_GanDB, "txtbox_GanDB");
            this.txtbox_GanDB.Name = "txtbox_GanDB";
            // 
            // txtbox_GanShapeFolder
            // 
            resources.ApplyResources(this.txtbox_GanShapeFolder, "txtbox_GanShapeFolder");
            this.txtbox_GanShapeFolder.Name = "txtbox_GanShapeFolder";
            // 
            // btn_SelectGanDB
            // 
            resources.ApplyResources(this.btn_SelectGanDB, "btn_SelectGanDB");
            this.btn_SelectGanDB.Name = "btn_SelectGanDB";
            this.btn_SelectGanDB.UseVisualStyleBackColor = true;
            this.btn_SelectGanDB.Click += new System.EventHandler(this.btn_SelectGanDB_Click);
            // 
            // FormAddGanfeldData
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btn_SelectGanDB);
            this.Controls.Add(this.btn_CancelAddGanfeldData);
            this.Controls.Add(this.btn_ImportGanfeld);
            this.Controls.Add(this.label_GanDB);
            this.Controls.Add(this.label_GanShapes);
            this.Controls.Add(this.btn_SelectGanShapeFolder);
            this.Controls.Add(this.txtbox_GanDB);
            this.Controls.Add(this.txtbox_GanShapeFolder);
            this.Name = "Form_Load_FieldDataGanfeld";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_CancelAddGanfeldData;
        private System.Windows.Forms.Button btn_ImportGanfeld;
        private System.Windows.Forms.Label label_GanDB;
        private System.Windows.Forms.Label label_GanShapes;
        private System.Windows.Forms.Button btn_SelectGanShapeFolder;
        private System.Windows.Forms.TextBox txtbox_GanDB;
        private System.Windows.Forms.TextBox txtbox_GanShapeFolder;
        private System.Windows.Forms.Button btn_SelectGanDB;
    }
}