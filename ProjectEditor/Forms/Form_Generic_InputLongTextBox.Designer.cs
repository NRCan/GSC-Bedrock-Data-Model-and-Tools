namespace GSC_ProjectEditor
{
    partial class Form_Generic_InputLongTextBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Generic_InputLongTextBox));
            this.textBox_LongText = new System.Windows.Forms.TextBox();
            this.button_Paste = new System.Windows.Forms.Button();
            this.button_Clear = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox_LongText
            // 
            resources.ApplyResources(this.textBox_LongText, "textBox_LongText");
            this.textBox_LongText.Name = "textBox_LongText";
            // 
            // button_Paste
            // 
            resources.ApplyResources(this.button_Paste, "button_Paste");
            this.button_Paste.Name = "button_Paste";
            this.button_Paste.UseVisualStyleBackColor = true;
            this.button_Paste.Click += new System.EventHandler(this.button_Paste_Click);
            // 
            // button_Clear
            // 
            resources.ApplyResources(this.button_Clear, "button_Clear");
            this.button_Clear.Name = "button_Clear";
            this.button_Clear.UseVisualStyleBackColor = true;
            this.button_Clear.Click += new System.EventHandler(this.button_Clear_Click);
            // 
            // FormLongText
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button_Clear);
            this.Controls.Add(this.button_Paste);
            this.Controls.Add(this.textBox_LongText);
            this.Name = "Form_Generic_InputLongTextBox";
            this.ShowIcon = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_LongText;
        private System.Windows.Forms.Button button_Paste;
        private System.Windows.Forms.Button button_Clear;
    }
}