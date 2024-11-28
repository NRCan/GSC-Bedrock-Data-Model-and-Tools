namespace GSC_ProjectEditor
{
    partial class Form_ProjectMetadata_Activities
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_ProjectMetadata_Activities));
            this.checkBox_CGM = new System.Windows.Forms.CheckBox();
            this.label_MainSubActivity = new System.Windows.Forms.Label();
            this.txtbox_ActAbbr = new System.Windows.Forms.TextBox();
            this.timepckr_ActStart = new System.Windows.Forms.DateTimePicker();
            this.label_SelectAct = new System.Windows.Forms.Label();
            this.txtbox_ActName = new System.Windows.Forms.TextBox();
            this.radioButton_MainAct = new System.Windows.Forms.RadioButton();
            this.groupBox_ActicvityType = new System.Windows.Forms.GroupBox();
            this.radioButton_SubAct = new System.Windows.Forms.RadioButton();
            this.cbox_ActRelation = new System.Windows.Forms.ComboBox();
            this.label_ActDynamic = new System.Windows.Forms.Label();
            this.txtbox_ActDescription = new System.Windows.Forms.TextBox();
            this.cbox_selectAct = new System.Windows.Forms.ComboBox();
            this.timepckr_ActEnd = new System.Windows.Forms.DateTimePicker();
            this.btn_AddAct = new System.Windows.Forms.Button();
            this.cultureInfoBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox_Act = new System.Windows.Forms.GroupBox();
            this.label_ActDescription = new System.Windows.Forms.Label();
            this.label_ActEnd = new System.Windows.Forms.Label();
            this.label_ActStart = new System.Windows.Forms.Label();
            this.label_ActAbbr = new System.Windows.Forms.Label();
            this.label_ActName = new System.Windows.Forms.Label();
            this.label_ActRelation = new System.Windows.Forms.Label();
            this.btn_ClearActBoxes = new System.Windows.Forms.Button();
            this.groupBox_ActicvityType.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cultureInfoBindingSource)).BeginInit();
            this.groupBox_Act.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBox_CGM
            // 
            resources.ApplyResources(this.checkBox_CGM, "checkBox_CGM");
            this.checkBox_CGM.Name = "checkBox_CGM";
            this.checkBox_CGM.UseVisualStyleBackColor = true;
            this.checkBox_CGM.CheckedChanged += new System.EventHandler(this.checkBox_CGM_CheckedChanged);
            this.checkBox_CGM.EnabledChanged += new System.EventHandler(this.checkBox_CGM_EnabledChanged);
            // 
            // label_MainSubActivity
            // 
            resources.ApplyResources(this.label_MainSubActivity, "label_MainSubActivity");
            this.label_MainSubActivity.Name = "label_MainSubActivity";
            // 
            // txtbox_ActAbbr
            // 
            resources.ApplyResources(this.txtbox_ActAbbr, "txtbox_ActAbbr");
            this.txtbox_ActAbbr.Name = "txtbox_ActAbbr";
            // 
            // timepckr_ActStart
            // 
            this.timepckr_ActStart.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GSC_ProjectEditor.Properties.Settings.Default, "WorkingStartDate", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.timepckr_ActStart, "timepckr_ActStart");
            this.timepckr_ActStart.Name = "timepckr_ActStart";
            this.timepckr_ActStart.Value = global::GSC_ProjectEditor.Properties.Settings.Default.WorkingStartDate;
            // 
            // label_SelectAct
            // 
            resources.ApplyResources(this.label_SelectAct, "label_SelectAct");
            this.label_SelectAct.Name = "label_SelectAct";
            // 
            // txtbox_ActName
            // 
            resources.ApplyResources(this.txtbox_ActName, "txtbox_ActName");
            this.txtbox_ActName.Name = "txtbox_ActName";
            this.txtbox_ActName.TextChanged += new System.EventHandler(this.txtbox_ActName_TextChanged);
            // 
            // radioButton_MainAct
            // 
            resources.ApplyResources(this.radioButton_MainAct, "radioButton_MainAct");
            this.radioButton_MainAct.Checked = true;
            this.radioButton_MainAct.Name = "radioButton_MainAct";
            this.radioButton_MainAct.TabStop = true;
            this.radioButton_MainAct.UseVisualStyleBackColor = true;
            this.radioButton_MainAct.CheckedChanged += new System.EventHandler(this.radioButton_MainAct_CheckedChanged);
            // 
            // groupBox_ActicvityType
            // 
            this.groupBox_ActicvityType.Controls.Add(this.radioButton_MainAct);
            this.groupBox_ActicvityType.Controls.Add(this.radioButton_SubAct);
            resources.ApplyResources(this.groupBox_ActicvityType, "groupBox_ActicvityType");
            this.groupBox_ActicvityType.Name = "groupBox_ActicvityType";
            this.groupBox_ActicvityType.TabStop = false;
            // 
            // radioButton_SubAct
            // 
            resources.ApplyResources(this.radioButton_SubAct, "radioButton_SubAct");
            this.radioButton_SubAct.Name = "radioButton_SubAct";
            this.radioButton_SubAct.TabStop = true;
            this.radioButton_SubAct.UseVisualStyleBackColor = true;
            this.radioButton_SubAct.CheckedChanged += new System.EventHandler(this.radioButton_SubAct_CheckedChanged);
            // 
            // cbox_ActRelation
            // 
            this.cbox_ActRelation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbox_ActRelation.FormattingEnabled = true;
            resources.ApplyResources(this.cbox_ActRelation, "cbox_ActRelation");
            this.cbox_ActRelation.Name = "cbox_ActRelation";
            // 
            // label_ActDynamic
            // 
            resources.ApplyResources(this.label_ActDynamic, "label_ActDynamic");
            this.label_ActDynamic.Name = "label_ActDynamic";
            // 
            // txtbox_ActDescription
            // 
            resources.ApplyResources(this.txtbox_ActDescription, "txtbox_ActDescription");
            this.txtbox_ActDescription.Name = "txtbox_ActDescription";
            // 
            // cbox_selectAct
            // 
            this.cbox_selectAct.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbox_selectAct.FormattingEnabled = true;
            resources.ApplyResources(this.cbox_selectAct, "cbox_selectAct");
            this.cbox_selectAct.Name = "cbox_selectAct";
            this.cbox_selectAct.SelectedIndexChanged += new System.EventHandler(this.cbox_selectAct_SelectedIndexChanged);
            // 
            // timepckr_ActEnd
            // 
            this.timepckr_ActEnd.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::GSC_ProjectEditor.Properties.Settings.Default, "WorkingEndDate", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.timepckr_ActEnd, "timepckr_ActEnd");
            this.timepckr_ActEnd.Name = "timepckr_ActEnd";
            this.timepckr_ActEnd.Value = global::GSC_ProjectEditor.Properties.Settings.Default.WorkingEndDate;
            // 
            // btn_AddAct
            // 
            resources.ApplyResources(this.btn_AddAct, "btn_AddAct");
            this.btn_AddAct.Name = "btn_AddAct";
            this.btn_AddAct.UseVisualStyleBackColor = true;
            this.btn_AddAct.Click += new System.EventHandler(this.btn_AddAct_Click);
            // 
            // cultureInfoBindingSource
            // 
            this.cultureInfoBindingSource.AllowNew = true;
            this.cultureInfoBindingSource.DataSource = typeof(System.Globalization.CultureInfo);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // groupBox_Act
            // 
            this.groupBox_Act.Controls.Add(this.checkBox_CGM);
            this.groupBox_Act.Controls.Add(this.txtbox_ActName);
            this.groupBox_Act.Controls.Add(this.txtbox_ActAbbr);
            this.groupBox_Act.Controls.Add(this.timepckr_ActStart);
            this.groupBox_Act.Controls.Add(this.timepckr_ActEnd);
            this.groupBox_Act.Controls.Add(this.txtbox_ActDescription);
            this.groupBox_Act.Controls.Add(this.label1);
            this.groupBox_Act.Controls.Add(this.label_ActDescription);
            this.groupBox_Act.Controls.Add(this.label_ActEnd);
            this.groupBox_Act.Controls.Add(this.label_ActStart);
            this.groupBox_Act.Controls.Add(this.label_ActAbbr);
            this.groupBox_Act.Controls.Add(this.label_ActName);
            resources.ApplyResources(this.groupBox_Act, "groupBox_Act");
            this.groupBox_Act.Name = "groupBox_Act";
            this.groupBox_Act.TabStop = false;
            // 
            // label_ActDescription
            // 
            resources.ApplyResources(this.label_ActDescription, "label_ActDescription");
            this.label_ActDescription.Name = "label_ActDescription";
            // 
            // label_ActEnd
            // 
            resources.ApplyResources(this.label_ActEnd, "label_ActEnd");
            this.label_ActEnd.Name = "label_ActEnd";
            // 
            // label_ActStart
            // 
            resources.ApplyResources(this.label_ActStart, "label_ActStart");
            this.label_ActStart.Name = "label_ActStart";
            // 
            // label_ActAbbr
            // 
            resources.ApplyResources(this.label_ActAbbr, "label_ActAbbr");
            this.label_ActAbbr.Name = "label_ActAbbr";
            // 
            // label_ActName
            // 
            resources.ApplyResources(this.label_ActName, "label_ActName");
            this.label_ActName.Name = "label_ActName";
            // 
            // label_ActRelation
            // 
            resources.ApplyResources(this.label_ActRelation, "label_ActRelation");
            this.label_ActRelation.Name = "label_ActRelation";
            // 
            // btn_ClearActBoxes
            // 
            resources.ApplyResources(this.btn_ClearActBoxes, "btn_ClearActBoxes");
            this.btn_ClearActBoxes.Name = "btn_ClearActBoxes";
            this.btn_ClearActBoxes.UseVisualStyleBackColor = true;
            this.btn_ClearActBoxes.Click += new System.EventHandler(this.btn_ClearActBoxes_Click);
            // 
            // FormActivity
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label_MainSubActivity);
            this.Controls.Add(this.label_SelectAct);
            this.Controls.Add(this.groupBox_ActicvityType);
            this.Controls.Add(this.cbox_ActRelation);
            this.Controls.Add(this.label_ActDynamic);
            this.Controls.Add(this.cbox_selectAct);
            this.Controls.Add(this.btn_AddAct);
            this.Controls.Add(this.groupBox_Act);
            this.Controls.Add(this.label_ActRelation);
            this.Controls.Add(this.btn_ClearActBoxes);
            this.Name = "Form_ProjectMetadata_Activities";
            this.groupBox_ActicvityType.ResumeLayout(false);
            this.groupBox_ActicvityType.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cultureInfoBindingSource)).EndInit();
            this.groupBox_Act.ResumeLayout(false);
            this.groupBox_Act.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox_CGM;
        private System.Windows.Forms.Label label_MainSubActivity;
        private System.Windows.Forms.TextBox txtbox_ActAbbr;
        private System.Windows.Forms.DateTimePicker timepckr_ActStart;
        private System.Windows.Forms.Label label_SelectAct;
        private System.Windows.Forms.TextBox txtbox_ActName;
        private System.Windows.Forms.RadioButton radioButton_MainAct;
        private System.Windows.Forms.GroupBox groupBox_ActicvityType;
        private System.Windows.Forms.RadioButton radioButton_SubAct;
        private System.Windows.Forms.ComboBox cbox_ActRelation;
        private System.Windows.Forms.Label label_ActDynamic;
        private System.Windows.Forms.TextBox txtbox_ActDescription;
        private System.Windows.Forms.ComboBox cbox_selectAct;
        private System.Windows.Forms.DateTimePicker timepckr_ActEnd;
        private System.Windows.Forms.Button btn_AddAct;
        public System.Windows.Forms.BindingSource cultureInfoBindingSource;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox_Act;
        private System.Windows.Forms.Label label_ActDescription;
        private System.Windows.Forms.Label label_ActEnd;
        private System.Windows.Forms.Label label_ActStart;
        private System.Windows.Forms.Label label_ActAbbr;
        private System.Windows.Forms.Label label_ActName;
        private System.Windows.Forms.Label label_ActRelation;
        private System.Windows.Forms.Button btn_ClearActBoxes;
    }
}