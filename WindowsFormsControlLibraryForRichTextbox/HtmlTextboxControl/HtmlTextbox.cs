using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WindowsFormsControlLibraryForRichTextbox.HtmlTextboxControl
{
    [ToolboxBitmap(typeof(HtmlTextbox), "WinformHtmlTextboxIco.bmp")]
    public partial class HtmlTextbox : UserControl
    {

        private const string EditorId = "theTextbox";

        private bool _browserReady = false;
        private bool _focusHandled = false;
        private Control _lastControlFocused;

        private FrmToolbar _toolbar;

        public HtmlTextbox()
        {
            InitializeComponent();

            // Call OnPaint when resized
            this.SetStyle(ControlStyles.ResizeRedraw, true);

            // Initialize the state of the toolbar
            this.ShowHideToolbar();

            // Initialize the Font Size
            // this.SetupFontSizeComboBox();

            // Load the initial code
            this.theBrowser.DocumentText = WindowsFormsControlLibraryForRichTextbox.Properties.Resources.HtmlContent;
            this.BackColor = Color.Black;

            tsbReload.Visible = false;

            tsbFontSize.SelectedIndex = 2;
            tsbFont.SelectedIndex = 2;
        }

        #region Text

        /// <summary>
        /// Returns the text the user edited in Html format
        /// </summary>
        [Category("Appearance")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(BindableSupport.Yes)]
        public override string Text
        {
            get
            {
                return BrowserText;
            }
            set
            {
                this.BrowserText = this.SourceText = value;
            }
        }

        public string BrowserText
        {
            get
            {
                this.WaitUntilBrowserReady();
                var htmlDocument = this.theBrowser.Document;
                if (htmlDocument != null)
                {
                    var htmlElement = htmlDocument.All[EditorId];
                    if (htmlElement != null)
                    {
                        return htmlElement.InnerHtml;
                    }
                }
                return null;
            }
            set
            {
                if (!this._browserReady)
                {
                    return;
                }
                if (theBrowser.Document != null)
                {
                    var htmlElement = theBrowser.Document.All[EditorId];
                    if (htmlElement != null) htmlElement.InnerHtml = value;
                }
            }
        }

        private string SourceText
        {
            get
            {
                return this.FilterHtml(this.txtSource.Text);
            }
            set
            {
                if (value != txtSource.Text)
                {
                    this.txtSource.Text = value;
                }
            }
        }

        #endregion

        #region ToolbarStyle

        private ToolbarStyles _toolbarStyle = ToolbarStyles.AlwaysInternal;
        [DefaultValue(ToolbarStyles.AlwaysInternal)]
        [Category("Appearance")]
        private ToolbarStyles ToolbarStyle
        {
            get { return this._toolbarStyle; }
            set
            {
                if (value != this._toolbarStyle)
                {
                    this._toolbarStyle = value;
                    this.OnToolbarStyleChanged(EventArgs.Empty);
                    this.ShowHideAll();
                }
            }
        }

        private readonly object _toolbarStyleChangedEventKey = new object();

        private void OnToolbarStyleChanged(EventArgs e)
        {
            EventHandler evnt;
            if (null != (evnt = (EventHandler)this.Events[_toolbarStyleChangedEventKey]))
            {
                evnt(this, e);
            }
        }

        #endregion

        #region Showing, hiding syncing source <--> html

        private bool _showHtmlSource;
        [Category("Appearance")]
        public bool ShowHtmlSource
        {
            get
            {
                return this._showHtmlSource;
            }
            set
            {
                this._showHtmlSource = value;
                this.ShowHideSourcePanel();
                if (value == false && this.ContainsFocusExtra)
                {
                    this.WaitUntilBrowserReady();
                    this.theBrowser.Focus();
                }
            }
        }


        private void ShowHideAll()
        {
            this.ShowHideToolbar();
            this.ShowHideSourcePanel();
        }

        /// <summary>
        /// Show the source panel, when
        /// <list type="bullet">
        /// <item>ShowHtmlSource is true</item>
        /// <item>this control has the focus</item>
        /// </list>
        /// </summary>
        private void ShowHideSourcePanel()
        {
            if (this.ShowHtmlSource && this.ContainsFocusExtra)
            {
                this.editSplit.Panel2Collapsed = false;
                if (editSplit.SplitterDistance > this.ClientSize.Height)
                {
                    editSplit.SplitterDistance = editSplit.Height / 2;
                }
            }
            else
            {
                this.editSplit.Panel2Collapsed = true;
            }
        }

        /// <summary>
        /// Returns true, if the HtmlTextBox, or the corresponding (external) toolbar contains focus
        /// </summary>
        private bool ContainsFocusExtra
        {
            get
            {
                if (this.ContainsFocus) return true;
                if (this._toolbar != null && this._toolbar.ContainsFocus) return true;
                return false;
            }
        }

        public void ShowHideToolbar()
        {
            if (ToolbarStyle == ToolbarStyles.External)
            {
                // External toolbar
                this.toolBar.Visible = false;
                if (this._toolbar == null)
                {
                    this._toolbar = new FrmToolbar(this);
                    this.components.Add(this._toolbar);
                }
                if (this.ContainsFocusExtra)
                {
                    this._toolbar.Appear();
                }
                else
                {
                    this._toolbar.Disappear();
                }
            }
            else
            {
                // No external toolbar
                if (this._toolbar != null)
                {
                    this._toolbar.Dispose();
                    this._toolbar = null;
                }
                switch (this.ToolbarStyle)
                {
                    case ToolbarStyles.Internal:
                        if (this.ContainsFocusExtra)
                        {
                            this.toolBar.Visible = true;
                        }
                        else
                        {
                            this.toolBar.Visible = false;
                        }
                        break;
                    case ToolbarStyles.AlwaysInternal:
                        this.toolBar.Visible = true;
                        break;
                    case ToolbarStyles.Hide:
                        this.toolBar.Visible = false;
                        break;
                } // switch
            } // else
        }

        /// <summary>
        /// Returns true, if the browser is focused, or the browser was the last
        /// control focused, before the external toolbar was focused
        /// </summary>
        public bool IsBrowserFocused
        {
            get
            {
                if (this.theBrowser.Focused)
                {
                    this._lastControlFocused = this.theBrowser;
                    return true;
                }
                if (this.ContainsFocusExtra)
                {
                    return this._lastControlFocused == this.theBrowser;
                }
                return false;
            }
        }

        /// <summary>
        /// Returns true, if the source view is focused, or the source view was the
        /// last control focused before the external toolbar got the focus.
        /// </summary>
        internal bool IsSourceFocused
        {
            get
            {
                return this.txtSource.Focused || this._lastControlFocused == this.txtSource;
            }
        }

        /// <summary>
        /// Synchronizes the content of the browser, to the content of the source
        /// textbox.
        /// </summary>
        private void SyncBrowser()
        {
            this.BrowserText = this.SourceText;
        }

        /// <summary>
        /// Synchronizes the content of the txtSource to the browser
        /// </summary>
        private void SyncSource()
        {
            this.SourceText = this.BrowserText;
        }

        #endregion

        #region Cut / Copy / Paste / Image Insert

        private void tsbInsertImage_Click(object sender, EventArgs e)
        {
            this.WaitUntilBrowserReady();
            var htmlDocument = this.theBrowser.Document;
            if (htmlDocument != null) htmlDocument.ExecCommand("InsertImage", true, null);
        }

        #endregion

        #region Simple style toggles : Bold, Italic, Indent, UnIndent, Lists

        private void tsbBold_Click(object sender, EventArgs e)
        {
            this.WaitUntilBrowserReady();
            var htmlDocument = this.theBrowser.Document;
            if (htmlDocument != null) htmlDocument.ExecCommand("Bold", false, null);
        }

        private void tsbItalic_Click(object sender, EventArgs e)
        {
            this.WaitUntilBrowserReady();
            var htmlDocument = this.theBrowser.Document;
            if (htmlDocument != null) htmlDocument.ExecCommand("Italic", false, null);
        }

        private void tsbUnderline_Click(object sender, EventArgs e)
        {
            this.WaitUntilBrowserReady();
            var htmlDocument = this.theBrowser.Document;
            if (htmlDocument != null) htmlDocument.ExecCommand("Underline", false, null);
        }

        private void tsbOrderedList_Click(object sender, EventArgs e)
        {
            this.WaitUntilBrowserReady();
            var htmlDocument = this.theBrowser.Document;
            if (htmlDocument != null)
                htmlDocument.ExecCommand("InsertOrderedList", false, null);
        }

        private void tsbBulletList_Click(object sender, EventArgs e)
        {
            this.WaitUntilBrowserReady();
            var htmlDocument = this.theBrowser.Document;
            if (htmlDocument != null)
                htmlDocument.ExecCommand("InsertUnOrderedList", false, null);
        }

        private void tsbUnIndent_Click(object sender, EventArgs e)
        {
            this.WaitUntilBrowserReady();
            var htmlDocument = this.theBrowser.Document;
            if (htmlDocument != null) htmlDocument.ExecCommand("Outdent", false, null);
        }

        private void tsbIndent_Click(object sender, EventArgs e)
        {
            this.WaitUntilBrowserReady();
            var htmlDocument = this.theBrowser.Document;
            if (htmlDocument != null) htmlDocument.ExecCommand("Indent", false, null);
        }

        #endregion

        #region Handle Font

        private void tsbFont_Leave(object sender, EventArgs e)
        {
            this.WaitUntilBrowserReady();
            var htmlDocument = this.theBrowser.Document;
            if (htmlDocument != null)
                htmlDocument.ExecCommand("FontName", false, this.tsbFont.Text);
        }

        private void tsbFont_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                this.WaitUntilBrowserReady();
                var htmlDocument = this.theBrowser.Document;
                if (htmlDocument != null)
                    htmlDocument.ExecCommand("FontName", false, this.tsbFont.Text);
                this.theBrowser.Focus();
                e.Handled = true;
            }
        }

        private void tsbFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.WaitUntilBrowserReady();
            var htmlDocument = this.theBrowser.Document;
            if (htmlDocument != null)
                htmlDocument.ExecCommand("FontName", false, this.tsbFont.Text);
        }

        private void SetupFontSizeComboBox()
        {
            tsbFontSize.Items.Add("Size");
            for (int x = 1; x <= 5; x++)
            {
                tsbFontSize.Items.Add(x.ToString(CultureInfo.InvariantCulture) + " pt");
            }

            tsbFontSize.SelectedIndex = 0;
            tsbFontSize.SelectedIndexChanged += new EventHandler(tsbFontSize_SelectedIndexChanged);
        }

        private void tsbFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.WaitUntilBrowserReady();
            var htmlDocument = this.theBrowser.Document;
            if (htmlDocument != null)
                htmlDocument.ExecCommand("FontSize", false, this.tsbFontSize.Text);
        }

        private void tsbFontSize_Leave(object sender, EventArgs e)
        {
            this.WaitUntilBrowserReady();
            var htmlDocument = this.theBrowser.Document;
            if (htmlDocument != null)
                htmlDocument.ExecCommand("FontSize", false, this.tsbFontSize.Text);
        }

        private void tsbFontSize_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                this.WaitUntilBrowserReady();
                var htmlDocument = this.theBrowser.Document;
                if (htmlDocument != null)
                    htmlDocument.ExecCommand("FontSize", false, this.tsbFontSize.Text);
                this.theBrowser.Focus();
                e.Handled = true;
            }
        }

        #endregion

        #region Misc events: DocumentCompleted, OnPaint, TextChanged

        private void theBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            this._browserReady = true;
            this.SyncBrowser();
            this.SetFontInBrowser();
        }

        private void WaitUntilBrowserReady()
        {
            if (this._browserReady)
            {
                return;
            }
            for (int i = 0; i < 60 && !this._browserReady; i++)
            {
                System.Threading.Thread.Sleep(100);
                Application.DoEvents();
            }
        }

        private Color _borderColorFocused = SystemColors.ActiveCaption;
        private Color _borderColorNonFocused = SystemColors.ControlDark;

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "ActiveCaption")]
        private Color BorderColorFocused
        {
            get { return this._borderColorFocused; }
            set { this._borderColorFocused = value; }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Color), "ControlDark")]
        private Color BorderColorNonFocused
        {
            get { return this._borderColorNonFocused; }
            set { this._borderColorNonFocused = value; }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle,
                this.ContainsFocus ? this.BorderColorFocused : this.BorderColorNonFocused, ButtonBorderStyle.Solid);
        }

        private void txtSource_TextChanged(object sender, EventArgs e)
        {
            this.OnTextChanged(EventArgs.Empty);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            this.SetFontInBrowser();
        }

        private void SetFontInBrowser()
        {
            if (this._browserReady)
            {
                var htmlDocument = this.theBrowser.Document;
                if (htmlDocument != null)
                    htmlDocument.InvokeScript("SetFont",
                        new object[] {
                            this.Font.Name,
                            this.Font.Size.ToString(System.Globalization.CultureInfo.InvariantCulture) + "pt"});
            }
        }

        #endregion

        private string[] _fonts = null;
        private readonly string[] _defaultFonts = new string[] {
            "Corbel",
            "Corbel, Verdana, Arial, Helvetica, sans-serif",
            "Georgia, Times New Roman, Times, serif",
            "Consolas, Courier New, Courier, monospace" };
        [Category("Behavior")]
        public string[] Fonts
        {
            get { return _fonts ?? _defaultFonts; }
            set
            {
                if (value == null || value.Length == 0)
                {
                    _fonts = null;
                }
                this._fonts = value;

                this.tsbFont.Items.Clear();
                if (_fonts != null)
                {
                    this.tsbFont.Items.AddRange(_fonts);
                    if (this._toolbar != null)
                    {
                        this._toolbar.tsbFont.Items.Clear();
                        this._toolbar.tsbFont.Items.AddRange(this._fonts);
                        tsbFont.SelectedIndex = 2;
                    }
                }
#if DEBUG
                foreach (var item in System.Drawing.FontFamily.Families)
                {
                    Debug.WriteLine(item);
                }
#endif
            }
        }

        private static readonly string[] IllegalPatternsDefault = new string[] {
                @"<script.*?>",                            // all <script >
                @"<\w+\s+.*?(j|java|vb|ecma)script:.*?>",  // any tag containing *script:
                @"<\w+(\s+|\s+.*?\s+)on\w+\s*=.+?>",       // any tag containing an attribute starting with "on"
                @"</?input.*?>"                            // <input> and </input>
            };
        private string[] _illegalPatterns = IllegalPatternsDefault;
        /// <summary>
        /// Contains a list of regular expression that are cleared from the html.
        /// Like script, of event handlers
        /// </summary>
        [Category("Behavior")]
        [Description(@"A list of regular expressions that are removed from the html. To reset, set to single line with *.")]
        public string[] IllegalPatterns
        {
            get
            {
                if (this._illegalPatterns == null)
                {
                    return new string[0];
                }
                return this._illegalPatterns;
            }
            set
            {
                // When zero length then store a null
                if (value == null || value.Length == 0)
                {
                    this._illegalPatterns = null;
                    return;
                }
                if (value.Length == 1 && value[0] == "*")
                {
                    this._illegalPatterns = IllegalPatternsDefault;
                    return;
                }
                // Remove empty & duplicate strings
                var buf = new List<string>();
                foreach (var item in value)
                {
                    if (!string.IsNullOrEmpty(item) && !buf.Contains(item))
                    {
                        buf.Add(item);
                    }
                }
                this._illegalPatterns = buf.Count == 0 ? null : buf.ToArray();
            }
        }

        private string FilterHtml(string original)
        {
            if (string.IsNullOrEmpty(original) || this.IllegalPatterns.Length == 0)
            {
                return original;
            }
            string buf = this.IllegalPatterns.Select(pattern => new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline)).Aggregate(original, (current, reg) => reg.Replace(current, string.Empty));
            //System.Diagnostics.Debug.WriteLineIf(buf != original, "Filtered: " + buf);
            return buf;
        }

        private void txtSource_Enter(object sender, EventArgs e)
        {
            this._lastControlFocused = this.txtSource;
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            // If Enter is pressed, this key does not need to be handled by the dialog,
            if (keyData == Keys.Return)
            {
                return false;
            }

            return base.ProcessDialogKey(keyData);
        }

        private void SaveInMemoryImageOnPasteEvent()
        {
            var imageUrl = ReadImageInBase64();
            if (imageUrl != null)
            {
                var htmlDocument = this.theBrowser.Document;
                if (htmlDocument != null)
                    htmlDocument.InvokeScript("SetImageData", new[] { imageUrl });
            }
        }

        private string ReadImageInBase64()
        {
            try
            {
                IDataObject d = Clipboard.GetDataObject();

                if (d != null && d.GetDataPresent(DataFormats.Bitmap))
                {
                    var bitmapImage = (Bitmap)d.GetData(DataFormats.Bitmap);
                    string imageNameWithPath = CreateImagePathWithName();
                    bitmapImage.Save(imageNameWithPath, System.Drawing.Imaging.ImageFormat.Png);
                    Clipboard.Clear();

                    return imageNameWithPath;
                }
            }
            catch (Exception)
            {
                //MessageBox.Show(GeneralStrings.HtmlTextbox_ReadImageInBase64_Unable_to_insert_image_Please_try_again);
            }

            return null;
        }

        private void tbsPasteImage_Click(object sender, EventArgs e)
        {
            SaveInMemoryImageOnPasteEvent();
        }

        public static string CreateImagePathWithName()
        {
            return Path.Combine(@"D:\Git-New\NaviPlannerWpf\NaviPlannerWPF\bin\Debug\NaviPlannerData\VoyageReports\Data\ReportImages", "Report_Image_" + DateTime.Now.ToString("yyyyMMdd_hhmmsstt") + "_" + ".png");
        }
    }
}
