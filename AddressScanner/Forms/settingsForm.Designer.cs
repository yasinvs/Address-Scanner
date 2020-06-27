namespace AddressScanner.Forms
{
    partial class settingsForm
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
            this.tcSettings = new System.Windows.Forms.TabControl();
            this.tcbApplication = new System.Windows.Forms.TabPage();
            this.cbCheckforUpdates = new System.Windows.Forms.CheckBox();
            this.tcbTest = new System.Windows.Forms.TabPage();
            this.txtTimeout = new System.Windows.Forms.TextBox();
            this.lblPingTimeout = new System.Windows.Forms.Label();
            this.cbAutoScroll = new System.Windows.Forms.CheckBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnDefault = new System.Windows.Forms.Button();
            this.tcSettings.SuspendLayout();
            this.tcbApplication.SuspendLayout();
            this.tcbTest.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcSettings
            // 
            this.tcSettings.Controls.Add(this.tcbApplication);
            this.tcSettings.Controls.Add(this.tcbTest);
            this.tcSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.tcSettings.Location = new System.Drawing.Point(0, 0);
            this.tcSettings.Name = "tcSettings";
            this.tcSettings.SelectedIndex = 0;
            this.tcSettings.Size = new System.Drawing.Size(389, 185);
            this.tcSettings.TabIndex = 0;
            // 
            // tcbApplication
            // 
            this.tcbApplication.Controls.Add(this.cbCheckforUpdates);
            this.tcbApplication.Location = new System.Drawing.Point(4, 22);
            this.tcbApplication.Name = "tcbApplication";
            this.tcbApplication.Padding = new System.Windows.Forms.Padding(3);
            this.tcbApplication.Size = new System.Drawing.Size(381, 159);
            this.tcbApplication.TabIndex = 1;
            this.tcbApplication.Text = "Application";
            this.tcbApplication.UseVisualStyleBackColor = true;
            // 
            // cbCheckforUpdates
            // 
            this.cbCheckforUpdates.AutoSize = true;
            this.cbCheckforUpdates.Location = new System.Drawing.Point(8, 6);
            this.cbCheckforUpdates.Name = "cbCheckforUpdates";
            this.cbCheckforUpdates.Size = new System.Drawing.Size(173, 17);
            this.cbCheckforUpdates.TabIndex = 0;
            this.cbCheckforUpdates.Text = "Check for Update(s) on Startup";
            this.cbCheckforUpdates.UseVisualStyleBackColor = true;
            // 
            // tcbTest
            // 
            this.tcbTest.Controls.Add(this.txtTimeout);
            this.tcbTest.Controls.Add(this.lblPingTimeout);
            this.tcbTest.Controls.Add(this.cbAutoScroll);
            this.tcbTest.Location = new System.Drawing.Point(4, 22);
            this.tcbTest.Name = "tcbTest";
            this.tcbTest.Padding = new System.Windows.Forms.Padding(3);
            this.tcbTest.Size = new System.Drawing.Size(381, 159);
            this.tcbTest.TabIndex = 0;
            this.tcbTest.Text = "Test";
            this.tcbTest.UseVisualStyleBackColor = true;
            // 
            // txtTimeout
            // 
            this.txtTimeout.Location = new System.Drawing.Point(163, 29);
            this.txtTimeout.MaxLength = 5;
            this.txtTimeout.Name = "txtTimeout";
            this.txtTimeout.Size = new System.Drawing.Size(100, 20);
            this.txtTimeout.TabIndex = 2;
            this.txtTimeout.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtTimeout_KeyPress);
            // 
            // lblPingTimeout
            // 
            this.lblPingTimeout.AutoSize = true;
            this.lblPingTimeout.Location = new System.Drawing.Point(5, 32);
            this.lblPingTimeout.Name = "lblPingTimeout";
            this.lblPingTimeout.Size = new System.Drawing.Size(152, 13);
            this.lblPingTimeout.TabIndex = 1;
            this.lblPingTimeout.Text = "Ping Timeout (default : 2000) : ";
            // 
            // cbAutoScroll
            // 
            this.cbAutoScroll.AutoSize = true;
            this.cbAutoScroll.Location = new System.Drawing.Point(8, 6);
            this.cbAutoScroll.Name = "cbAutoScroll";
            this.cbAutoScroll.Size = new System.Drawing.Size(162, 17);
            this.cbAutoScroll.TabIndex = 0;
            this.cbAutoScroll.Text = "Enable AutoScroll on Startup";
            this.cbAutoScroll.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(302, 219);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(221, 219);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(12, 203);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(188, 26);
            this.label1.TabIndex = 3;
            this.label1.Text = "Restart the application for the settings \r\nto take effect";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnDefault
            // 
            this.btnDefault.Location = new System.Drawing.Point(221, 190);
            this.btnDefault.Name = "btnDefault";
            this.btnDefault.Size = new System.Drawing.Size(156, 23);
            this.btnDefault.TabIndex = 4;
            this.btnDefault.Text = "Default";
            this.btnDefault.UseVisualStyleBackColor = true;
            this.btnDefault.Click += new System.EventHandler(this.btnDefault_Click);
            // 
            // settingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(389, 252);
            this.Controls.Add(this.btnDefault);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.tcSettings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "settingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.formSettings_Load);
            this.tcSettings.ResumeLayout(false);
            this.tcbApplication.ResumeLayout(false);
            this.tcbApplication.PerformLayout();
            this.tcbTest.ResumeLayout(false);
            this.tcbTest.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tcSettings;
        private System.Windows.Forms.TabPage tcbTest;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TabPage tcbApplication;
        private System.Windows.Forms.CheckBox cbCheckforUpdates;
        private System.Windows.Forms.TextBox txtTimeout;
        private System.Windows.Forms.Label lblPingTimeout;
        private System.Windows.Forms.CheckBox cbAutoScroll;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnDefault;
    }
}