namespace GSC_ProjectEditor
{
    partial class Form_ProjectMetadata_Definition
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_ProjectMetadata_Definition));
            this.groupBox_Project = new System.Windows.Forms.GroupBox();
            this.txtbox_ProjectCode = new System.Windows.Forms.TextBox();
            this.label_ProjectID = new System.Windows.Forms.Label();
            this.txtbox_ProjectName = new System.Windows.Forms.TextBox();
            this.label_ProjectName = new System.Windows.Forms.Label();
            this.txtbox_ProjectAbbr = new System.Windows.Forms.TextBox();
            this.label_ProjectAbbr = new System.Windows.Forms.Label();
            this.txtbox_ProjectRemarks = new System.Windows.Forms.TextBox();
            this.timepckr_ProjectStart = new System.Windows.Forms.DateTimePicker();
            this.timepckr_ProjectEnd = new System.Windows.Forms.DateTimePicker();
            this.txtbox_ProjectWebLink = new System.Windows.Forms.TextBox();
            this.label_ProjectWebLink = new System.Windows.Forms.Label();
            this.label_ProjectRemarks = new System.Windows.Forms.Label();
            this.label_ProjectEnd = new System.Windows.Forms.Label();
            this.label_ProjectStart = new System.Windows.Forms.Label();
            this.btn_ClearProjectBoxes = new System.Windows.Forms.Button();
            this.btn_AddProject = new System.Windows.Forms.Button();
            this.groupBox_Project.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox_Project
            // 
            this.groupBox_Project.Controls.Add(this.txtbox_ProjectCode);
            this.groupBox_Project.Controls.Add(this.label_ProjectID);
            this.groupBox_Project.Controls.Add(this.txtbox_ProjectName);
            this.groupBox_Project.Controls.Add(this.label_ProjectName);
            this.groupBox_Project.Controls.Add(this.txtbox_ProjectAbbr);
            this.groupBox_Project.Controls.Add(this.label_ProjectAbbr);
            this.groupBox_Project.Controls.Add(this.txtbox_ProjectRemarks);
            this.groupBox_Project.Controls.Add(this.timepckr_ProjectStart);
            this.groupBox_Project.Controls.Add(this.timepckr_ProjectEnd);
            this.groupBox_Project.Controls.Add(this.txtbox_ProjectWebLink);
            this.groupBox_Project.Controls.Add(this.label_ProjectWebLink);
            this.groupBox_Project.Controls.Add(this.label_ProjectRemarks);
            this.groupBox_Project.Controls.Add(this.label_ProjectEnd);
            this.groupBox_Project.Controls.Add(this.label_ProjectStart);
            resources.ApplyResources(this.groupBox_Project, "groupBox_Project");
            this.groupBox_Project.Name = "groupBox_Project";
            this.groupBox_Project.TabStop = false;
            // 
            // txtbox_ProjectCode
            // 
            resources.ApplyResources(this.txtbox_ProjectCode, "txtbox_ProjectCode");
            this.txtbox_ProjectCode.Name = "txtbox_ProjectCode";
            // 
            // label_ProjectID
            // 
            resources.ApplyResources(this.label_ProjectID, "label_ProjectID");
            this.label_ProjectID.Name = "label_ProjectID";
            // 
            // txtbox_ProjectName
            // 
            resources.ApplyResources(this.txtbox_ProjectName, "txtbox_ProjectName");
            this.txtbox_ProjectName.Name = "txtbox_ProjectName";
            // 
            // label_ProjectName
            // 
            resources.ApplyResources(this.label_ProjectName, "label_ProjectName");
            this.label_ProjectName.Name = "label_ProjectName";
            // 
            // txtbox_ProjectAbbr
            // 
            resources.ApplyResources(this.txtbox_ProjectAbbr, "txtbox_ProjectAbbr");
            this.txtbox_ProjectAbbr.Name = "txtbox_ProjectAbbr";
            // 
            // label_ProjectAbbr
            // 
            resources.ApplyResources(this.label_ProjectAbbr, "label_ProjectAbbr");
            this.label_ProjectAbbr.Name = "label_ProjectAbbr";
            // 
            // txtbox_ProjectRemarks
            // 
            resources.ApplyResources(this.txtbox_ProjectRemarks, "txtbox_ProjectRemarks");
            this.txtbox_ProjectRemarks.Name = "txtbox_ProjectRemarks";
            // 
            // timepckr_ProjectStart
            // 
            this.timepckr_ProjectStart.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GSC_ProjectEditor.Properties.Settings.Default, "WorkingStartDate", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.timepckr_ProjectStart, "timepckr_ProjectStart");
            this.timepckr_ProjectStart.Name = "timepckr_ProjectStart";
            this.timepckr_ProjectStart.TabStop = false;
            this.timepckr_ProjectStart.Value = global::GSC_ProjectEditor.Properties.Settings.Default.WorkingStartDate;
            // 
            // timepckr_ProjectEnd
            // 
            this.timepckr_ProjectEnd.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GSC_ProjectEditor.Properties.Settings.Default, "WorkingEndDate", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.timepckr_ProjectEnd, "timepckr_ProjectEnd");
            this.timepckr_ProjectEnd.Name = "timepckr_ProjectEnd";
            this.timepckr_ProjectEnd.TabStop = false;
            this.timepckr_ProjectEnd.Value = global::GSC_ProjectEditor.Properties.Settings.Default.WorkingEndDate;
            // 
            // txtbox_ProjectWebLink
            // 
            resources.ApplyResources(this.txtbox_ProjectWebLink, "txtbox_ProjectWebLink");
            this.txtbox_ProjectWebLink.Name = "txtbox_ProjectWebLink";
            // 
            // label_ProjectWebLink
            // 
            resources.ApplyResources(this.label_ProjectWebLink, "label_ProjectWebLink");
            this.label_ProjectWebLink.Name = "label_ProjectWebLink";
            // 
            // label_ProjectRemarks
            // 
            resources.ApplyResources(this.label_ProjectRemarks, "label_ProjectRemarks");
            this.label_ProjectRemarks.Name = "label_ProjectRemarks";
            // 
            // label_ProjectEnd
            // 
            resources.ApplyResources(this.label_ProjectEnd, "label_ProjectEnd");
            this.label_ProjectEnd.Name = "label_ProjectEnd";
            // 
            // label_ProjectStart
            // 
            resources.ApplyResources(this.label_ProjectStart, "label_ProjectStart");
            this.label_ProjectStart.Name = "label_ProjectStart";
            // 
            // btn_ClearProjectBoxes
            // 
            resources.ApplyResources(this.btn_ClearProjectBoxes, "btn_ClearProjectBoxes");
            this.btn_ClearProjectBoxes.Name = "btn_ClearProjectBoxes";
            this.btn_ClearProjectBoxes.TabStop = false;
            this.btn_ClearProjectBoxes.UseVisualStyleBackColor = true;
            this.btn_ClearProjectBoxes.Click += new System.EventHandler(this.btn_ClearProjectBoxes_Click);
            // 
            // btn_AddProject
            // 
            resources.ApplyResources(this.btn_AddProject, "btn_AddProject");
            this.btn_AddProject.Name = "btn_AddProject";
            this.btn_AddProject.TabStop = false;
            this.btn_AddProject.UseVisualStyleBackColor = true;
            this.btn_AddProject.Click += new System.EventHandler(this.btn_AddProject_Click);
            // 
            // FormProjectDefinition
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox_Project);
            this.Controls.Add(this.btn_ClearProjectBoxes);
            this.Controls.Add(this.btn_AddProject);
            this.Name = "Form_ProjectMetadata_Definition";
            this.groupBox_Project.ResumeLayout(false);
            this.groupBox_Project.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox_Project;
        private System.Windows.Forms.TextBox txtbox_ProjectCode;
        private System.Windows.Forms.Label label_ProjectID;
        private System.Windows.Forms.TextBox txtbox_ProjectName;
        private System.Windows.Forms.Label label_ProjectName;
        private System.Windows.Forms.TextBox txtbox_ProjectAbbr;
        private System.Windows.Forms.Label label_ProjectAbbr;
        private System.Windows.Forms.TextBox txtbox_ProjectRemarks;
        private System.Windows.Forms.DateTimePicker timepckr_ProjectStart;
        private System.Windows.Forms.DateTimePicker timepckr_ProjectEnd;
        private System.Windows.Forms.TextBox txtbox_ProjectWebLink;
        private System.Windows.Forms.Label label_ProjectWebLink;
        private System.Windows.Forms.Label label_ProjectRemarks;
        private System.Windows.Forms.Label label_ProjectEnd;
        private System.Windows.Forms.Label label_ProjectStart;
        private System.Windows.Forms.Button btn_ClearProjectBoxes;
        private System.Windows.Forms.Button btn_AddProject;
    }
}