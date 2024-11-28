namespace GSC_ProjectEditor
{
    partial class Form_Legend_CreateTempTableLegendGenerator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Legend_CreateTempTableLegendGenerator));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBox_AllMaps = new System.Windows.Forms.CheckBox();
            this.label_MapList = new System.Windows.Forms.Label();
            this.checkedListBox_Maps = new System.Windows.Forms.CheckedListBox();
            this.checkBox_FullMapUnits = new System.Windows.Forms.CheckBox();
            this.btn_Create = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBox_AllMaps);
            this.groupBox1.Controls.Add(this.label_MapList);
            this.groupBox1.Controls.Add(this.checkedListBox_Maps);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // checkBox_AllMaps
            // 
            resources.ApplyResources(this.checkBox_AllMaps, "checkBox_AllMaps");
            this.checkBox_AllMaps.Checked = true;
            this.checkBox_AllMaps.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_AllMaps.Name = "checkBox_AllMaps";
            this.checkBox_AllMaps.TabStop = false;
            this.checkBox_AllMaps.UseVisualStyleBackColor = true;
            this.checkBox_AllMaps.CheckedChanged += new System.EventHandler(this.checkBox_AllMaps_CheckedChanged);
            // 
            // label_MapList
            // 
            resources.ApplyResources(this.label_MapList, "label_MapList");
            this.label_MapList.Name = "label_MapList";
            // 
            // checkedListBox_Maps
            // 
            this.checkedListBox_Maps.CheckOnClick = true;
            this.checkedListBox_Maps.FormattingEnabled = true;
            resources.ApplyResources(this.checkedListBox_Maps, "checkedListBox_Maps");
            this.checkedListBox_Maps.Name = "checkedListBox_Maps";
            this.checkedListBox_Maps.TabStop = false;
            // 
            // checkBox_FullMapUnits
            // 
            resources.ApplyResources(this.checkBox_FullMapUnits, "checkBox_FullMapUnits");
            this.checkBox_FullMapUnits.Checked = true;
            this.checkBox_FullMapUnits.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_FullMapUnits.Name = "checkBox_FullMapUnits";
            this.checkBox_FullMapUnits.UseVisualStyleBackColor = true;
            // 
            // btn_Create
            // 
            resources.ApplyResources(this.btn_Create, "btn_Create");
            this.btn_Create.Name = "btn_Create";
            this.btn_Create.TabStop = false;
            this.btn_Create.UseVisualStyleBackColor = true;
            this.btn_Create.Click += new System.EventHandler(this.btn_Create_Click);
            // 
            // FormCreateTempLegendGenerator
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btn_Create);
            this.Controls.Add(this.checkBox_FullMapUnits);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form_Legend_CreateTempTableLegendGenerator";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBox_AllMaps;
        private System.Windows.Forms.Label label_MapList;
        private System.Windows.Forms.CheckedListBox checkedListBox_Maps;
        private System.Windows.Forms.CheckBox checkBox_FullMapUnits;
        private System.Windows.Forms.Button btn_Create;
    }
}