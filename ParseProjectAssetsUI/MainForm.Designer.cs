

namespace ParseProjectAssetsUI
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            btnSelectFile = new Button();
            txtFileName = new TextBox();
            lblSolutionPath = new Label();
            btnParse = new Button();
            lblPackageName = new Label();
            txtPackageName = new ComboBox();
            chkVert = new CheckBox();
            chkGroupTop = new CheckBox();
            btnCopyMermaid = new Button();
            button1 = new Button();
            ((System.ComponentModel.ISupportInitialize)webView21).BeginInit();
            SuspendLayout();
            // 
            // webView21
            // 
            webView21.AllowExternalDrop = true;
            webView21.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            webView21.CreationProperties = null;
            webView21.DefaultBackgroundColor = Color.WhiteSmoke;
            webView21.Location = new Point(-1, 74);
            webView21.Name = "webView21";
            webView21.Size = new Size(1248, 335);
            webView21.TabIndex = 5;
            webView21.ZoomFactor = 1D;
            // 
            // btnSelectFile
            // 
            btnSelectFile.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSelectFile.Location = new Point(1201, 8);
            btnSelectFile.Name = "btnSelectFile";
            btnSelectFile.Size = new Size(34, 23);
            btnSelectFile.TabIndex = 2;
            btnSelectFile.Text = "...";
            btnSelectFile.UseVisualStyleBackColor = true;
            btnSelectFile.Click += btnSelectFile_Click;
            // 
            // txtFileName
            // 
            txtFileName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtFileName.Location = new Point(99, 9);
            txtFileName.Name = "txtFileName";
            txtFileName.Size = new Size(1096, 23);
            txtFileName.TabIndex = 1;
            // 
            // lblSolutionPath
            // 
            lblSolutionPath.AutoSize = true;
            lblSolutionPath.Location = new Point(12, 12);
            lblSolutionPath.Name = "lblSolutionPath";
            lblSolutionPath.Size = new Size(81, 15);
            lblSolutionPath.TabIndex = 3;
            lblSolutionPath.Text = "Solution Path:";
            // 
            // btnParse
            // 
            btnParse.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnParse.Location = new Point(972, 415);
            btnParse.Name = "btnParse";
            btnParse.Size = new Size(75, 23);
            btnParse.TabIndex = 6;
            btnParse.Text = "&Render";
            btnParse.UseVisualStyleBackColor = true;
            btnParse.Click += btnParse_Click;
            // 
            // lblPackageName
            // 
            lblPackageName.AutoSize = true;
            lblPackageName.Location = new Point(12, 41);
            lblPackageName.Name = "lblPackageName";
            lblPackageName.Size = new Size(89, 15);
            lblPackageName.TabIndex = 3;
            lblPackageName.Text = "Package Name:";
            // 
            // txtPackageName
            // 
            txtPackageName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtPackageName.FormattingEnabled = true;
            txtPackageName.Location = new Point(107, 38);
            txtPackageName.Name = "txtPackageName";
            txtPackageName.Size = new Size(940, 23);
            txtPackageName.TabIndex = 4;
            txtPackageName.SelectedValueChanged += txtPackageName_SelectedValueChanged;
            // 
            // chkVert
            // 
            chkVert.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chkVert.AutoSize = true;
            chkVert.Checked = true;
            chkVert.CheckState = CheckState.Checked;
            chkVert.Location = new Point(1053, 40);
            chkVert.Name = "chkVert";
            chkVert.Size = new Size(64, 19);
            chkVert.TabIndex = 7;
            chkVert.Text = "Vertical";
            chkVert.UseVisualStyleBackColor = true;
            chkVert.CheckStateChanged += chkVert_CheckStateChanged;
            // 
            // chkGroupTop
            // 
            chkGroupTop.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            chkGroupTop.AutoSize = true;
            chkGroupTop.Location = new Point(1123, 40);
            chkGroupTop.Name = "chkGroupTop";
            chkGroupTop.Size = new Size(112, 19);
            chkGroupTop.TabIndex = 8;
            chkGroupTop.Text = "Group Top Level";
            chkGroupTop.UseVisualStyleBackColor = true;
            chkGroupTop.CheckedChanged += chkGroupTop_CheckedChanged;
            // 
            // btnCopyMermaid
            // 
            btnCopyMermaid.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCopyMermaid.Location = new Point(1053, 415);
            btnCopyMermaid.Name = "btnCopyMermaid";
            btnCopyMermaid.Size = new Size(114, 23);
            btnCopyMermaid.TabIndex = 9;
            btnCopyMermaid.Text = "Copy Mermaid";
            btnCopyMermaid.UseVisualStyleBackColor = true;
            btnCopyMermaid.Click += btnCopyMermaid_Click;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button1.Location = new Point(1172, 415);
            button1.Name = "button1";
            button1.Size = new Size(63, 23);
            button1.TabIndex = 10;
            button1.Text = "E&xit";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1247, 450);
            Controls.Add(button1);
            Controls.Add(btnCopyMermaid);
            Controls.Add(chkGroupTop);
            Controls.Add(chkVert);
            Controls.Add(txtPackageName);
            Controls.Add(lblPackageName);
            Controls.Add(btnParse);
            Controls.Add(lblSolutionPath);
            Controls.Add(txtFileName);
            Controls.Add(btnSelectFile);
            Controls.Add(webView21);
            Name = "MainForm";
            Text = "Parse Project Assets File";
            ((System.ComponentModel.ISupportInitialize)webView21).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }


        #endregion

        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private Button btnSelectFile;
        private TextBox txtFileName;
        private Label lblSolutionPath;
        private Button btnParse;
        private Label lblPackageName;
        private ComboBox txtPackageName;
        private CheckBox chkVert;
        private CheckBox chkGroupTop;
        private Button btnCopyMermaid;
        private Button button1;
    }
}
