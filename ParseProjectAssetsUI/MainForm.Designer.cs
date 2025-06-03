

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
            webView21.Size = new Size(801, 335);
            webView21.TabIndex = 5;
            webView21.ZoomFactor = 1D;
            // 
            // btnSelectFile
            // 
            btnSelectFile.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSelectFile.Location = new Point(754, 8);
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
            txtFileName.Size = new Size(649, 23);
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
            btnParse.Location = new Point(564, 415);
            btnParse.Name = "btnParse";
            btnParse.Size = new Size(75, 23);
            btnParse.TabIndex = 6;
            btnParse.Text = "&Parse";
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
            txtPackageName.Size = new Size(681, 23);
            txtPackageName.TabIndex = 4;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
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
    }
}
