namespace WindowsFormsControlLibraryForRichTextbox
{
    partial class UserControl1
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.htmlTextbox1 = new WindowsFormsControlLibraryForRichTextbox.HtmlTextboxControl.HtmlTextbox();
            this.SuspendLayout();
            // 
            // htmlTextbox1
            // 
            this.htmlTextbox1.BrowserText = "htmlTextbox1";
            this.htmlTextbox1.Fonts = new string[] {
        "Corbel",
        "Corbel, Verdana, Arial, Helvetica, sans-serif",
        "Georgia, Times New Roman, Times, serif",
        "Consolas, Courier New, Courier, monospace"};
            this.htmlTextbox1.IllegalPatterns = new string[] {
        "<script.*?>",
        "<\\w+\\s+.*?(j|java|vb|ecma)script:.*?>",
        "<\\w+(\\s+|\\s+.*?\\s+)on\\w+\\s*=.+?>",
        "</?input.*?>"};
            this.htmlTextbox1.Location = new System.Drawing.Point(0, 3);
            this.htmlTextbox1.Name = "htmlTextbox1";
            this.htmlTextbox1.Padding = new System.Windows.Forms.Padding(1);
            this.htmlTextbox1.ShowHtmlSource = false;
            this.htmlTextbox1.Size = new System.Drawing.Size(569, 165);
            this.htmlTextbox1.TabIndex = 0;
            this.htmlTextbox1.Text = "htmlTextbox1";
            // 
            // UserControl1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.htmlTextbox1);
            this.Name = "UserControl1";
            this.Size = new System.Drawing.Size(599, 201);
            this.ResumeLayout(false);

        }

        #endregion

        private HtmlTextboxControl.HtmlTextbox htmlTextbox1;
    }
}
