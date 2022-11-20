using ComponentOwl.BetterListView;
using DiscordRPC;
using DiscordRPC.Logging;
using FastColoredTextBoxNS;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using File = System.IO.File;

namespace Dragon_
{
    public partial class DragonIDE : Form
    {
        public DragonIDE()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();

            Refresh();

            CheckForIllegalCrossThreadCalls = false;
        }

        private Rectangle sizeGripRectangle;
        int paintReps = 0;

        protected override void OnPaint(PaintEventArgs e)
        {

            base.OnPaint(e);
            ControlPaint.DrawSizeGrip(e.Graphics, Color.Transparent, sizeGripRectangle);
            System.Threading.Thread.Sleep(0);

            if (paintReps++ % 500 == 0)
                Application.DoEvents();

        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            this.Invalidate();

            base.OnScroll(se);

            base.DoubleBuffered = true;

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();

            base.OnScroll(se);

        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;

                return cp;
            }
        }

        private static void RegisterForFileExtension(string extension, string applicationPath)
        {
            RegistryKey FileReg = Registry.CurrentUser.CreateSubKey("Software\\Classes\\" + extension);
            FileReg.CreateSubKey("shell\\open\\command").SetValue("", $"\"{applicationPath}\" \"%1\"");
            FileReg.Close();

            SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
        }
        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

        string filePath;
        string[] paths = Environment.GetCommandLineArgs();

        void deleteGoFile()
        {


            RegistryKey FileExts = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts");
            RegistryKey hdr = FileExts.OpenSubKey(".go", true);
            foreach (String key in hdr.GetSubKeyNames())
            hdr.DeleteSubKey(key);
            hdr.Close();
            FileExts.DeleteSubKeyTree(".go");


        }

        private void DragonIDE_Load(object sender, EventArgs e)
        {

            pnExplorer.BringToFront();

            selectedTB = tbCode;


            try
            {


                filePath = Environment.GetCommandLineArgs()[1];

                if (paths.Length > 1)
                {

                    string xArgs = System.IO.File.ReadAllText(filePath);
                    RegisterForFileExtension(".df", "C:\\Dragon+\\Dragon+.exe");
                    RegisterForFileExtension(".dp", "C:\\Dragon+\\Dragon+.exe");
                    RegisterForFileExtension(".go", "C:\\Dragon+\\Dragon+.exe");

                    tbCode.Text = xArgs;


                }

            }
            catch
            {



            }


            initialized = true;
            client = new DiscordRpcClient("1042767542755803136");
            client.Logger = new ConsoleLogger() { Level = DiscordRPC.Logging.LogLevel.Warning };
            client.Initialize();

            XboxInfo();

            client.SetPresence(new DiscordRPC.RichPresence()
            {
                Details = $"Coding  {guna2TabControl1.SelectedTab.Text}.",
                State = $"{txtProfile.Text} signed in.",
                Timestamps = Timestamps.Now,
                Assets = new Assets()
                {
                    LargeImageKey = $"dglogo",
                    LargeImageText = "Dragon+ IDE",
                    SmallImageKey = $"src",
                    SmallImageText = "Dragon+ GoLang"
                }
            });

        }

        string _xboxName;
        string _xboxIconLink;

        public DiscordRpcClient client;
        bool initialized = false;

        private void XboxIconCompleted(object sender, AsyncCompletedEventArgs e)
        {
            pbProfile.Image = new Bitmap("C:\\Dragon+\\icon.png");
            pbProfile2.Image = new Bitmap("C:\\Dragon+\\icon.png");
        }

        public void OpenFile(string args)
        {

            tbCode.Text = args;

        }

        private void XboxInfo()
        {
            string environmentVariable = Environment.GetEnvironmentVariable("LocalAppData");

            if (System.IO.File.Exists("C:\\Dragon+\\XboxLiveGamer.xml.txt"))
            {
                System.IO.File.Delete("C:\\Dragon+\\XboxLiveGamer.xml.txt");
            }
            else
            {

                Directory.CreateDirectory("C:\\Dragon+");

            }
            if (!System.IO.File.Exists(environmentVariable + "\\Packages\\Microsoft.XboxApp_8wekyb3d8bbwe\\LocalState\\XboxLiveGamer.xml"))
            {
                return;
            }
            try
            {
                System.IO.File.Copy(environmentVariable + "\\Packages\\Microsoft.XboxApp_8wekyb3d8bbwe\\LocalState\\XboxLiveGamer.xml", "C:\\Dragon+\\XboxLiveGamer.xml.txt");
                foreach (string text in System.IO.File.ReadAllLines("C:\\Dragon+\\XboxLiveGamer.xml.txt"))
                {
                    if (text.Contains("Gamertag"))
                    {
                        this._xboxName = text;
                    }
                    else if (text.Contains("DisplayPic"))
                    {
                        this._xboxIconLink = text;
                    }
                }
                this._xboxName = this._xboxName.Replace("\"Gamertag\"", "");
                this._xboxName = this._xboxName.Replace("\"", "");
                this._xboxName = this._xboxName.Replace(": ", "");
                this._xboxName = this._xboxName.Replace(",", "");
                this._xboxName = this._xboxName.Trim();
                this.txtProfile.Text = this._xboxName;
                this.txtProfile2.Text = this._xboxName;
                this._xboxIconLink = this._xboxIconLink.Replace("\"DisplayPic\"", "");
                this._xboxIconLink = this._xboxIconLink.Replace("\"", "");
                this._xboxIconLink = this._xboxIconLink.Replace(": ", "");
                this._xboxIconLink = this._xboxIconLink.Replace(",", "");
                WebClient webClient = new WebClient();
                webClient.DownloadFileCompleted += this.XboxIconCompleted;
                webClient.DownloadFileAsync(new Uri(this._xboxIconLink), "C:\\Dragon+\\icon.png");
            }
            catch (ArgumentException)
            {
                txtProfile.Text = "Unknown User";
                txtProfile2.Text = "Unknown User";
            }
        }

        class EllipseStyle : Style
        {
            public override void Draw(Graphics gr, Point position, Range range)
            {
                //get size of rectangle
                Size size = GetSizeOfRange(range);
                //create rectangle
                Rectangle rect = new Rectangle(position, size);
                //inflate it
                rect.Inflate(2, 2);
                //get rounded rectangle
                var path = GetRoundedRectangle(rect, 7);
                //draw rounded rectangle
                gr.DrawPath(Pens.SkyBlue, path);
            }
        }

        string lang = "GoLang";

        public readonly SolidBrush firstStyle = new SolidBrush(Color.FromArgb(244, 112, 103));
        public readonly SolidBrush secondStyle = new SolidBrush(Color.FromArgb(220, 176, 201));
        public readonly SolidBrush thirdStyle = new SolidBrush(Color.FromArgb(150, 208, 255));
        public readonly SolidBrush fourthStyle = new SolidBrush(Color.FromArgb(98, 182, 243));
        public readonly SolidBrush fifthStyle = new SolidBrush(Color.FromArgb(89, 117, 132));

        TextStyle BlueStyle = new TextStyle(Brushes.BlueViolet, null, FontStyle.Bold);
        TextStyle BoldStyle = new TextStyle(null, null, FontStyle.Bold | FontStyle.Underline);
        TextStyle GrayStyle = new TextStyle(Brushes.Gray, null, FontStyle.Regular);
        TextStyle MagentaStyle = new TextStyle(Brushes.Magenta, null, FontStyle.Regular);
        TextStyle GreenStyle = new TextStyle(Brushes.PaleGreen, null, FontStyle.Regular);
        TextStyle BrownStyle = new TextStyle(Brushes.Brown, null, FontStyle.Italic);
        TextStyle FunctionStyle = new TextStyle(Brushes.MediumPurple, null, FontStyle.Regular);
        TextStyle MedFunctionStyle = new TextStyle(Brushes.Orange, null, FontStyle.Regular);
        TextStyle LGrayStyle = new TextStyle(Brushes.Gray, null, FontStyle.Regular);
        TextStyle OrchStyle = new TextStyle(Brushes.Orchid, null, FontStyle.Regular);
        TextStyle TyLStyle = new TextStyle(Brushes.Teal, null, FontStyle.Regular);
        TextStyle MaroonStyle = new TextStyle(Brushes.MediumAquamarine, null, FontStyle.Bold);
        TextStyle funcColor = new TextStyle(Brushes.Khaki, null, FontStyle.Regular);
        EllipseStyle ellipseStyle = new EllipseStyle();



        private void GoHighlight(TextChangedEventArgs e)
        {

            BlueStyle.ForeBrush = firstStyle;
            funcColor.ForeBrush = secondStyle;
            GreenStyle.ForeBrush = thirdStyle;
            MagentaStyle.ForeBrush = fourthStyle;
            LGrayStyle.ForeBrush = fifthStyle;

            selectedTB.LeftBracket = '(';
            selectedTB.RightBracket = ')';
            selectedTB.LeftBracket2 = '\x0';
            selectedTB.RightBracket2 = '\x0';

            //clear style of changed range
            e.ChangedRange.ClearStyle(MedFunctionStyle, MaroonStyle, LGrayStyle, OrchStyle, BlueStyle, BoldStyle, GrayStyle, MagentaStyle, GreenStyle, BrownStyle, FunctionStyle, funcColor, ellipseStyle);

            //string highlighting
            e.ChangedRange.SetStyle(GreenStyle, @"""""|@""""|''|@"".*?""|(?<!@)(?<range>"".*?[^\\]"")|'.*?[^\\]'");
            //comment highlighting
            e.ChangedRange.SetStyle(LGrayStyle, @"//.*$", RegexOptions.Multiline);
            e.ChangedRange.SetStyle(LGrayStyle, @"(/\*.*?\*/)|(/\*.*)", RegexOptions.Singleline);
            e.ChangedRange.SetStyle(LGrayStyle, @"(/\*.*?\*/)|(.*\*/)", RegexOptions.Singleline | RegexOptions.RightToLeft);
            //number highlighting
            e.ChangedRange.SetStyle(MagentaStyle, @"\b\d+[\.]?\d*([eE]\-?\d+)?[lLdDfF]?\b|\b0x[a-fA-F\d]+\b");
            //attribute highlighting
            e.ChangedRange.SetStyle(GrayStyle, @"^\s*(?<range>\[.+?\])\s*$", RegexOptions.Multiline);
            //class name highlighting
            e.ChangedRange.SetStyle(funcColor, @"\b(#|var|package|func|type|for|range|return|new|.|:=)\s+(?<range>\w+?)\b");
            //keyword highlighting
            // e.ChangedRange.SetStyle(MedFunctionStyle, @"\b(abstract|as|break|byte|case|checked|const|continue|decimal|default|delegate|do|double|muidu|enum|event|explicit|extern|finally|fixed|float|goto|implicit|in|interface|internal|is|lock|long|nil|object|operator|out|override|params|private|protected|public|readonly|ref|sbyte|sealed|short|sizeof|switch|this.|throw|typeof|uint|ulong|unchecked|unsafe|ushort|virtual|add|alias|ascending|descending|dynamic|from|get|global|group|into|join|let|orderby|partial|remove|select|set|value|yield)\b|#region\b|#endregion\b");

            e.ChangedRange.SetStyle(BlueStyle, @"import|bool|int|string|float|Printf|package|case|switch|default|:=|func|type|struct|return|for|range|if|else|where|else|var|try|catch|while|char|=|new");
            e.ChangedRange.SetStyle(BlueStyle, @"interface|output");
            e.ChangedRange.SetStyle(GreenStyle, @"true|false|nil");

            //append style for word 'Babylon'
            e.ChangedRange.SetStyle(ellipseStyle, @"{", RegexOptions.IgnoreCase);
            e.ChangedRange.SetStyle(ellipseStyle, @"}", RegexOptions.IgnoreCase);


            //clear folding markers
            e.ChangedRange.ClearFoldingMarkers();

            //set folding markers
            e.ChangedRange.SetFoldingMarkers("{", "}");
            e.ChangedRange.SetFoldingMarkers(@"#region\b", @"#endregion\b");
            e.ChangedRange.SetFoldingMarkers(@"/\*", @"\*/");

        }

        private void DragonIDE_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void tbCode_Load(object sender, EventArgs e)
        {
            
        }

        private void tbCode_TextChanged(object sender, TextChangedEventArgs e)
        {

            /*if (guna2TabControl1.TabPages.Count > 0)
            {

                if (!guna2TabControl1.SelectedTab.Text.Contains("*"))
                    guna2TabControl1.SelectedTab.Text += "*";

            } */

            switch (lang)
            {
                case "GoLang":
                    GoHighlight(e);
                    break;
                default:
                    break;
            }
        }

        private void tbCode_ZoomChanged(object sender, EventArgs e)
        {
            txtZoomLevel.Text = tbCode.Zoom.ToString() + "% Zoomed";
        }

        private void label2_Click(object sender, EventArgs e)
        {

            if (CMSFile.Visible != true)
            {

                CMSFile.Show(label2.PointToScreen(new Point(0, label2.Height)));

            }
            else
            {

                CMSFile.Hide();

            }

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label3_Click(object sender, EventArgs e)
        {

            if (CMSEdit.Visible != true)
            {

                CMSEdit.Show(label3.PointToScreen(new Point(0, label3.Height)));

            }
            else
            {

                CMSEdit.Hide();

            }

        }

        private void label4_Click(object sender, EventArgs e)
        {

            if (CMSView.Visible != true)
            {

                CMSView.Show(label4.PointToScreen(new Point(0, label4.Height)));

            }
            else
            {

                CMSView.Hide();

            }

        }

        private void label5_Click(object sender, EventArgs e)
        {

            if (CMSBuild.Visible != true)
            {

                CMSBuild.Show(label5.PointToScreen(new Point(0, label5.Height)));

            }
            else
            {

                CMSBuild.Hide();

            }

        }

        private void label6_Click(object sender, EventArgs e)
        {

            if (CMSHelp.Visible != true)
            {

                CMSHelp.Show(label6.PointToScreen(new Point(0, label6.Height)));

            }
            else
            {

                CMSHelp.Hide();

            }

        }

        private void guna2ImageButton1_MouseEnter(object sender, EventArgs e)
        {

            pnHoverT.BringToFront();


            txtHoverT.Text = "Explorer";


            pnHoverT.Location = PointToClient(MousePosition);
            pnHoverT.Show();

        }

        private void guna2ImageButton2_MouseEnter(object sender, EventArgs e)
        {

            pnHoverT.BringToFront();


            txtHoverT.Text = "Projects";


            pnHoverT.Location = PointToClient(MousePosition);
            pnHoverT.Show();
        }

        private void guna2ImageButton3_MouseEnter(object sender, EventArgs e)
        {

            pnHoverT.BringToFront();


            txtHoverT.Text = "Modules";


            pnHoverT.Location = PointToClient(MousePosition);
            pnHoverT.Show();
        }

        private void guna2ImageButton4_MouseEnter(object sender, EventArgs e)
        {

            pnHoverT.BringToFront();


            txtHoverT.Text = "Collaboration";


            pnHoverT.Location = PointToClient(MousePosition);
            pnHoverT.Show();
        }

        private void guna2ImageButton1_MouseLeave(object sender, EventArgs e)
        {
            pnHoverT.Hide();
        }

        private void guna2ImageButton2_MouseLeave(object sender, EventArgs e)
        {
            pnHoverT.Hide();
        }

        private void guna2ImageButton3_MouseLeave(object sender, EventArgs e)
        {
            pnHoverT.Hide();
        }

        private void guna2ImageButton4_MouseLeave(object sender, EventArgs e)
        {
            pnHoverT.Hide();
        }

        private void guna2ImageButton1_Click(object sender, EventArgs e)
        {
            pnExplorer.BringToFront();
            pnHoverT.BringToFront();
        }

        private void guna2ImageButton2_Click(object sender, EventArgs e)
        {
            pnProjects.BringToFront();
            pnHoverT.BringToFront();
        }

        private void guna2ImageButton3_Click(object sender, EventArgs e)
        {
            pnModules.BringToFront();
            pnHoverT.BringToFront();
        }

        private void guna2ImageButton4_Click(object sender, EventArgs e)
        {
            pnCollaboration.BringToFront();
            pnHoverT.BringToFront();
        }

        private void guna2ImageButton5_Click(object sender, EventArgs e)
        {

        }

        private void guna2ImageButton5_MouseEnter(object sender, EventArgs e)
        {

            pnHoverT.BringToFront();


            txtHoverT.Text = "Warnings";


            pnHoverT.Location = PointToClient(MousePosition);
            pnHoverT.Show();
        }

        private void guna2ImageButton5_MouseLeave(object sender, EventArgs e)
        {
            pnHoverT.Hide();
        }

        private void pbProfile_Click(object sender, EventArgs e)
        {
            if (pnAccount.Visible == false)
            {

                pnAccount.BringToFront();
                guna2Transition1.ShowSync(pnAccount, true);

            }
            else
                guna2Transition1.HideSync(pnAccount, true);
        }

        private void txtProfile_Click(object sender, EventArgs e)
        {
            if (pnAccount.Visible == false)
            {

                pnAccount.BringToFront();
                guna2Transition1.ShowSync(pnAccount, true);

            }
            else
                guna2Transition1.HideSync(pnAccount, true);
        }

        private Control CloneControl(Control srcCtl)
        {
            var cloned = Activator.CreateInstance(srcCtl.GetType()) as Control;
            var binding = BindingFlags.Public | BindingFlags.Instance;
            foreach (PropertyInfo prop in srcCtl.GetType().GetProperties(binding))
            {
                if (IsClonable(prop))
                {
                    object val = prop.GetValue(srcCtl);
                    prop.SetValue(cloned, val, null);
                }
            }

            foreach (Control ctl in srcCtl.Controls)
            {
                cloned.Controls.Add(CloneControl(ctl));
            }

            return cloned;
        }

        private bool IsClonable(PropertyInfo prop)
        {
            var browsableAttr = prop.GetCustomAttribute(typeof(BrowsableAttribute), true) as BrowsableAttribute;
            var editorBrowsableAttr = prop.GetCustomAttribute(typeof(EditorBrowsableAttribute), true) as EditorBrowsableAttribute;

            return prop.CanWrite
                && (browsableAttr == null || browsableAttr.Browsable == true)
                && (editorBrowsableAttr == null || editorBrowsableAttr.State != EditorBrowsableState.Advanced);
        }

        int tabs = 1;

        FastColoredTextBox selectedTB = new FastColoredTextBox();

        private void guna2ImageButton6_Click(object sender, EventArgs e)
        {

            tabs += 1;

            TabPage tb = new TabPage();

            tb.Text = $"Untitled{tabs}.go";
            tb.ImageIndex = 0;
            tb.BackColor = Color.FromArgb(6, 26, 64);

            FastColoredTextBox newTxt = new FastColoredTextBox();

            newTxt.BackColor = tbCode.BackColor;
            newTxt.ForeColor = tbCode.ForeColor;
            newTxt.Language = tbCode.Language;
            newTxt.TextChanged += tbCode_TextChanged;
            newTxt.KeyDown += tbCode_KeyDown;
            newTxt.Font = tbCode.Font;
            newTxt.Anchor = tbCode.Anchor;
            newTxt.Size = tbCode.Size;
            newTxt.Location = tbCode.Location;
            newTxt.AutoCompleteBrackets = true;
            newTxt.AutoCompleteBracketsList = tbCode.AutoCompleteBracketsList;
            newTxt.AutoIndentCharsPatterns = tbCode.AutoIndentCharsPatterns;
            newTxt.CaretColor = tbCode.CaretColor;
            newTxt.CurrentLineColor = tbCode.CurrentLineColor;
            newTxt.DisabledColor = tbCode.DisabledColor;
            newTxt.HighlightingRangeType = tbCode.HighlightingRangeType;
            newTxt.IndentBackColor = tbCode.IndentBackColor;
            newTxt.SelectionColor = tbCode.SelectionColor;
            newTxt.ShowScrollBars = false;
            newTxt.ShowFoldingLines = true;
            newTxt.PaddingBackColor = tbCode.PaddingBackColor;
            newTxt.LineNumberColor = tbCode.LineNumberColor;
            newTxt.ServiceLinesColor = tbCode.ServiceLinesColor;
            newTxt.AutoScrollMinSize = tbCode.AutoScrollMinSize;
            newTxt.LeftPadding = tbCode.LeftPadding;
            newTxt.Paddings = tbCode.Paddings;

            DocumentMap dcm = new DocumentMap();
            dcm.Target = newTxt;
            dcm.Text = String.Empty;
            dcm.Font = new Font("Gordita", 9);
            dcm.ForeColor = Color.White;
            dcm.BackColor = Color.FromArgb(13, 32, 64);
            dcm.Size = documentMap1.Size;
            dcm.Location = documentMap1.Location;
            dcm.Anchor = documentMap1.Anchor;

            tb.Controls.Add(dcm);

            tb.Controls.Add(newTxt);

            guna2TabControl1.TabPages.Add(tb);


            BetterListViewItem lvI = new BetterListViewItem();

            lvI.Text = $"Untitled{tabs}.go";
            lvI.Font = new Font("Gordita", 10);
            lvI.ForeColor = Color.White;
            lvI.Image = Properties.Resources.small;

            lvExplorer.Items.Add(lvI);

            selectedTB = newTxt;


        }

        private void guna2ImageButton7_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Creating projects is currently not supported yet!", "Unsupported feature", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnNewFile_Click(object sender, EventArgs e)
        {
            guna2ImageButton6.PerformClick();
        }

        private void btnON_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.autoSave = true;
            Properties.Settings.Default.Save();
        }

        private void btnOFF_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.autoSave = false;
            Properties.Settings.Default.Save();
        }

        string savePath;

        private void btnSave_Click(object sender, EventArgs e)
        {

            if (savePath == null)
            {

                SaveFileDialog sfd = new SaveFileDialog();

                sfd.Title = "Save changes";
                sfd.DefaultExt = "go";
                sfd.Filter = "Go File (*.go)|*.go|Dragon+ File (*.dp)|*.dp|Dragonfly File (*.df)|*.df|All files (*.*)|*.*";

                if (sfd.ShowDialog() == DialogResult.OK)
                {

                    savePath = sfd.FileName;

                    File.WriteAllText(savePath, selectedTB.Text);

                    string path = Path.GetFileName(sfd.FileName);

                    string tabToRemove = "UntitledGO";
                    for (int i = 0; i < guna2TabControl1.TabPages.Count; i++)
                    {
                        if (guna2TabControl1.TabPages[i].Name.Equals(tabToRemove, StringComparison.OrdinalIgnoreCase))
                        {
                            guna2TabControl1.TabPages.RemoveAt(i);
                            break;
                        }
                    }

                    deleteItemFromListBox("Untitled.go", lvExplorer);


                    string data = File.ReadAllText(sfd.FileName);

                    TabPage tb = new TabPage();

                    tb.Text = path;
                    tb.ImageIndex = 0;
                    tb.BackColor = Color.FromArgb(6, 26, 64);

                    FastColoredTextBox newTxt = new FastColoredTextBox();
                    newTxt.BackColor = tbCode.BackColor;
                    newTxt.ForeColor = tbCode.ForeColor;
                    newTxt.Language = tbCode.Language;
                    newTxt.TextChanged += tbCode_TextChanged;
                    newTxt.KeyDown += tbCode_KeyDown;
                    newTxt.Font = tbCode.Font;
                    newTxt.Anchor = tbCode.Anchor;
                    newTxt.Size = tbCode.Size;
                    newTxt.Location = tbCode.Location;
                    newTxt.AutoCompleteBrackets = true;
                    newTxt.AutoCompleteBracketsList = tbCode.AutoCompleteBracketsList;
                    newTxt.AutoIndentCharsPatterns = tbCode.AutoIndentCharsPatterns;
                    newTxt.CaretColor = tbCode.CaretColor;
                    newTxt.CurrentLineColor = tbCode.CurrentLineColor;
                    newTxt.DisabledColor = tbCode.DisabledColor;
                    newTxt.HighlightingRangeType = tbCode.HighlightingRangeType;
                    newTxt.IndentBackColor = tbCode.IndentBackColor;
                    newTxt.SelectionColor = tbCode.SelectionColor;
                    newTxt.ShowScrollBars = false;
                    newTxt.ShowFoldingLines = true;
                    newTxt.PaddingBackColor = tbCode.PaddingBackColor;
                    newTxt.LineNumberColor = tbCode.LineNumberColor;
                    newTxt.ServiceLinesColor = tbCode.ServiceLinesColor;
                    newTxt.AutoScrollMinSize = tbCode.AutoScrollMinSize;
                    newTxt.LeftPadding = tbCode.LeftPadding;
                    newTxt.Paddings = tbCode.Paddings;
                    newTxt.Text = data;

                    DocumentMap dcm = new DocumentMap();
                    dcm.Target = newTxt;
                    dcm.Text = String.Empty;
                    dcm.Font = new Font("Gordita", 9);
                    dcm.ForeColor = Color.White;
                    dcm.BackColor = Color.FromArgb(13, 32, 64);
                    dcm.Size = documentMap1.Size;
                    dcm.Location = documentMap1.Location;
                    dcm.Anchor = documentMap1.Anchor;

                    tb.Controls.Add(dcm);

                    tb.Controls.Add(newTxt);

                    guna2TabControl1.TabPages.Add(tb);

                    BetterListViewItem lvI = new BetterListViewItem();

                    lvI.Text = path;
                    lvI.Font = new Font("Gordita", 10);
                    lvI.ForeColor = Color.White;
                    lvI.Image = Properties.Resources.small;

                    lvExplorer.Items.Add(lvI);

                    selectedTB = newTxt;

                }

            }
            else
            {

                File.WriteAllText(savePath, selectedTB.Text);

            }

            guna2TabControl1.SelectedTab.Text.Replace("*", "");

        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.Title = "Save changes";
            sfd.DefaultExt = "go";
            sfd.Filter = "Go File (*.go)|*.go|Dragon+ File (*.dp)|*.dp|Dragonfly File (*.df)|*.df|All files (*.*)|*.*";

            if (sfd.ShowDialog() == DialogResult.OK)
            {

                savePath = sfd.FileName;

                File.WriteAllText(savePath, selectedTB.Text);

                string path = Path.GetFileName(sfd.FileName);

                string tabToRemove = "UntitledGO";
                for (int i = 0; i < guna2TabControl1.TabPages.Count; i++)
                {
                    if (guna2TabControl1.TabPages[i].Name.Equals(tabToRemove, StringComparison.OrdinalIgnoreCase))
                    {
                        guna2TabControl1.TabPages.RemoveAt(i);
                        break;
                    }
                }

                deleteItemFromListBox("Untitled.go", lvExplorer);


                string data = File.ReadAllText(sfd.FileName);

                TabPage tb = new TabPage();

                tb.Text = path;
                tb.ImageIndex = 0;
                tb.BackColor = Color.FromArgb(6, 26, 64);

                FastColoredTextBox newTxt = new FastColoredTextBox();
                newTxt.BackColor = tbCode.BackColor;
                newTxt.ForeColor = tbCode.ForeColor;
                newTxt.Language = tbCode.Language;
                newTxt.TextChanged += tbCode_TextChanged;
                newTxt.KeyDown += tbCode_KeyDown;
                newTxt.Font = tbCode.Font;
                newTxt.Anchor = tbCode.Anchor;
                newTxt.Size = tbCode.Size;
                newTxt.Location = tbCode.Location;
                newTxt.AutoCompleteBrackets = true;
                newTxt.AutoCompleteBracketsList = tbCode.AutoCompleteBracketsList;
                newTxt.AutoIndentCharsPatterns = tbCode.AutoIndentCharsPatterns;
                newTxt.CaretColor = tbCode.CaretColor;
                newTxt.CurrentLineColor = tbCode.CurrentLineColor;
                newTxt.DisabledColor = tbCode.DisabledColor;
                newTxt.HighlightingRangeType = tbCode.HighlightingRangeType;
                newTxt.IndentBackColor = tbCode.IndentBackColor;
                newTxt.SelectionColor = tbCode.SelectionColor;
                newTxt.ShowScrollBars = false;
                newTxt.ShowFoldingLines = true;
                newTxt.PaddingBackColor = tbCode.PaddingBackColor;
                newTxt.LineNumberColor = tbCode.LineNumberColor;
                newTxt.ServiceLinesColor = tbCode.ServiceLinesColor;
                newTxt.AutoScrollMinSize = tbCode.AutoScrollMinSize;
                newTxt.LeftPadding = tbCode.LeftPadding;
                newTxt.Paddings = tbCode.Paddings;
                newTxt.Text = data;

                DocumentMap dcm = new DocumentMap();
                dcm.Target = newTxt;
                dcm.Text = String.Empty;
                dcm.Font = new Font("Gordita", 9);
                dcm.ForeColor = Color.White;
                dcm.BackColor = Color.FromArgb(13, 32, 64);
                dcm.Size = documentMap1.Size;
                dcm.Location = documentMap1.Location;
                dcm.Anchor = documentMap1.Anchor;

                tb.Controls.Add(dcm);

                tb.Controls.Add(newTxt);

                guna2TabControl1.TabPages.Add(tb);

                BetterListViewItem lvI = new BetterListViewItem();

                lvI.Text = path;
                lvI.Font = new Font("Gordita", 10);
                lvI.ForeColor = Color.White;
                lvI.Image = Properties.Resources.small;

                lvExplorer.Items.Add(lvI);

                selectedTB = newTxt;

                guna2TabControl1.TabPages.Remove(guna2TabControl1.SelectedTab);

                guna2TabControl1.SelectedTab.Text.Replace("*", "");

            }
        }

        void deleteItemFromListBox(string stringToDelete, BetterListView listBoxToDeleteItem)
        {
            for (int i = 0; i < listBoxToDeleteItem.Items.Count; i++)
            {
                if (stringToDelete == listBoxToDeleteItem.Items[i].Text)
                {
                    listBoxToDeleteItem.Items[i].Remove();
                }
            }
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {

            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Title = "Open File";
            ofd.DefaultExt = "go";
            ofd.Filter = "Go File (*.go)|*.go|Dragon+ File (*.dp)|*.dp|Dragonfly File (*.df)|*.df|All files (*.*)|*.*";

            if (ofd.ShowDialog() == DialogResult.OK)
            {

                string path = Path.GetFileName(ofd.FileName);

                string tabToRemove = "UntitledGO";
                for (int i = 0; i < guna2TabControl1.TabPages.Count; i++)
                {
                    if (guna2TabControl1.TabPages[i].Name.Equals(tabToRemove, StringComparison.OrdinalIgnoreCase))
                    {
                        guna2TabControl1.TabPages.RemoveAt(i);
                        break;
                    }
                }

                deleteItemFromListBox("Untitled.go", lvExplorer);


                string data = File.ReadAllText(ofd.FileName);

                TabPage tb = new TabPage();

                tb.Text = path;
                tb.ImageIndex = 0;
                tb.BackColor = Color.FromArgb(6, 26, 64);

                FastColoredTextBox newTxt = new FastColoredTextBox();
                newTxt.BackColor = tbCode.BackColor;
                newTxt.ForeColor = tbCode.ForeColor;
                newTxt.Language = tbCode.Language;
                newTxt.TextChanged += tbCode_TextChanged;
                newTxt.KeyDown += tbCode_KeyDown;
                newTxt.Font = tbCode.Font;
                newTxt.Anchor = tbCode.Anchor;
                newTxt.Size = tbCode.Size;
                newTxt.Location = tbCode.Location;
                newTxt.AutoCompleteBrackets = true;
                newTxt.AutoCompleteBracketsList = tbCode.AutoCompleteBracketsList;
                newTxt.AutoIndentCharsPatterns = tbCode.AutoIndentCharsPatterns;
                newTxt.CaretColor = tbCode.CaretColor;
                newTxt.CurrentLineColor = tbCode.CurrentLineColor;
                newTxt.DisabledColor = tbCode.DisabledColor;
                newTxt.HighlightingRangeType = tbCode.HighlightingRangeType;
                newTxt.IndentBackColor = tbCode.IndentBackColor;
                newTxt.SelectionColor = tbCode.SelectionColor;
                newTxt.ShowScrollBars = false;
                newTxt.ShowFoldingLines = true;
                newTxt.PaddingBackColor = tbCode.PaddingBackColor;
                newTxt.LineNumberColor = tbCode.LineNumberColor;
                newTxt.ServiceLinesColor = tbCode.ServiceLinesColor;
                newTxt.AutoScrollMinSize = tbCode.AutoScrollMinSize;
                newTxt.LeftPadding = tbCode.LeftPadding;
                newTxt.Paddings = tbCode.Paddings;
                newTxt.Text = data;

                DocumentMap dcm = new DocumentMap();
                dcm.Target = newTxt;
                dcm.Text = String.Empty;
                dcm.Font = new Font("Gordita", 9);
                dcm.ForeColor = Color.White;
                dcm.BackColor = Color.FromArgb(13, 32, 64);
                dcm.Size = documentMap1.Size;
                dcm.Location = documentMap1.Location;
                dcm.Anchor = documentMap1.Anchor;

                tb.Controls.Add(dcm);

                tb.Controls.Add(newTxt);

                guna2TabControl1.TabPages.Add(tb);

                BetterListViewItem lvI = new BetterListViewItem();

                lvI.Text = path;
                lvI.Font = new Font("Gordita", 10);
                lvI.ForeColor = Color.White;
                lvI.Image = Properties.Resources.small;

                lvExplorer.Items.Add(lvI);

                selectedTB = newTxt;

                guna2TabControl1.SelectTab(tb);

            }

        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            selectedTB.Undo();
        }

        private void btnRedo_Click(object sender, EventArgs e)
        {
            selectedTB.Redo();
        }

        private void btnCut_Click(object sender, EventArgs e)
        {
            selectedTB.Cut();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            selectedTB.Copy();
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            selectedTB.Paste();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            selectedTB.ShowFindDialog();
        }

        private void btnReplace_Click(object sender, EventArgs e)
        {
            selectedTB.ShowReplaceDialog();
        }

        private void btnDiscord_Click(object sender, EventArgs e)
        {
            Process.Start("https://discord.gg/vbn4gcdqjb");
        }

        private void btnWebsite_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/mixrify/DragonPlus");
        }

        [DllImport("user32")]
        public static extern int GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        public static extern int GetKeyboardState(byte[] keystate);
        private void tbCode_KeyDown(object sender, KeyEventArgs e)
        {

            byte[] keys = new byte[256];

            GetKeyboardState(keys);

            if ((keys[(int)Keys.ControlKey] & (keys[(int)Keys.ControlKey] & keys[(int)Keys.S] & 128)) == 128)
            {

                btnSave.PerformClick();

            }


        }

        private void label16_Click(object sender, EventArgs e)
        {

            DialogResult d = MessageBox.Show("Are you sure you want to clear all the tabs?", "Dragon+", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (d == DialogResult.Yes)
            {

                guna2TabControl1.TabPages.Clear();
                lvExplorer.Items.Clear();

                TabPage tb = new TabPage();

                tb.Text = "Untitled.go";
                tb.ImageIndex = 0;
                tb.BackColor = Color.FromArgb(6, 26, 64);

                FastColoredTextBox newTxt = new FastColoredTextBox();
                newTxt.BackColor = tbCode.BackColor;
                newTxt.ForeColor = tbCode.ForeColor;
                newTxt.Language = tbCode.Language;
                newTxt.TextChanged += tbCode_TextChanged;
                newTxt.KeyDown += tbCode_KeyDown;
                newTxt.Font = tbCode.Font;
                newTxt.Anchor = tbCode.Anchor;
                newTxt.Size = tbCode.Size;
                newTxt.Location = tbCode.Location;
                newTxt.AutoCompleteBrackets = true;
                newTxt.AutoCompleteBracketsList = tbCode.AutoCompleteBracketsList;
                newTxt.AutoIndentCharsPatterns = tbCode.AutoIndentCharsPatterns;
                newTxt.CaretColor = tbCode.CaretColor;
                newTxt.CurrentLineColor = tbCode.CurrentLineColor;
                newTxt.DisabledColor = tbCode.DisabledColor;
                newTxt.HighlightingRangeType = tbCode.HighlightingRangeType;
                newTxt.IndentBackColor = tbCode.IndentBackColor;
                newTxt.SelectionColor = tbCode.SelectionColor;
                newTxt.ShowScrollBars = false;
                newTxt.ShowFoldingLines = true;
                newTxt.PaddingBackColor = tbCode.PaddingBackColor;
                newTxt.LineNumberColor = tbCode.LineNumberColor;
                newTxt.ServiceLinesColor = tbCode.ServiceLinesColor;
                newTxt.AutoScrollMinSize = tbCode.AutoScrollMinSize;
                newTxt.LeftPadding = tbCode.LeftPadding;
                newTxt.Paddings = tbCode.Paddings;

                DocumentMap dcm = new DocumentMap();
                dcm.Target = newTxt;
                dcm.Text = String.Empty;
                dcm.Font = new Font("Gordita", 9);
                dcm.ForeColor = Color.White;
                dcm.BackColor = Color.FromArgb(13, 32, 64);
                dcm.Size = documentMap1.Size;
                dcm.Location = documentMap1.Location;
                dcm.Anchor = documentMap1.Anchor;

                tb.Controls.Add(dcm);

                tb.Controls.Add(newTxt);

                guna2TabControl1.TabPages.Add(tb);

                BetterListViewItem lvI = new BetterListViewItem();

                lvI.Text = "Untitled.go";
                lvI.Font = new Font("Gordita", 10);
                lvI.ForeColor = Color.White;
                lvI.Image = Properties.Resources.small;

                lvExplorer.Items.Add(lvI);

                selectedTB = newTxt;

            }

        }
    }
}
