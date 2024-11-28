namespace GSC_ProjectEditor
{
    partial class Form_Load_SourceInformation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Load_SourceInformation));
            this.groupBox_Project = new System.Windows.Forms.GroupBox();
            this.txtbox_SourceExtension = new System.Windows.Forms.TextBox();
            this.label_SourceExtended = new System.Windows.Forms.Label();
            this.txtbox_SourceAbbrName = new System.Windows.Forms.TextBox();
            this.txtbox_SourceFullName = new System.Windows.Forms.TextBox();
            this.txtbox_SourceRemarks = new System.Windows.Forms.TextBox();
            this.txtbox_SourceDOI = new System.Windows.Forms.TextBox();
            this.label_SourceRemarks = new System.Windows.Forms.Label();
            this.label_SourceDOI = new System.Windows.Forms.Label();
            this.label_SourceName = new System.Windows.Forms.Label();
            this.label_SourceLabel = new System.Windows.Forms.Label();
            this.txtBox_SourcePath = new System.Windows.Forms.TextBox();
            this.btn_SourcePrompt = new System.Windows.Forms.Button();
            this.label_SourcePath = new System.Windows.Forms.Label();
            this.btn_AddSource = new System.Windows.Forms.Button();
            this.btn_ClearSourceBoxes = new System.Windows.Forms.Button();
            this.label_SelectSource = new System.Windows.Forms.Label();
            this.cbox_selectSource = new System.Windows.Forms.ComboBox();
            this.groupBox_Project.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox_Project
            // 
            this.groupBox_Project.Controls.Add(this.txtbox_SourceExtension);
            this.groupBox_Project.Controls.Add(this.label_SourceExtended);
            this.groupBox_Project.Controls.Add(this.txtbox_SourceAbbrName);
            this.groupBox_Project.Controls.Add(this.txtbox_SourceFullName);
            this.groupBox_Project.Controls.Add(this.txtbox_SourceRemarks);
            this.groupBox_Project.Controls.Add(this.txtbox_SourceDOI);
            this.groupBox_Project.Controls.Add(this.label_SourceRemarks);
            this.groupBox_Project.Controls.Add(this.label_SourceDOI);
            this.groupBox_Project.Controls.Add(this.label_SourceName);
            this.groupBox_Project.Controls.Add(this.label_SourceLabel);
            resources.ApplyResources(this.groupBox_Project, "groupBox_Project");
            this.groupBox_Project.Name = "groupBox_Project";
            this.groupBox_Project.TabStop = false;
            // 
            // txtbox_SourceExtension
            // 
            resources.ApplyResources(this.txtbox_SourceExtension, "txtbox_SourceExtension");
            this.txtbox_SourceExtension.Name = "txtbox_SourceExtension";
            this.txtbox_SourceExtension.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.txtbox_SourceExtension_MouseDoubleClick);
            // 
            // label_SourceExtended
            // 
            resources.ApplyResources(this.label_SourceExtended, "label_SourceExtended");
            this.label_SourceExtended.Name = "label_SourceExtended";
            // 
            // txtbox_SourceAbbrName
            // 
            resources.ApplyResources(this.txtbox_SourceAbbrName, "txtbox_SourceAbbrName");
            this.txtbox_SourceAbbrName.Name = "txtbox_SourceAbbrName";
            // 
            // txtbox_SourceFullName
            // 
            resources.ApplyResources(this.txtbox_SourceFullName, "txtbox_SourceFullName");
            this.txtbox_SourceFullName.Name = "txtbox_SourceFullName";
            // 
            // txtbox_SourceRemarks
            // 
            resources.ApplyResources(this.txtbox_SourceRemarks, "txtbox_SourceRemarks");
            this.txtbox_SourceRemarks.Name = "txtbox_SourceRemarks";
            this.txtbox_SourceRemarks.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.txtbox_SourceRemarks_MouseDoubleClick);
            // 
            // txtbox_SourceDOI
            // 
            resources.ApplyResources(this.txtbox_SourceDOI, "txtbox_SourceDOI");
            this.txtbox_SourceDOI.Name = "txtbox_SourceDOI";
            // 
            // label_SourceRemarks
            // 
            resources.ApplyResources(this.label_SourceRemarks, "label_SourceRemarks");
            this.label_SourceRemarks.Name = "label_SourceRemarks";
            // 
            // label_SourceDOI
            // 
            resources.ApplyResources(this.label_SourceDOI, "label_SourceDOI");
            this.label_SourceDOI.Name = "label_SourceDOI";
            // 
            // label_SourceName
            // 
            resources.ApplyResources(this.label_SourceName, "label_SourceName");
            this.label_SourceName.Name = "label_SourceName";
            // 
            // label_SourceLabel
            // 
            resources.ApplyResources(this.label_SourceLabel, "label_SourceLabel");
            this.label_SourceLabel.Name = "label_SourceLabel";
            // 
            // txtBox_SourcePath
            // 
            resources.ApplyResources(this.txtBox_SourcePath, "txtBox_SourcePath");
            this.txtBox_SourcePath.Name = "txtBox_SourcePath";
            // 
            // btn_SourcePrompt
            // 
            resources.ApplyResources(this.btn_SourcePrompt, "btn_SourcePrompt");
            this.btn_SourcePrompt.Name = "btn_SourcePrompt";
            this.btn_SourcePrompt.TabStop = false;
            this.btn_SourcePrompt.UseVisualStyleBackColor = true;
            this.btn_SourcePrompt.Click += new System.EventHandler(this.btn_SourcePrompt_Click);
            // 
            // label_SourcePath
            // 
            resources.ApplyResources(this.label_SourcePath, "label_SourcePath");
            this.label_SourcePath.Name = "label_SourcePath";
            // 
            // btn_AddSource
            // 
            resources.ApplyResources(this.btn_AddSource, "btn_AddSource");
            this.btn_AddSource.Name = "btn_AddSource";
            this.btn_AddSource.TabStop = false;
            this.btn_AddSource.UseVisualStyleBackColor = true;
            this.btn_AddSource.Click += new System.EventHandler(this.btn_AddSource_Click);
            // 
            // btn_ClearSourceBoxes
            // 
            resources.ApplyResources(this.btn_ClearSourceBoxes, "btn_ClearSourceBoxes");
            this.btn_ClearSourceBoxes.Name = "btn_ClearSourceBoxes";
            this.btn_ClearSourceBoxes.TabStop = false;
            this.btn_ClearSourceBoxes.UseVisualStyleBackColor = true;
            this.btn_ClearSourceBoxes.Click += new System.EventHandler(this.btn_ClearSourceBoxes_Click);
            // 
            // label_SelectSource
            // 
            resources.ApplyResources(this.label_SelectSource, "label_SelectSource");
            this.label_SelectSource.Name = "label_SelectSource";
            // 
            // cbox_selectSource
            // 
            this.cbox_selectSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbox_selectSource.FormattingEnabled = true;
            resources.ApplyResources(this.cbox_selectSource, "cbox_selectSource");
            this.cbox_selectSource.Name = "cbox_selectSource";
            this.cbox_selectSource.SelectedIndexChanged += new System.EventHandler(this.cbox_selectSource_SelectedIndexChanged);
            // 
            // Form_Load_SourceInformation
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox_Project);
            this.Controls.Add(this.txtBox_SourcePath);
            this.Controls.Add(this.btn_SourcePrompt);
            this.Controls.Add(this.label_SourcePath);
            this.Controls.Add(this.btn_AddSource);
            this.Controls.Add(this.btn_ClearSourceBoxes);
            this.Controls.Add(this.label_SelectSource);
            this.Controls.Add(this.cbox_selectSource);
            this.Name = "Form_Load_SourceInformation";
            this.groupBox_Project.ResumeLayout(false);
            this.groupBox_Project.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox_Project;
        private System.Windows.Forms.TextBox txtbox_SourceExtension;
        private System.Windows.Forms.Label label_SourceExtended;
        private System.Windows.Forms.TextBox txtbox_SourceAbbrName;
        private System.Windows.Forms.TextBox txtbox_SourceFullName;
        private System.Windows.Forms.TextBox txtbox_SourceRemarks;
        private System.Windows.Forms.TextBox txtbox_SourceDOI;
        private System.Windows.Forms.Label label_SourceRemarks;
        private System.Windows.Forms.Label label_SourceDOI;
        private System.Windows.Forms.Label label_SourceName;
        private System.Windows.Forms.Label label_SourceLabel;
        private System.Windows.Forms.TextBox txtBox_SourcePath;
        private System.Windows.Forms.Button btn_SourcePrompt;
        private System.Windows.Forms.Label label_SourcePath;
        private System.Windows.Forms.Button btn_AddSource;
        private System.Windows.Forms.Button btn_ClearSourceBoxes;
        private System.Windows.Forms.Label label_SelectSource;
        private System.Windows.Forms.ComboBox cbox_selectSource;
    }
}