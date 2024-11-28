namespace GSC_ProjectEditor
{
    partial class Form_RefreshSymbols
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_RefreshSymbols));
            this.checkedListBox_RefreshSymbols = new System.Windows.Forms.CheckedListBox();
            this.label_refreshSymbolQuestion = new System.Windows.Forms.Label();
            this.button_OK = new System.Windows.Forms.Button();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // checkedListBox_RefreshSymbols
            // 
            this.checkedListBox_RefreshSymbols.BackColor = System.Drawing.Color.White;
            this.checkedListBox_RefreshSymbols.CheckOnClick = true;
            this.checkedListBox_RefreshSymbols.FormattingEnabled = true;
            this.checkedListBox_RefreshSymbols.Items.AddRange(new object[] {
            resources.GetString("checkedListBox_RefreshSymbols.Items"),
            resources.GetString("checkedListBox_RefreshSymbols.Items1"),
            resources.GetString("checkedListBox_RefreshSymbols.Items2"),
            resources.GetString("checkedListBox_RefreshSymbols.Items3")});
            resources.ApplyResources(this.checkedListBox_RefreshSymbols, "checkedListBox_RefreshSymbols");
            this.checkedListBox_RefreshSymbols.Name = "checkedListBox_RefreshSymbols";
            // 
            // label_refreshSymbolQuestion
            // 
            resources.ApplyResources(this.label_refreshSymbolQuestion, "label_refreshSymbolQuestion");
            this.label_refreshSymbolQuestion.Name = "label_refreshSymbolQuestion";
            // 
            // button_OK
            // 
            resources.ApplyResources(this.button_OK, "button_OK");
            this.button_OK.Name = "button_OK";
            this.button_OK.UseVisualStyleBackColor = true;
            this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
            // 
            // button_Cancel
            // 
            resources.ApplyResources(this.button_Cancel, "button_Cancel");
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.UseVisualStyleBackColor = true;
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // FormRefreshSymbols
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.button_OK);
            this.Controls.Add(this.label_refreshSymbolQuestion);
            this.Controls.Add(this.checkedListBox_RefreshSymbols);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", global::GSC_ProjectEditor.Properties.Settings.Default, "dwEnabling", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Enabled = global::GSC_ProjectEditor.Properties.Settings.Default.dwEnabling;
            this.Name = "Form_RefreshSymbols";
            this.ShowIcon = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox checkedListBox_RefreshSymbols;
        private System.Windows.Forms.Label label_refreshSymbolQuestion;
        private System.Windows.Forms.Button button_OK;
        private System.Windows.Forms.Button button_Cancel;
        private System.Windows.Forms.Label label1;
    }
}