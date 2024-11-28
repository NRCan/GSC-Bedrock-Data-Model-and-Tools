namespace GSC_ProjectEditor
{
    partial class Form_CreateEdit_GeologicalEvents
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_CreateEdit_GeologicalEvents));
            this.button_AddEvent = new System.Windows.Forms.Button();
            this.label_EventName = new System.Windows.Forms.Label();
            this.comboBox_Event = new System.Windows.Forms.ComboBox();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.button_AddModify = new System.Windows.Forms.Button();
            this.button_AddItem = new System.Windows.Forms.Button();
            this.label_linkedItem = new System.Windows.Forms.Label();
            this.comboBox_linkedItem = new System.Windows.Forms.ComboBox();
            this.label_PrefixMin = new System.Windows.Forms.Label();
            this.comboBox_PrefixMin = new System.Windows.Forms.ComboBox();
            this.comboBox_TimescaleMin = new System.Windows.Forms.ComboBox();
            this.label_TimescaleMin = new System.Windows.Forms.Label();
            this.groupBox_MinAge = new System.Windows.Forms.GroupBox();
            this.numericUpDown_CertaintyMin = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown_ValueMin = new System.Windows.Forms.NumericUpDown();
            this.label_CertaintyMin = new System.Windows.Forms.Label();
            this.label_ValueMin = new System.Windows.Forms.Label();
            this.groupBox_MaxAge = new System.Windows.Forms.GroupBox();
            this.numericUpDown_CertaintyMax = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown_ValueMax = new System.Windows.Forms.NumericUpDown();
            this.label_CertaintyMax = new System.Windows.Forms.Label();
            this.label_ValueMax = new System.Windows.Forms.Label();
            this.label_TimescaleMax = new System.Windows.Forms.Label();
            this.label_PrefixMax = new System.Windows.Forms.Label();
            this.comboBox_PrefixMax = new System.Windows.Forms.ComboBox();
            this.comboBox_TimescaleMax = new System.Windows.Forms.ComboBox();
            this.button_RenameEvent = new System.Windows.Forms.Button();
            this.button_OnScreenItemSelection = new System.Windows.Forms.Button();
            this.label_Source = new System.Windows.Forms.Label();
            this.comboBox_Source = new System.Windows.Forms.ComboBox();
            this.groupBox_MinAge.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_CertaintyMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_ValueMin)).BeginInit();
            this.groupBox_MaxAge.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_CertaintyMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_ValueMax)).BeginInit();
            this.SuspendLayout();
            // 
            // button_AddEvent
            // 
            resources.ApplyResources(this.button_AddEvent, "button_AddEvent");
            this.button_AddEvent.Name = "button_AddEvent";
            this.button_AddEvent.UseVisualStyleBackColor = true;
            this.button_AddEvent.Click += new System.EventHandler(this.button_AddEvent_Click);
            // 
            // label_EventName
            // 
            resources.ApplyResources(this.label_EventName, "label_EventName");
            this.label_EventName.Name = "label_EventName";
            // 
            // comboBox_Event
            // 
            this.comboBox_Event.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Event.FormattingEnabled = true;
            resources.ApplyResources(this.comboBox_Event, "comboBox_Event");
            this.comboBox_Event.Name = "comboBox_Event";
            this.comboBox_Event.SelectedIndexChanged += new System.EventHandler(this.comboBox_Event_SelectedIndexChanged);
            // 
            // button_Cancel
            // 
            resources.ApplyResources(this.button_Cancel, "button_Cancel");
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.UseVisualStyleBackColor = true;
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // button_AddModify
            // 
            resources.ApplyResources(this.button_AddModify, "button_AddModify");
            this.button_AddModify.Name = "button_AddModify";
            this.button_AddModify.UseVisualStyleBackColor = true;
            this.button_AddModify.Click += new System.EventHandler(this.button_AddModify_Click);
            // 
            // button_AddItem
            // 
            resources.ApplyResources(this.button_AddItem, "button_AddItem");
            this.button_AddItem.Name = "button_AddItem";
            this.button_AddItem.UseVisualStyleBackColor = true;
            this.button_AddItem.Click += new System.EventHandler(this.button_AddItem_Click);
            // 
            // label_linkedItem
            // 
            resources.ApplyResources(this.label_linkedItem, "label_linkedItem");
            this.label_linkedItem.Name = "label_linkedItem";
            // 
            // comboBox_linkedItem
            // 
            this.comboBox_linkedItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_linkedItem.FormattingEnabled = true;
            resources.ApplyResources(this.comboBox_linkedItem, "comboBox_linkedItem");
            this.comboBox_linkedItem.Name = "comboBox_linkedItem";
            // 
            // label_PrefixMin
            // 
            resources.ApplyResources(this.label_PrefixMin, "label_PrefixMin");
            this.label_PrefixMin.Name = "label_PrefixMin";
            // 
            // comboBox_PrefixMin
            // 
            this.comboBox_PrefixMin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_PrefixMin.FormattingEnabled = true;
            resources.ApplyResources(this.comboBox_PrefixMin, "comboBox_PrefixMin");
            this.comboBox_PrefixMin.Name = "comboBox_PrefixMin";
            // 
            // comboBox_TimescaleMin
            // 
            this.comboBox_TimescaleMin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_TimescaleMin.FormattingEnabled = true;
            resources.ApplyResources(this.comboBox_TimescaleMin, "comboBox_TimescaleMin");
            this.comboBox_TimescaleMin.Name = "comboBox_TimescaleMin";
            // 
            // label_TimescaleMin
            // 
            resources.ApplyResources(this.label_TimescaleMin, "label_TimescaleMin");
            this.label_TimescaleMin.Name = "label_TimescaleMin";
            // 
            // groupBox_MinAge
            // 
            this.groupBox_MinAge.Controls.Add(this.numericUpDown_CertaintyMin);
            this.groupBox_MinAge.Controls.Add(this.numericUpDown_ValueMin);
            this.groupBox_MinAge.Controls.Add(this.label_CertaintyMin);
            this.groupBox_MinAge.Controls.Add(this.label_ValueMin);
            this.groupBox_MinAge.Controls.Add(this.label_TimescaleMin);
            this.groupBox_MinAge.Controls.Add(this.label_PrefixMin);
            this.groupBox_MinAge.Controls.Add(this.comboBox_PrefixMin);
            this.groupBox_MinAge.Controls.Add(this.comboBox_TimescaleMin);
            resources.ApplyResources(this.groupBox_MinAge, "groupBox_MinAge");
            this.groupBox_MinAge.Name = "groupBox_MinAge";
            this.groupBox_MinAge.TabStop = false;
            // 
            // numericUpDown_CertaintyMin
            // 
            resources.ApplyResources(this.numericUpDown_CertaintyMin, "numericUpDown_CertaintyMin");
            this.numericUpDown_CertaintyMin.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDown_CertaintyMin.Name = "numericUpDown_CertaintyMin";
            // 
            // numericUpDown_ValueMin
            // 
            this.numericUpDown_ValueMin.DecimalPlaces = 2;
            resources.ApplyResources(this.numericUpDown_ValueMin, "numericUpDown_ValueMin");
            this.numericUpDown_ValueMin.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numericUpDown_ValueMin.Name = "numericUpDown_ValueMin";
            // 
            // label_CertaintyMin
            // 
            resources.ApplyResources(this.label_CertaintyMin, "label_CertaintyMin");
            this.label_CertaintyMin.Name = "label_CertaintyMin";
            // 
            // label_ValueMin
            // 
            resources.ApplyResources(this.label_ValueMin, "label_ValueMin");
            this.label_ValueMin.Name = "label_ValueMin";
            // 
            // groupBox_MaxAge
            // 
            this.groupBox_MaxAge.Controls.Add(this.numericUpDown_CertaintyMax);
            this.groupBox_MaxAge.Controls.Add(this.numericUpDown_ValueMax);
            this.groupBox_MaxAge.Controls.Add(this.label_CertaintyMax);
            this.groupBox_MaxAge.Controls.Add(this.label_ValueMax);
            this.groupBox_MaxAge.Controls.Add(this.label_TimescaleMax);
            this.groupBox_MaxAge.Controls.Add(this.label_PrefixMax);
            this.groupBox_MaxAge.Controls.Add(this.comboBox_PrefixMax);
            this.groupBox_MaxAge.Controls.Add(this.comboBox_TimescaleMax);
            resources.ApplyResources(this.groupBox_MaxAge, "groupBox_MaxAge");
            this.groupBox_MaxAge.Name = "groupBox_MaxAge";
            this.groupBox_MaxAge.TabStop = false;
            // 
            // numericUpDown_CertaintyMax
            // 
            resources.ApplyResources(this.numericUpDown_CertaintyMax, "numericUpDown_CertaintyMax");
            this.numericUpDown_CertaintyMax.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDown_CertaintyMax.Name = "numericUpDown_CertaintyMax";
            // 
            // numericUpDown_ValueMax
            // 
            this.numericUpDown_ValueMax.DecimalPlaces = 2;
            resources.ApplyResources(this.numericUpDown_ValueMax, "numericUpDown_ValueMax");
            this.numericUpDown_ValueMax.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numericUpDown_ValueMax.Name = "numericUpDown_ValueMax";
            // 
            // label_CertaintyMax
            // 
            resources.ApplyResources(this.label_CertaintyMax, "label_CertaintyMax");
            this.label_CertaintyMax.Name = "label_CertaintyMax";
            // 
            // label_ValueMax
            // 
            resources.ApplyResources(this.label_ValueMax, "label_ValueMax");
            this.label_ValueMax.Name = "label_ValueMax";
            // 
            // label_TimescaleMax
            // 
            resources.ApplyResources(this.label_TimescaleMax, "label_TimescaleMax");
            this.label_TimescaleMax.Name = "label_TimescaleMax";
            // 
            // label_PrefixMax
            // 
            resources.ApplyResources(this.label_PrefixMax, "label_PrefixMax");
            this.label_PrefixMax.Name = "label_PrefixMax";
            // 
            // comboBox_PrefixMax
            // 
            this.comboBox_PrefixMax.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_PrefixMax.FormattingEnabled = true;
            resources.ApplyResources(this.comboBox_PrefixMax, "comboBox_PrefixMax");
            this.comboBox_PrefixMax.Name = "comboBox_PrefixMax";
            // 
            // comboBox_TimescaleMax
            // 
            this.comboBox_TimescaleMax.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_TimescaleMax.FormattingEnabled = true;
            resources.ApplyResources(this.comboBox_TimescaleMax, "comboBox_TimescaleMax");
            this.comboBox_TimescaleMax.Name = "comboBox_TimescaleMax";
            // 
            // button_RenameEvent
            // 
            resources.ApplyResources(this.button_RenameEvent, "button_RenameEvent");
            this.button_RenameEvent.Name = "button_RenameEvent";
            this.button_RenameEvent.UseVisualStyleBackColor = true;
            this.button_RenameEvent.Click += new System.EventHandler(this.button_RenameEvent_Click);
            // 
            // button_OnScreenItemSelection
            // 
            resources.ApplyResources(this.button_OnScreenItemSelection, "button_OnScreenItemSelection");
            this.button_OnScreenItemSelection.Name = "button_OnScreenItemSelection";
            this.button_OnScreenItemSelection.UseVisualStyleBackColor = true;
            this.button_OnScreenItemSelection.Click += new System.EventHandler(this.button_OnScreenItemSelection_Click);
            // 
            // label_Source
            // 
            resources.ApplyResources(this.label_Source, "label_Source");
            this.label_Source.Name = "label_Source";
            // 
            // comboBox_Source
            // 
            this.comboBox_Source.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Source.FormattingEnabled = true;
            resources.ApplyResources(this.comboBox_Source, "comboBox_Source");
            this.comboBox_Source.Name = "comboBox_Source";
            // 
            // FormAddGeoEvent
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label_Source);
            this.Controls.Add(this.comboBox_Source);
            this.Controls.Add(this.button_OnScreenItemSelection);
            this.Controls.Add(this.button_RenameEvent);
            this.Controls.Add(this.groupBox_MaxAge);
            this.Controls.Add(this.groupBox_MinAge);
            this.Controls.Add(this.button_AddItem);
            this.Controls.Add(this.label_linkedItem);
            this.Controls.Add(this.comboBox_linkedItem);
            this.Controls.Add(this.button_AddEvent);
            this.Controls.Add(this.label_EventName);
            this.Controls.Add(this.comboBox_Event);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.button_AddModify);
            this.Name = "Form_CreateEdit_GeologicalEvents";
            this.groupBox_MinAge.ResumeLayout(false);
            this.groupBox_MinAge.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_CertaintyMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_ValueMin)).EndInit();
            this.groupBox_MaxAge.ResumeLayout(false);
            this.groupBox_MaxAge.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_CertaintyMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_ValueMax)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }



        #endregion

        private System.Windows.Forms.Button button_AddEvent;
        private System.Windows.Forms.Label label_EventName;
        private System.Windows.Forms.ComboBox comboBox_Event;
        private System.Windows.Forms.Button button_Cancel;
        private System.Windows.Forms.Button button_AddModify;
        private System.Windows.Forms.Button button_AddItem;
        private System.Windows.Forms.Label label_linkedItem;
        private System.Windows.Forms.ComboBox comboBox_linkedItem;
        private System.Windows.Forms.Label label_PrefixMin;
        private System.Windows.Forms.ComboBox comboBox_PrefixMin;
        private System.Windows.Forms.ComboBox comboBox_TimescaleMin;
        private System.Windows.Forms.Label label_TimescaleMin;
        private System.Windows.Forms.GroupBox groupBox_MinAge;
        private System.Windows.Forms.Label label_CertaintyMin;
        private System.Windows.Forms.Label label_ValueMin;
        private System.Windows.Forms.GroupBox groupBox_MaxAge;
        private System.Windows.Forms.Label label_CertaintyMax;
        private System.Windows.Forms.Label label_ValueMax;
        private System.Windows.Forms.Label label_TimescaleMax;
        private System.Windows.Forms.Label label_PrefixMax;
        private System.Windows.Forms.ComboBox comboBox_PrefixMax;
        private System.Windows.Forms.ComboBox comboBox_TimescaleMax;
        private System.Windows.Forms.Button button_RenameEvent;
        private System.Windows.Forms.Button button_OnScreenItemSelection;
        private System.Windows.Forms.NumericUpDown numericUpDown_CertaintyMin;
        private System.Windows.Forms.NumericUpDown numericUpDown_ValueMin;
        private System.Windows.Forms.NumericUpDown numericUpDown_CertaintyMax;
        private System.Windows.Forms.NumericUpDown numericUpDown_ValueMax;
        private System.Windows.Forms.Label label_Source;
        private System.Windows.Forms.ComboBox comboBox_Source;
    }
}