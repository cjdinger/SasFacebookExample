namespace SAS.FbFriends
{
    partial class FbLoginBrowser
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
            this.wbFacebookSite = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // wbFacebookSite
            // 
            this.wbFacebookSite.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wbFacebookSite.Location = new System.Drawing.Point(0, 0);
            this.wbFacebookSite.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbFacebookSite.Name = "wbFacebookSite";
            this.wbFacebookSite.Size = new System.Drawing.Size(494, 326);
            this.wbFacebookSite.TabIndex = 0;
            // 
            // FbLoginBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 326);
            this.Controls.Add(this.wbFacebookSite);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FbLoginBrowser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Connect to Facebook";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser wbFacebookSite;
    }
}