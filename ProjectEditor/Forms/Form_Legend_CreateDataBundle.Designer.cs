namespace GSC_ProjectEditor
{
    partial class Form_Legend_CreateDataBundle
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Legend_CreateDataBundle));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkedListBox_Maps = new System.Windows.Forms.CheckedListBox();
            this.btn_Create = new System.Windows.Forms.Button();
            this.groupBox_Options = new System.Windows.Forms.GroupBox();
            this.label_AddToCartoElement = new System.Windows.Forms.Label();
            this.label_newProjection = new System.Windows.Forms.Label();
            this.btn_AddToCartoElement = new System.Windows.Forms.Button();
            this.txtbox_AddToCartoElement = new System.Windows.Forms.TextBox();
            this.btn_newProjection = new System.Windows.Forms.Button();
            this.txtbox_newProjection = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox_Options.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkedListBox_Maps);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // checkedListBox_Maps
            // 
            this.checkedListBox_Maps.CheckOnClick = true;
            this.checkedListBox_Maps.FormattingEnabled = true;
            resources.ApplyResources(this.checkedListBox_Maps, "checkedListBox_Maps");
            this.checkedListBox_Maps.Name = "checkedListBox_Maps";
            this.checkedListBox_Maps.TabStop = false;
            // 
            // btn_Create
            // 
            resources.ApplyResources(this.btn_Create, "btn_Create");
            this.btn_Create.Name = "btn_Create";
            this.btn_Create.TabStop = false;
            this.btn_Create.UseVisualStyleBackColor = true;
            this.btn_Create.Click += new System.EventHandler(this.btn_Create_Click);
            // 
            // groupBox_Options
            // 
            this.groupBox_Options.Controls.Add(this.label_AddToCartoElement);
            this.groupBox_Options.Controls.Add(this.label_newProjection);
            this.groupBox_Options.Controls.Add(this.btn_AddToCartoElement);
            this.groupBox_Options.Controls.Add(this.txtbox_AddToCartoElement);
            this.groupBox_Options.Controls.Add(this.btn_newProjection);
            this.groupBox_Options.Controls.Add(this.txtbox_newProjection);
            resources.ApplyResources(this.groupBox_Options, "groupBox_Options");
            this.groupBox_Options.Name = "groupBox_Options";
            this.groupBox_Options.TabStop = false;
            // 
            // label_AddToCartoElement
            // 
            resources.ApplyResources(this.label_AddToCartoElement, "label_AddToCartoElement");
            this.label_AddToCartoElement.Name = "label_AddToCartoElement";
            // 
            // label_newProjection
            // 
            resources.ApplyResources(this.label_newProjection, "label_newProjection");
            this.label_newProjection.Name = "label_newProjection";
            // 
            // btn_AddToCartoElement
            // 
            resources.ApplyResources(this.btn_AddToCartoElement, "btn_AddToCartoElement");
            this.btn_AddToCartoElement.Name = "btn_AddToCartoElement";
            this.btn_AddToCartoElement.UseVisualStyleBackColor = true;
            this.btn_AddToCartoElement.Click += new System.EventHandler(this.btn_AddToCartoElement_Click);
            // 
            // txtbox_AddToCartoElement
            // 
            resources.ApplyResources(this.txtbox_AddToCartoElement, "txtbox_AddToCartoElement");
            this.txtbox_AddToCartoElement.Name = "txtbox_AddToCartoElement";
            // 
            // btn_newProjection
            // 
            resources.ApplyResources(this.btn_newProjection, "btn_newProjection");
            this.btn_newProjection.Name = "btn_newProjection";
            this.btn_newProjection.UseVisualStyleBackColor = true;
            this.btn_newProjection.Click += new System.EventHandler(this.btn_newProjection_Click);
            // 
            // txtbox_newProjection
            // 
            resources.ApplyResources(this.txtbox_newProjection, "txtbox_newProjection");
            this.txtbox_newProjection.Name = "txtbox_newProjection";
            // 
            // FormCreatePublication
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox_Options);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btn_Create);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", global::GSC_ProjectEditor.Properties.Settings.Default, "dwEnabling", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Enabled = global::GSC_ProjectEditor.Properties.Settings.Default.dwEnabling;
            this.Name = "Form_Legend_CreateDataBundle";
            this.groupBox1.ResumeLayout(false);
            this.groupBox_Options.ResumeLayout(false);
            this.groupBox_Options.PerformLayout();
            this.ResumeLayout(false);

        }



        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckedListBox checkedListBox_Maps;
        private System.Windows.Forms.Button btn_Create;
        private System.Windows.Forms.GroupBox groupBox_Options;
        private System.Windows.Forms.Label label_newProjection;
        private System.Windows.Forms.Button btn_newProjection;
        private System.Windows.Forms.TextBox txtbox_newProjection;
        private System.Windows.Forms.Label label_AddToCartoElement;
        private System.Windows.Forms.Button btn_AddToCartoElement;
        private System.Windows.Forms.TextBox txtbox_AddToCartoElement;
    }
}