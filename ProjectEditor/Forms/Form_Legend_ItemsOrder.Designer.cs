namespace GSC_ProjectEditor
{
    partial class Form_Legend_ItemsOrder
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Legend_ItemsOrder));
            this.listBox_LegendItems = new System.Windows.Forms.ListBox();
            this.panel_hierarchy = new System.Windows.Forms.Panel();
            this.button_Top = new System.Windows.Forms.Button();
            this.button_Down = new System.Windows.Forms.Button();
            this.button_Bottom = new System.Windows.Forms.Button();
            this.button_Up = new System.Windows.Forms.Button();
            this.txtbox_Header = new System.Windows.Forms.TextBox();
            this.button_AddHeaderToList = new System.Windows.Forms.Button();
            this.label_SymType = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button_rightIndentation = new System.Windows.Forms.Button();
            this.button_leftIndentation = new System.Windows.Forms.Button();
            this.btn_ModifyItem = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label_HeaderType = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.radioButton_H1 = new System.Windows.Forms.RadioButton();
            this.radioButton_H2 = new System.Windows.Forms.RadioButton();
            this.radioButton_H3 = new System.Windows.Forms.RadioButton();
            this.toolTip_listBoxColors = new System.Windows.Forms.ToolTip(this.components);
            this.panel_hierarchy.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBox_LegendItems
            // 
            this.listBox_LegendItems.AllowDrop = true;
            resources.ApplyResources(this.listBox_LegendItems, "listBox_LegendItems");
            this.listBox_LegendItems.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.listBox_LegendItems.FormattingEnabled = true;
            this.listBox_LegendItems.Name = "listBox_LegendItems";
            this.listBox_LegendItems.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.toolTip_listBoxColors.SetToolTip(this.listBox_LegendItems, resources.GetString("listBox_LegendItems.ToolTip"));
            this.listBox_LegendItems.SelectedIndexChanged += new System.EventHandler(this.listBox_LegendItems_SelectedIndexChanged);
            // 
            // panel_hierarchy
            // 
            resources.ApplyResources(this.panel_hierarchy, "panel_hierarchy");
            this.panel_hierarchy.Controls.Add(this.button_Top);
            this.panel_hierarchy.Controls.Add(this.button_Down);
            this.panel_hierarchy.Controls.Add(this.button_Bottom);
            this.panel_hierarchy.Controls.Add(this.button_Up);
            this.panel_hierarchy.Name = "panel_hierarchy";
            // 
            // button_Top
            // 
            this.button_Top.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.button_Top, "button_Top");
            this.button_Top.Name = "button_Top";
            this.button_Top.UseVisualStyleBackColor = true;
            this.button_Top.Click += new System.EventHandler(this.button_Top_Click);
            // 
            // button_Down
            // 
            this.button_Down.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.button_Down, "button_Down");
            this.button_Down.Name = "button_Down";
            this.button_Down.UseVisualStyleBackColor = true;
            this.button_Down.Click += new System.EventHandler(this.button_Down_Click);
            // 
            // button_Bottom
            // 
            this.button_Bottom.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.button_Bottom, "button_Bottom");
            this.button_Bottom.Name = "button_Bottom";
            this.button_Bottom.UseVisualStyleBackColor = true;
            this.button_Bottom.Click += new System.EventHandler(this.button_Bottom_Click);
            // 
            // button_Up
            // 
            this.button_Up.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.button_Up, "button_Up");
            this.button_Up.Name = "button_Up";
            this.button_Up.UseVisualStyleBackColor = true;
            this.button_Up.Click += new System.EventHandler(this.button_Up_Click);
            // 
            // txtbox_Header
            // 
            resources.ApplyResources(this.txtbox_Header, "txtbox_Header");
            this.txtbox_Header.Name = "txtbox_Header";
            // 
            // button_AddHeaderToList
            // 
            resources.ApplyResources(this.button_AddHeaderToList, "button_AddHeaderToList");
            this.button_AddHeaderToList.Name = "button_AddHeaderToList";
            this.button_AddHeaderToList.TabStop = false;
            this.button_AddHeaderToList.UseVisualStyleBackColor = true;
            this.button_AddHeaderToList.Click += new System.EventHandler(this.button_AddHeaderToList_Click);
            // 
            // label_SymType
            // 
            resources.ApplyResources(this.label_SymType, "label_SymType");
            this.label_SymType.Name = "label_SymType";
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Controls.Add(this.button_rightIndentation);
            this.panel2.Controls.Add(this.button_leftIndentation);
            this.panel2.Name = "panel2";
            // 
            // button_rightIndentation
            // 
            this.button_rightIndentation.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.button_rightIndentation, "button_rightIndentation");
            this.button_rightIndentation.Name = "button_rightIndentation";
            this.button_rightIndentation.UseVisualStyleBackColor = true;
            this.button_rightIndentation.Click += new System.EventHandler(this.button_rightIndentation_Click);
            // 
            // button_leftIndentation
            // 
            this.button_leftIndentation.FlatAppearance.BorderSize = 0;
            resources.ApplyResources(this.button_leftIndentation, "button_leftIndentation");
            this.button_leftIndentation.Name = "button_leftIndentation";
            this.button_leftIndentation.UseVisualStyleBackColor = true;
            this.button_leftIndentation.Click += new System.EventHandler(this.button_leftIndentation_Click);
            // 
            // btn_ModifyItem
            // 
            resources.ApplyResources(this.btn_ModifyItem, "btn_ModifyItem");
            this.btn_ModifyItem.Name = "btn_ModifyItem";
            this.btn_ModifyItem.TabStop = false;
            this.btn_ModifyItem.UseVisualStyleBackColor = true;
            this.btn_ModifyItem.Click += new System.EventHandler(this.btn_ModifyItem_Click);
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.btn_ModifyItem);
            this.panel1.Name = "panel1";
            // 
            // label_HeaderType
            // 
            resources.ApplyResources(this.label_HeaderType, "label_HeaderType");
            this.label_HeaderType.Name = "label_HeaderType";
            // 
            // panel3
            // 
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Controls.Add(this.radioButton_H1);
            this.panel3.Controls.Add(this.radioButton_H2);
            this.panel3.Controls.Add(this.radioButton_H3);
            this.panel3.Name = "panel3";
            // 
            // radioButton_H1
            // 
            resources.ApplyResources(this.radioButton_H1, "radioButton_H1");
            this.radioButton_H1.Checked = true;
            this.radioButton_H1.Name = "radioButton_H1";
            this.radioButton_H1.TabStop = true;
            this.radioButton_H1.UseVisualStyleBackColor = true;
            // 
            // radioButton_H2
            // 
            resources.ApplyResources(this.radioButton_H2, "radioButton_H2");
            this.radioButton_H2.Name = "radioButton_H2";
            this.radioButton_H2.UseVisualStyleBackColor = true;
            // 
            // radioButton_H3
            // 
            resources.ApplyResources(this.radioButton_H3, "radioButton_H3");
            this.radioButton_H3.Name = "radioButton_H3";
            this.radioButton_H3.UseVisualStyleBackColor = true;
            // 
            // toolTip_listBoxColors
            // 
            this.toolTip_listBoxColors.AutomaticDelay = 50000;
            this.toolTip_listBoxColors.ShowAlways = true;
            this.toolTip_listBoxColors.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            // 
            // FormCGMLegendOrder
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label_HeaderType);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.txtbox_Header);
            this.Controls.Add(this.button_AddHeaderToList);
            this.Controls.Add(this.label_SymType);
            this.Controls.Add(this.panel_hierarchy);
            this.Controls.Add(this.listBox_LegendItems);
            this.Name = "Form_Legend_ItemsOrder";
            this.ShowIcon = false;
            this.panel_hierarchy.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }




        #endregion

        private System.Windows.Forms.ListBox listBox_LegendItems;
        private System.Windows.Forms.Panel panel_hierarchy;
        private System.Windows.Forms.Button button_Top;
        private System.Windows.Forms.Button button_Down;
        private System.Windows.Forms.Button button_Bottom;
        private System.Windows.Forms.Button button_Up;
        private System.Windows.Forms.TextBox txtbox_Header;
        private System.Windows.Forms.Button button_AddHeaderToList;
        private System.Windows.Forms.Label label_SymType;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button button_rightIndentation;
        private System.Windows.Forms.Button button_leftIndentation;
        private System.Windows.Forms.Button btn_ModifyItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label_HeaderType;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.RadioButton radioButton_H1;
        private System.Windows.Forms.RadioButton radioButton_H2;
        private System.Windows.Forms.RadioButton radioButton_H3;
        private System.Windows.Forms.ToolTip toolTip_listBoxColors;
    }
}