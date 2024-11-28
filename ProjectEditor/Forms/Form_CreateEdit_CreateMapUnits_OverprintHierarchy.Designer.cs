namespace GSC_ProjectEditor
{
    partial class Form_CreateEdit_CreateMapUnits_OverprintHierarchy
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_CreateEdit_CreateMapUnits_OverprintHierarchy));
            this.groupBox_OPLevel = new System.Windows.Forms.GroupBox();
            this.radioButton_level5 = new System.Windows.Forms.RadioButton();
            this.radioButton_level4 = new System.Windows.Forms.RadioButton();
            this.radioButton_level3 = new System.Windows.Forms.RadioButton();
            this.radioButton_level2 = new System.Windows.Forms.RadioButton();
            this.radioButton_level1 = new System.Windows.Forms.RadioButton();
            this.groupBox_OPLevel.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox_OPLevel
            // 
            this.groupBox_OPLevel.Controls.Add(this.radioButton_level5);
            this.groupBox_OPLevel.Controls.Add(this.radioButton_level4);
            this.groupBox_OPLevel.Controls.Add(this.radioButton_level3);
            this.groupBox_OPLevel.Controls.Add(this.radioButton_level2);
            this.groupBox_OPLevel.Controls.Add(this.radioButton_level1);
            resources.ApplyResources(this.groupBox_OPLevel, "groupBox_OPLevel");
            this.groupBox_OPLevel.Name = "groupBox_OPLevel";
            this.groupBox_OPLevel.TabStop = false;
            // 
            // radioButton_level5
            // 
            resources.ApplyResources(this.radioButton_level5, "radioButton_level5");
            this.radioButton_level5.Name = "radioButton_level5";
            this.radioButton_level5.Tag = "5";
            this.radioButton_level5.UseVisualStyleBackColor = true;
            // 
            // radioButton_level4
            // 
            resources.ApplyResources(this.radioButton_level4, "radioButton_level4");
            this.radioButton_level4.Name = "radioButton_level4";
            this.radioButton_level4.Tag = "4";
            this.radioButton_level4.UseVisualStyleBackColor = true;
            // 
            // radioButton_level3
            // 
            resources.ApplyResources(this.radioButton_level3, "radioButton_level3");
            this.radioButton_level3.Name = "radioButton_level3";
            this.radioButton_level3.Tag = "3";
            this.radioButton_level3.UseVisualStyleBackColor = true;
            // 
            // radioButton_level2
            // 
            resources.ApplyResources(this.radioButton_level2, "radioButton_level2");
            this.radioButton_level2.Name = "radioButton_level2";
            this.radioButton_level2.Tag = "2";
            this.radioButton_level2.UseVisualStyleBackColor = true;
            // 
            // radioButton_level1
            // 
            resources.ApplyResources(this.radioButton_level1, "radioButton_level1");
            this.radioButton_level1.Checked = true;
            this.radioButton_level1.Name = "radioButton_level1";
            this.radioButton_level1.TabStop = true;
            this.radioButton_level1.Tag = "1";
            this.radioButton_level1.UseVisualStyleBackColor = true;
            // 
            // FormOverprintHierarchy
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox_OPLevel);
            this.Name = "Form_CreateEdit_CreateMapUnits_OverprintHierarchy";
            this.TopMost = true;
            this.groupBox_OPLevel.ResumeLayout(false);
            this.groupBox_OPLevel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox_OPLevel;
        private System.Windows.Forms.RadioButton radioButton_level5;
        private System.Windows.Forms.RadioButton radioButton_level4;
        private System.Windows.Forms.RadioButton radioButton_level3;
        private System.Windows.Forms.RadioButton radioButton_level2;
        private System.Windows.Forms.RadioButton radioButton_level1;
    }
}