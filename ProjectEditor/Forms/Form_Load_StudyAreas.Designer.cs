namespace GSC_ProjectEditor
{
    partial class Form_Load_StudyAreas
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Load_StudyAreas));
            this.label_AreaNorth = new System.Windows.Forms.Label();
            this.txtbox_AreaWest = new System.Windows.Forms.TextBox();
            this.label_AreaWest = new System.Windows.Forms.Label();
            this.groupBox_Area = new System.Windows.Forms.GroupBox();
            this.cbox_AreaPurpose = new System.Windows.Forms.ComboBox();
            this.cbox_AreaPurposeValue = new System.Windows.Forms.ComboBox();
            this.txtbox_AreaName = new System.Windows.Forms.TextBox();
            this.txtbox_AreaRemarks = new System.Windows.Forms.TextBox();
            this.label_AreaRemark = new System.Windows.Forms.Label();
            this.label_AreaName = new System.Windows.Forms.Label();
            this.label_AreaValue = new System.Windows.Forms.Label();
            this.label_AreaPurpose = new System.Windows.Forms.Label();
            this.groupBox_AreaCoord = new System.Windows.Forms.GroupBox();
            this.txtbox_AreaNorth = new System.Windows.Forms.TextBox();
            this.txtbox_AreaSouth = new System.Windows.Forms.TextBox();
            this.label_AreaSouth = new System.Windows.Forms.Label();
            this.label_AreaEast = new System.Windows.Forms.Label();
            this.txtbox_AreaEast = new System.Windows.Forms.TextBox();
            this.btn_AreaImport = new System.Windows.Forms.Button();
            this.txtbox_AreaImportPath = new System.Windows.Forms.TextBox();
            this.groupBox_AreaImport = new System.Windows.Forms.GroupBox();
            this.btn_ClearAreaBoxes = new System.Windows.Forms.Button();
            this.label_SelectArea = new System.Windows.Forms.Label();
            this.btn_AddArea = new System.Windows.Forms.Button();
            this.cbox_selectArea = new System.Windows.Forms.ComboBox();
            this.groupBox_Area.SuspendLayout();
            this.groupBox_AreaCoord.SuspendLayout();
            this.groupBox_AreaImport.SuspendLayout();
            this.SuspendLayout();
            // 
            // label_AreaNorth
            // 
            resources.ApplyResources(this.label_AreaNorth, "label_AreaNorth");
            this.label_AreaNorth.Name = "label_AreaNorth";
            // 
            // txtbox_AreaWest
            // 
            resources.ApplyResources(this.txtbox_AreaWest, "txtbox_AreaWest");
            this.txtbox_AreaWest.Name = "txtbox_AreaWest";
            this.txtbox_AreaWest.TextChanged += new System.EventHandler(this.txtbox_AreaWest_TextChanged);
            // 
            // label_AreaWest
            // 
            resources.ApplyResources(this.label_AreaWest, "label_AreaWest");
            this.label_AreaWest.Name = "label_AreaWest";
            // 
            // groupBox_Area
            // 
            this.groupBox_Area.Controls.Add(this.cbox_AreaPurpose);
            this.groupBox_Area.Controls.Add(this.cbox_AreaPurposeValue);
            this.groupBox_Area.Controls.Add(this.txtbox_AreaName);
            this.groupBox_Area.Controls.Add(this.txtbox_AreaRemarks);
            this.groupBox_Area.Controls.Add(this.label_AreaRemark);
            this.groupBox_Area.Controls.Add(this.label_AreaName);
            this.groupBox_Area.Controls.Add(this.label_AreaValue);
            this.groupBox_Area.Controls.Add(this.label_AreaPurpose);
            resources.ApplyResources(this.groupBox_Area, "groupBox_Area");
            this.groupBox_Area.Name = "groupBox_Area";
            this.groupBox_Area.TabStop = false;
            // 
            // cbox_AreaPurpose
            // 
            this.cbox_AreaPurpose.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbox_AreaPurpose.FormattingEnabled = true;
            this.cbox_AreaPurpose.Items.AddRange(new object[] {
            resources.GetString("cbox_AreaPurpose.Items"),
            resources.GetString("cbox_AreaPurpose.Items1"),
            resources.GetString("cbox_AreaPurpose.Items2"),
            resources.GetString("cbox_AreaPurpose.Items3")});
            resources.ApplyResources(this.cbox_AreaPurpose, "cbox_AreaPurpose");
            this.cbox_AreaPurpose.Name = "cbox_AreaPurpose";
            this.cbox_AreaPurpose.SelectedIndexChanged += new System.EventHandler(this.cbox_AreaPurpose_SelectedIndexChanged);
            // 
            // cbox_AreaPurposeValue
            // 
            this.cbox_AreaPurposeValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbox_AreaPurposeValue.FormattingEnabled = true;
            resources.ApplyResources(this.cbox_AreaPurposeValue, "cbox_AreaPurposeValue");
            this.cbox_AreaPurposeValue.Name = "cbox_AreaPurposeValue";
            // 
            // txtbox_AreaName
            // 
            resources.ApplyResources(this.txtbox_AreaName, "txtbox_AreaName");
            this.txtbox_AreaName.Name = "txtbox_AreaName";
            this.txtbox_AreaName.TextChanged += new System.EventHandler(this.txtbox_AreaName_TextChanged);
            // 
            // txtbox_AreaRemarks
            // 
            resources.ApplyResources(this.txtbox_AreaRemarks, "txtbox_AreaRemarks");
            this.txtbox_AreaRemarks.Name = "txtbox_AreaRemarks";
            // 
            // label_AreaRemark
            // 
            resources.ApplyResources(this.label_AreaRemark, "label_AreaRemark");
            this.label_AreaRemark.Name = "label_AreaRemark";
            // 
            // label_AreaName
            // 
            resources.ApplyResources(this.label_AreaName, "label_AreaName");
            this.label_AreaName.Name = "label_AreaName";
            // 
            // label_AreaValue
            // 
            resources.ApplyResources(this.label_AreaValue, "label_AreaValue");
            this.label_AreaValue.Name = "label_AreaValue";
            // 
            // label_AreaPurpose
            // 
            resources.ApplyResources(this.label_AreaPurpose, "label_AreaPurpose");
            this.label_AreaPurpose.Name = "label_AreaPurpose";
            // 
            // groupBox_AreaCoord
            // 
            this.groupBox_AreaCoord.Controls.Add(this.label_AreaNorth);
            this.groupBox_AreaCoord.Controls.Add(this.txtbox_AreaNorth);
            this.groupBox_AreaCoord.Controls.Add(this.label_AreaWest);
            this.groupBox_AreaCoord.Controls.Add(this.txtbox_AreaWest);
            this.groupBox_AreaCoord.Controls.Add(this.txtbox_AreaSouth);
            this.groupBox_AreaCoord.Controls.Add(this.label_AreaSouth);
            this.groupBox_AreaCoord.Controls.Add(this.label_AreaEast);
            this.groupBox_AreaCoord.Controls.Add(this.txtbox_AreaEast);
            resources.ApplyResources(this.groupBox_AreaCoord, "groupBox_AreaCoord");
            this.groupBox_AreaCoord.Name = "groupBox_AreaCoord";
            this.groupBox_AreaCoord.TabStop = false;
            // 
            // txtbox_AreaNorth
            // 
            resources.ApplyResources(this.txtbox_AreaNorth, "txtbox_AreaNorth");
            this.txtbox_AreaNorth.Name = "txtbox_AreaNorth";
            this.txtbox_AreaNorth.TextChanged += new System.EventHandler(this.txtbox_AreaNorth_TextChanged);
            // 
            // txtbox_AreaSouth
            // 
            resources.ApplyResources(this.txtbox_AreaSouth, "txtbox_AreaSouth");
            this.txtbox_AreaSouth.Name = "txtbox_AreaSouth";
            this.txtbox_AreaSouth.TextChanged += new System.EventHandler(this.txtbox_AreaSouth_TextChanged);
            // 
            // label_AreaSouth
            // 
            resources.ApplyResources(this.label_AreaSouth, "label_AreaSouth");
            this.label_AreaSouth.Name = "label_AreaSouth";
            // 
            // label_AreaEast
            // 
            resources.ApplyResources(this.label_AreaEast, "label_AreaEast");
            this.label_AreaEast.Name = "label_AreaEast";
            // 
            // txtbox_AreaEast
            // 
            resources.ApplyResources(this.txtbox_AreaEast, "txtbox_AreaEast");
            this.txtbox_AreaEast.Name = "txtbox_AreaEast";
            this.txtbox_AreaEast.TextChanged += new System.EventHandler(this.txtbox_AreaEast_TextChanged);
            // 
            // btn_AreaImport
            // 
            resources.ApplyResources(this.btn_AreaImport, "btn_AreaImport");
            this.btn_AreaImport.Name = "btn_AreaImport";
            this.btn_AreaImport.TabStop = false;
            this.btn_AreaImport.UseVisualStyleBackColor = true;
            this.btn_AreaImport.Click += new System.EventHandler(this.btn_AreaImport_Click);
            // 
            // txtbox_AreaImportPath
            // 
            resources.ApplyResources(this.txtbox_AreaImportPath, "txtbox_AreaImportPath");
            this.txtbox_AreaImportPath.Name = "txtbox_AreaImportPath";
            // 
            // groupBox_AreaImport
            // 
            this.groupBox_AreaImport.Controls.Add(this.btn_AreaImport);
            this.groupBox_AreaImport.Controls.Add(this.txtbox_AreaImportPath);
            resources.ApplyResources(this.groupBox_AreaImport, "groupBox_AreaImport");
            this.groupBox_AreaImport.Name = "groupBox_AreaImport";
            this.groupBox_AreaImport.TabStop = false;
            // 
            // btn_ClearAreaBoxes
            // 
            resources.ApplyResources(this.btn_ClearAreaBoxes, "btn_ClearAreaBoxes");
            this.btn_ClearAreaBoxes.Name = "btn_ClearAreaBoxes";
            this.btn_ClearAreaBoxes.TabStop = false;
            this.btn_ClearAreaBoxes.UseVisualStyleBackColor = true;
            this.btn_ClearAreaBoxes.Click += new System.EventHandler(this.btn_ClearAreaBoxes_Click);
            // 
            // label_SelectArea
            // 
            resources.ApplyResources(this.label_SelectArea, "label_SelectArea");
            this.label_SelectArea.Name = "label_SelectArea";
            // 
            // btn_AddArea
            // 
            resources.ApplyResources(this.btn_AddArea, "btn_AddArea");
            this.btn_AddArea.Name = "btn_AddArea";
            this.btn_AddArea.TabStop = false;
            this.btn_AddArea.UseVisualStyleBackColor = true;
            this.btn_AddArea.Click += new System.EventHandler(this.btn_AddArea_Click);
            // 
            // cbox_selectArea
            // 
            this.cbox_selectArea.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbox_selectArea.FormattingEnabled = true;
            resources.ApplyResources(this.cbox_selectArea, "cbox_selectArea");
            this.cbox_selectArea.Name = "cbox_selectArea";
            this.cbox_selectArea.SelectedIndexChanged += new System.EventHandler(this.cbox_selectArea_SelectedIndexChanged);
            // 
            // FormStudyAreas
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox_Area);
            this.Controls.Add(this.groupBox_AreaCoord);
            this.Controls.Add(this.groupBox_AreaImport);
            this.Controls.Add(this.btn_ClearAreaBoxes);
            this.Controls.Add(this.label_SelectArea);
            this.Controls.Add(this.btn_AddArea);
            this.Controls.Add(this.cbox_selectArea);
            this.Name = "Form_Load_StudyAreas";
            this.groupBox_Area.ResumeLayout(false);
            this.groupBox_Area.PerformLayout();
            this.groupBox_AreaCoord.ResumeLayout(false);
            this.groupBox_AreaCoord.PerformLayout();
            this.groupBox_AreaImport.ResumeLayout(false);
            this.groupBox_AreaImport.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_AreaNorth;
        private System.Windows.Forms.TextBox txtbox_AreaWest;
        private System.Windows.Forms.Label label_AreaWest;
        private System.Windows.Forms.GroupBox groupBox_Area;
        private System.Windows.Forms.ComboBox cbox_AreaPurpose;
        private System.Windows.Forms.ComboBox cbox_AreaPurposeValue;
        private System.Windows.Forms.TextBox txtbox_AreaName;
        private System.Windows.Forms.TextBox txtbox_AreaRemarks;
        private System.Windows.Forms.Label label_AreaRemark;
        private System.Windows.Forms.Label label_AreaName;
        private System.Windows.Forms.Label label_AreaValue;
        private System.Windows.Forms.Label label_AreaPurpose;
        private System.Windows.Forms.GroupBox groupBox_AreaCoord;
        private System.Windows.Forms.TextBox txtbox_AreaNorth;
        private System.Windows.Forms.TextBox txtbox_AreaSouth;
        private System.Windows.Forms.Label label_AreaSouth;
        private System.Windows.Forms.Label label_AreaEast;
        private System.Windows.Forms.TextBox txtbox_AreaEast;
        private System.Windows.Forms.Button btn_AreaImport;
        private System.Windows.Forms.TextBox txtbox_AreaImportPath;
        private System.Windows.Forms.GroupBox groupBox_AreaImport;
        private System.Windows.Forms.Button btn_ClearAreaBoxes;
        private System.Windows.Forms.Label label_SelectArea;
        private System.Windows.Forms.Button btn_AddArea;
        private System.Windows.Forms.ComboBox cbox_selectArea;
    }
}