namespace GSC_ProjectEditor
{
    partial class Form_ProjectMetadata_Roles
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_ProjectMetadata_Roles));
            this.cbox_SelectMetaID = new System.Windows.Forms.ComboBox();
            this.btn_ClearBoxes = new System.Windows.Forms.Button();
            this.grp_NewParticipant = new System.Windows.Forms.GroupBox();
            this.txtbox_PartPersonOrganisation = new System.Windows.Forms.TextBox();
            this.cbox_SelectPartActivity = new System.Windows.Forms.ComboBox();
            this.cbox_SelectPartSActivity = new System.Windows.Forms.ComboBox();
            this.label_PartActivityProject = new System.Windows.Forms.Label();
            this.label_SelectPartActivity = new System.Windows.Forms.Label();
            this.label_PartPersonOrg = new System.Windows.Forms.Label();
            this.label_PartEndDate = new System.Windows.Forms.Label();
            this.btn_AddParticipant = new System.Windows.Forms.Button();
            this.timepckr_PartEndDate = new System.Windows.Forms.DateTimePicker();
            this.groupBox_ParticipantInformation = new System.Windows.Forms.GroupBox();
            this.cbox_SelectPartRole = new System.Windows.Forms.ComboBox();
            this.txtbox_PartRoleDescription = new System.Windows.Forms.TextBox();
            this.txtbox_PartGeolcode = new System.Windows.Forms.TextBox();
            this.timepckr_PartStartDate = new System.Windows.Forms.DateTimePicker();
            this.txtbox_PartRemarks = new System.Windows.Forms.TextBox();
            this.label_PartMetaID = new System.Windows.Forms.Label();
            this.label_PartRemark = new System.Windows.Forms.Label();
            this.label_PartGeolcode = new System.Windows.Forms.Label();
            this.label_PartRoleDesc = new System.Windows.Forms.Label();
            this.label_PartRole = new System.Windows.Forms.Label();
            this.label_PartStartDate = new System.Windows.Forms.Label();
            this.label_SelectParticipant = new System.Windows.Forms.Label();
            this.cbox_SelectParticipant = new System.Windows.Forms.ComboBox();
            this.button_AddWorkspace = new System.Windows.Forms.Button();
            this.grp_NewParticipant.SuspendLayout();
            this.groupBox_ParticipantInformation.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbox_SelectMetaID
            // 
            this.cbox_SelectMetaID.FormattingEnabled = true;
            resources.ApplyResources(this.cbox_SelectMetaID, "cbox_SelectMetaID");
            this.cbox_SelectMetaID.Name = "cbox_SelectMetaID";
            // 
            // btn_ClearBoxes
            // 
            resources.ApplyResources(this.btn_ClearBoxes, "btn_ClearBoxes");
            this.btn_ClearBoxes.Name = "btn_ClearBoxes";
            this.btn_ClearBoxes.TabStop = false;
            this.btn_ClearBoxes.UseVisualStyleBackColor = true;
            this.btn_ClearBoxes.Click += new System.EventHandler(this.btn_ClearBoxes_Click);
            // 
            // grp_NewParticipant
            // 
            this.grp_NewParticipant.BackColor = System.Drawing.SystemColors.Control;
            this.grp_NewParticipant.Controls.Add(this.txtbox_PartPersonOrganisation);
            this.grp_NewParticipant.Controls.Add(this.cbox_SelectPartActivity);
            this.grp_NewParticipant.Controls.Add(this.cbox_SelectPartSActivity);
            this.grp_NewParticipant.Controls.Add(this.label_PartActivityProject);
            this.grp_NewParticipant.Controls.Add(this.label_SelectPartActivity);
            this.grp_NewParticipant.Controls.Add(this.label_PartPersonOrg);
            resources.ApplyResources(this.grp_NewParticipant, "grp_NewParticipant");
            this.grp_NewParticipant.Name = "grp_NewParticipant";
            this.grp_NewParticipant.TabStop = false;
            // 
            // txtbox_PartPersonOrganisation
            // 
            this.txtbox_PartPersonOrganisation.Cursor = System.Windows.Forms.Cursors.Arrow;
            resources.ApplyResources(this.txtbox_PartPersonOrganisation, "txtbox_PartPersonOrganisation");
            this.txtbox_PartPersonOrganisation.Name = "txtbox_PartPersonOrganisation";
            this.txtbox_PartPersonOrganisation.ReadOnly = true;
            this.txtbox_PartPersonOrganisation.TabStop = false;
            // 
            // cbox_SelectPartActivity
            // 
            this.cbox_SelectPartActivity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbox_SelectPartActivity.FormattingEnabled = true;
            resources.ApplyResources(this.cbox_SelectPartActivity, "cbox_SelectPartActivity");
            this.cbox_SelectPartActivity.Name = "cbox_SelectPartActivity";
            this.cbox_SelectPartActivity.SelectedIndexChanged += new System.EventHandler(this.cbox_SelectPartActivity_SelectedIndexChanged);
            // 
            // cbox_SelectPartSActivity
            // 
            this.cbox_SelectPartSActivity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbox_SelectPartSActivity.FormattingEnabled = true;
            resources.ApplyResources(this.cbox_SelectPartSActivity, "cbox_SelectPartSActivity");
            this.cbox_SelectPartSActivity.Name = "cbox_SelectPartSActivity";
            // 
            // label_PartActivityProject
            // 
            resources.ApplyResources(this.label_PartActivityProject, "label_PartActivityProject");
            this.label_PartActivityProject.Name = "label_PartActivityProject";
            // 
            // label_SelectPartActivity
            // 
            resources.ApplyResources(this.label_SelectPartActivity, "label_SelectPartActivity");
            this.label_SelectPartActivity.Name = "label_SelectPartActivity";
            // 
            // label_PartPersonOrg
            // 
            resources.ApplyResources(this.label_PartPersonOrg, "label_PartPersonOrg");
            this.label_PartPersonOrg.Name = "label_PartPersonOrg";
            // 
            // label_PartEndDate
            // 
            resources.ApplyResources(this.label_PartEndDate, "label_PartEndDate");
            this.label_PartEndDate.Name = "label_PartEndDate";
            // 
            // btn_AddParticipant
            // 
            resources.ApplyResources(this.btn_AddParticipant, "btn_AddParticipant");
            this.btn_AddParticipant.Name = "btn_AddParticipant";
            this.btn_AddParticipant.TabStop = false;
            this.btn_AddParticipant.UseVisualStyleBackColor = true;
            this.btn_AddParticipant.Click += new System.EventHandler(this.btn_AddParticipant_Click);
            // 
            // timepckr_PartEndDate
            // 
            this.timepckr_PartEndDate.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GSC_ProjectEditor.Properties.Settings.Default, "WorkingEndDate", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.timepckr_PartEndDate, "timepckr_PartEndDate");
            this.timepckr_PartEndDate.Name = "timepckr_PartEndDate";
            this.timepckr_PartEndDate.TabStop = false;
            this.timepckr_PartEndDate.Value = global::GSC_ProjectEditor.Properties.Settings.Default.WorkingEndDate;
            // 
            // groupBox_ParticipantInformation
            // 
            this.groupBox_ParticipantInformation.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox_ParticipantInformation.Controls.Add(this.cbox_SelectMetaID);
            this.groupBox_ParticipantInformation.Controls.Add(this.label_PartEndDate);
            this.groupBox_ParticipantInformation.Controls.Add(this.timepckr_PartEndDate);
            this.groupBox_ParticipantInformation.Controls.Add(this.cbox_SelectPartRole);
            this.groupBox_ParticipantInformation.Controls.Add(this.txtbox_PartRoleDescription);
            this.groupBox_ParticipantInformation.Controls.Add(this.txtbox_PartGeolcode);
            this.groupBox_ParticipantInformation.Controls.Add(this.timepckr_PartStartDate);
            this.groupBox_ParticipantInformation.Controls.Add(this.txtbox_PartRemarks);
            this.groupBox_ParticipantInformation.Controls.Add(this.label_PartMetaID);
            this.groupBox_ParticipantInformation.Controls.Add(this.label_PartRemark);
            this.groupBox_ParticipantInformation.Controls.Add(this.label_PartGeolcode);
            this.groupBox_ParticipantInformation.Controls.Add(this.label_PartRoleDesc);
            this.groupBox_ParticipantInformation.Controls.Add(this.label_PartRole);
            this.groupBox_ParticipantInformation.Controls.Add(this.label_PartStartDate);
            resources.ApplyResources(this.groupBox_ParticipantInformation, "groupBox_ParticipantInformation");
            this.groupBox_ParticipantInformation.Name = "groupBox_ParticipantInformation";
            this.groupBox_ParticipantInformation.TabStop = false;
            // 
            // cbox_SelectPartRole
            // 
            this.cbox_SelectPartRole.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbox_SelectPartRole.FormattingEnabled = true;
            this.cbox_SelectPartRole.Items.AddRange(new object[] {
            resources.GetString("cbox_SelectPartRole.Items")});
            resources.ApplyResources(this.cbox_SelectPartRole, "cbox_SelectPartRole");
            this.cbox_SelectPartRole.Name = "cbox_SelectPartRole";
            // 
            // txtbox_PartRoleDescription
            // 
            resources.ApplyResources(this.txtbox_PartRoleDescription, "txtbox_PartRoleDescription");
            this.txtbox_PartRoleDescription.Name = "txtbox_PartRoleDescription";
            // 
            // txtbox_PartGeolcode
            // 
            resources.ApplyResources(this.txtbox_PartGeolcode, "txtbox_PartGeolcode");
            this.txtbox_PartGeolcode.Name = "txtbox_PartGeolcode";
            // 
            // timepckr_PartStartDate
            // 
            this.timepckr_PartStartDate.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GSC_ProjectEditor.Properties.Settings.Default, "WorkingStartDate", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.timepckr_PartStartDate, "timepckr_PartStartDate");
            this.timepckr_PartStartDate.Name = "timepckr_PartStartDate";
            this.timepckr_PartStartDate.TabStop = false;
            this.timepckr_PartStartDate.Value = global::GSC_ProjectEditor.Properties.Settings.Default.WorkingStartDate;
            // 
            // txtbox_PartRemarks
            // 
            resources.ApplyResources(this.txtbox_PartRemarks, "txtbox_PartRemarks");
            this.txtbox_PartRemarks.Name = "txtbox_PartRemarks";
            // 
            // label_PartMetaID
            // 
            resources.ApplyResources(this.label_PartMetaID, "label_PartMetaID");
            this.label_PartMetaID.Name = "label_PartMetaID";
            // 
            // label_PartRemark
            // 
            resources.ApplyResources(this.label_PartRemark, "label_PartRemark");
            this.label_PartRemark.Name = "label_PartRemark";
            // 
            // label_PartGeolcode
            // 
            resources.ApplyResources(this.label_PartGeolcode, "label_PartGeolcode");
            this.label_PartGeolcode.BackColor = System.Drawing.Color.Transparent;
            this.label_PartGeolcode.Name = "label_PartGeolcode";
            // 
            // label_PartRoleDesc
            // 
            resources.ApplyResources(this.label_PartRoleDesc, "label_PartRoleDesc");
            this.label_PartRoleDesc.BackColor = System.Drawing.Color.Transparent;
            this.label_PartRoleDesc.Name = "label_PartRoleDesc";
            // 
            // label_PartRole
            // 
            resources.ApplyResources(this.label_PartRole, "label_PartRole");
            this.label_PartRole.BackColor = System.Drawing.Color.Transparent;
            this.label_PartRole.Name = "label_PartRole";
            // 
            // label_PartStartDate
            // 
            resources.ApplyResources(this.label_PartStartDate, "label_PartStartDate");
            this.label_PartStartDate.Name = "label_PartStartDate";
            // 
            // label_SelectParticipant
            // 
            resources.ApplyResources(this.label_SelectParticipant, "label_SelectParticipant");
            this.label_SelectParticipant.Name = "label_SelectParticipant";
            // 
            // cbox_SelectParticipant
            // 
            this.cbox_SelectParticipant.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbox_SelectParticipant.DropDownWidth = 400;
            this.cbox_SelectParticipant.FormattingEnabled = true;
            resources.ApplyResources(this.cbox_SelectParticipant, "cbox_SelectParticipant");
            this.cbox_SelectParticipant.Name = "cbox_SelectParticipant";
            this.cbox_SelectParticipant.Sorted = true;
            this.cbox_SelectParticipant.SelectedIndexChanged += new System.EventHandler(this.cbox_SelectParticipant_SelectedIndexChanged);
            // 
            // button_AddWorkspace
            // 
            resources.ApplyResources(this.button_AddWorkspace, "button_AddWorkspace");
            this.button_AddWorkspace.Name = "button_AddWorkspace";
            this.button_AddWorkspace.UseVisualStyleBackColor = true;
            this.button_AddWorkspace.Click += new System.EventHandler(this.button_AddWorkspace_Click);
            // 
            // FormParticipantRoles
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button_AddWorkspace);
            this.Controls.Add(this.btn_ClearBoxes);
            this.Controls.Add(this.grp_NewParticipant);
            this.Controls.Add(this.btn_AddParticipant);
            this.Controls.Add(this.groupBox_ParticipantInformation);
            this.Controls.Add(this.label_SelectParticipant);
            this.Controls.Add(this.cbox_SelectParticipant);
            this.Name = "Form_ProjectMetadata_Roles";
            this.grp_NewParticipant.ResumeLayout(false);
            this.grp_NewParticipant.PerformLayout();
            this.groupBox_ParticipantInformation.ResumeLayout(false);
            this.groupBox_ParticipantInformation.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbox_SelectMetaID;
        private System.Windows.Forms.Button btn_ClearBoxes;
        private System.Windows.Forms.GroupBox grp_NewParticipant;
        private System.Windows.Forms.TextBox txtbox_PartPersonOrganisation;
        private System.Windows.Forms.ComboBox cbox_SelectPartActivity;
        private System.Windows.Forms.ComboBox cbox_SelectPartSActivity;
        private System.Windows.Forms.Label label_PartActivityProject;
        private System.Windows.Forms.Label label_SelectPartActivity;
        private System.Windows.Forms.Label label_PartPersonOrg;
        private System.Windows.Forms.Label label_PartEndDate;
        private System.Windows.Forms.Button btn_AddParticipant;
        private System.Windows.Forms.DateTimePicker timepckr_PartEndDate;
        private System.Windows.Forms.GroupBox groupBox_ParticipantInformation;
        private System.Windows.Forms.ComboBox cbox_SelectPartRole;
        private System.Windows.Forms.TextBox txtbox_PartRoleDescription;
        private System.Windows.Forms.TextBox txtbox_PartGeolcode;
        private System.Windows.Forms.DateTimePicker timepckr_PartStartDate;
        private System.Windows.Forms.TextBox txtbox_PartRemarks;
        private System.Windows.Forms.Label label_PartMetaID;
        private System.Windows.Forms.Label label_PartRemark;
        private System.Windows.Forms.Label label_PartGeolcode;
        private System.Windows.Forms.Label label_PartRoleDesc;
        private System.Windows.Forms.Label label_PartRole;
        private System.Windows.Forms.Label label_PartStartDate;
        private System.Windows.Forms.Label label_SelectParticipant;
        private System.Windows.Forms.ComboBox cbox_SelectParticipant;
        private System.Windows.Forms.Button button_AddWorkspace;
    }
}