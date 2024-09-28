namespace CustomCommandBarCreator
{
    partial class SetupSettings
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
            this.cb_openURL = new System.Windows.Forms.CheckBox();
            this.txt_url = new System.Windows.Forms.TextBox();
            this.panel_url = new System.Windows.Forms.Panel();
            this.txt_setupFolder = new System.Windows.Forms.TextBox();
            this.btn_searchFolder = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btn_logo = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_ok = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btn_save = new System.Windows.Forms.Button();
            this.btn_load = new System.Windows.Forms.Button();
            this.btn_cancel = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lba_readmePath = new System.Windows.Forms.LinkLabel();
            this.button1 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.lba_icon = new System.Windows.Forms.LinkLabel();
            this.btn_icon = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.txt_Author = new System.Windows.Forms.TextBox();
            this.panel7 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.txt_email = new System.Windows.Forms.TextBox();
            this.panel8 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.btn_configurations = new System.Windows.Forms.Button();
            this.txt_configurations = new System.Windows.Forms.TextBox();
            this.panel_url.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel8.SuspendLayout();
            this.SuspendLayout();
            // 
            // cb_openURL
            // 
            this.cb_openURL.AutoSize = true;
            this.cb_openURL.Location = new System.Drawing.Point(9, 12);
            this.cb_openURL.Name = "cb_openURL";
            this.cb_openURL.Size = new System.Drawing.Size(77, 17);
            this.cb_openURL.TabIndex = 0;
            this.cb_openURL.Text = "Open URL";
            this.cb_openURL.UseVisualStyleBackColor = true;
            this.cb_openURL.Click += new System.EventHandler(this.cb_openURL_Click);
            // 
            // txt_url
            // 
            this.txt_url.Location = new System.Drawing.Point(93, 12);
            this.txt_url.Name = "txt_url";
            this.txt_url.Size = new System.Drawing.Size(224, 20);
            this.txt_url.TabIndex = 1;
            this.txt_url.TextChanged += new System.EventHandler(this.txt_url_TextChanged);
            // 
            // panel_url
            // 
            this.panel_url.BackColor = System.Drawing.Color.White;
            this.panel_url.Controls.Add(this.txt_url);
            this.panel_url.Controls.Add(this.cb_openURL);
            this.panel_url.Location = new System.Drawing.Point(12, 142);
            this.panel_url.Name = "panel_url";
            this.panel_url.Size = new System.Drawing.Size(328, 45);
            this.panel_url.TabIndex = 2;
            this.panel_url.Visible = false;
            // 
            // txt_setupFolder
            // 
            this.txt_setupFolder.Location = new System.Drawing.Point(6, 24);
            this.txt_setupFolder.Name = "txt_setupFolder";
            this.txt_setupFolder.Size = new System.Drawing.Size(224, 20);
            this.txt_setupFolder.TabIndex = 0;
            // 
            // btn_searchFolder
            // 
            this.btn_searchFolder.Location = new System.Drawing.Point(247, 22);
            this.btn_searchFolder.Name = "btn_searchFolder";
            this.btn_searchFolder.Size = new System.Drawing.Size(70, 23);
            this.btn_searchFolder.TabIndex = 1;
            this.btn_searchFolder.Text = "Browser";
            this.btn_searchFolder.UseVisualStyleBackColor = true;
            this.btn_searchFolder.Click += new System.EventHandler(this.btn_searchFolder_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btn_searchFolder);
            this.panel1.Controls.Add(this.txt_setupFolder);
            this.panel1.Location = new System.Drawing.Point(12, 193);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(328, 55);
            this.panel1.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(175, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Select a folder to save yours setups";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.pictureBox1);
            this.panel2.Controls.Add(this.btn_logo);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Location = new System.Drawing.Point(12, 255);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(328, 215);
            this.panel2.TabIndex = 4;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBox1.Location = new System.Drawing.Point(18, 39);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(160, 160);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // btn_logo
            // 
            this.btn_logo.Location = new System.Drawing.Point(247, 176);
            this.btn_logo.Name = "btn_logo";
            this.btn_logo.Size = new System.Drawing.Size(70, 23);
            this.btn_logo.TabIndex = 0;
            this.btn_logo.Text = "Browser";
            this.btn_logo.UseVisualStyleBackColor = true;
            this.btn_logo.Click += new System.EventHandler(this.btn_logo_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Select a logo (160x160)";
            // 
            // btn_ok
            // 
            this.btn_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_ok.Location = new System.Drawing.Point(170, 7);
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.Size = new System.Drawing.Size(70, 23);
            this.btn_ok.TabIndex = 2;
            this.btn_ok.Text = "OK";
            this.btn_ok.UseVisualStyleBackColor = true;
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btn_save);
            this.panel3.Controls.Add(this.btn_load);
            this.panel3.Controls.Add(this.btn_cancel);
            this.panel3.Controls.Add(this.btn_ok);
            this.panel3.Location = new System.Drawing.Point(12, 610);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(330, 33);
            this.panel3.TabIndex = 7;
            // 
            // btn_save
            // 
            this.btn_save.Location = new System.Drawing.Point(93, 7);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(70, 23);
            this.btn_save.TabIndex = 1;
            this.btn_save.Text = "Save";
            this.btn_save.UseVisualStyleBackColor = true;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // btn_load
            // 
            this.btn_load.Location = new System.Drawing.Point(16, 7);
            this.btn_load.Name = "btn_load";
            this.btn_load.Size = new System.Drawing.Size(70, 23);
            this.btn_load.TabIndex = 0;
            this.btn_load.Text = "Load";
            this.btn_load.UseVisualStyleBackColor = true;
            this.btn_load.Click += new System.EventHandler(this.btn_load_Click);
            // 
            // btn_cancel
            // 
            this.btn_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_cancel.Location = new System.Drawing.Point(247, 7);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(70, 23);
            this.btn_cancel.TabIndex = 3;
            this.btn_cancel.Text = "Cancel";
            this.btn_cancel.UseVisualStyleBackColor = true;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.White;
            this.panel4.Controls.Add(this.lba_readmePath);
            this.panel4.Controls.Add(this.button1);
            this.panel4.Controls.Add(this.label3);
            this.panel4.Location = new System.Drawing.Point(14, 476);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(328, 61);
            this.panel4.TabIndex = 5;
            // 
            // lba_readmePath
            // 
            this.lba_readmePath.Location = new System.Drawing.Point(4, 30);
            this.lba_readmePath.Name = "lba_readmePath";
            this.lba_readmePath.Size = new System.Drawing.Size(230, 13);
            this.lba_readmePath.TabIndex = 4;
            this.lba_readmePath.Click += new System.EventHandler(this.lba_readmePath_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(245, 25);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(70, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Browser";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btn_readme_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(140, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Select ReadME/License file";
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.White;
            this.panel5.Controls.Add(this.lba_icon);
            this.panel5.Controls.Add(this.btn_icon);
            this.panel5.Controls.Add(this.label4);
            this.panel5.Location = new System.Drawing.Point(14, 543);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(328, 61);
            this.panel5.TabIndex = 6;
            // 
            // lba_icon
            // 
            this.lba_icon.Location = new System.Drawing.Point(4, 30);
            this.lba_icon.Name = "lba_icon";
            this.lba_icon.Size = new System.Drawing.Size(230, 13);
            this.lba_icon.TabIndex = 4;
            this.lba_icon.Click += new System.EventHandler(this.lba_readmePath_Click);
            // 
            // btn_icon
            // 
            this.btn_icon.Location = new System.Drawing.Point(245, 25);
            this.btn_icon.Name = "btn_icon";
            this.btn_icon.Size = new System.Drawing.Size(70, 23);
            this.btn_icon.TabIndex = 1;
            this.btn_icon.Text = "Browser";
            this.btn_icon.UseVisualStyleBackColor = true;
            this.btn_icon.Click += new System.EventHandler(this.btn_icon_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 4);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Select setup icon";
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.White;
            this.panel6.Controls.Add(this.label5);
            this.panel6.Controls.Add(this.txt_Author);
            this.panel6.Location = new System.Drawing.Point(14, 18);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(328, 55);
            this.panel6.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 8);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Author";
            // 
            // txt_Author
            // 
            this.txt_Author.Location = new System.Drawing.Point(6, 24);
            this.txt_Author.Name = "txt_Author";
            this.txt_Author.Size = new System.Drawing.Size(309, 20);
            this.txt_Author.TabIndex = 1;
            this.txt_Author.TextChanged += new System.EventHandler(this.txt_Author_TextChanged);
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.White;
            this.panel7.Controls.Add(this.label6);
            this.panel7.Controls.Add(this.txt_email);
            this.panel7.Location = new System.Drawing.Point(14, 79);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(328, 55);
            this.panel7.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 8);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "Email";
            // 
            // txt_email
            // 
            this.txt_email.Location = new System.Drawing.Point(6, 24);
            this.txt_email.Name = "txt_email";
            this.txt_email.Size = new System.Drawing.Size(309, 20);
            this.txt_email.TabIndex = 0;
            this.txt_email.TextChanged += new System.EventHandler(this.txt_email_TextChanged);
            // 
            // panel8
            // 
            this.panel8.BackColor = System.Drawing.Color.White;
            this.panel8.Controls.Add(this.label7);
            this.panel8.Controls.Add(this.btn_configurations);
            this.panel8.Controls.Add(this.txt_configurations);
            this.panel8.Location = new System.Drawing.Point(348, 18);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(328, 230);
            this.panel8.TabIndex = 8;
            this.panel8.Visible = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 8);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(185, 13);
            this.label7.TabIndex = 4;
            this.label7.Text = "Will create a setup for follow versions:";
            // 
            // btn_configurations
            // 
            this.btn_configurations.Location = new System.Drawing.Point(158, 178);
            this.btn_configurations.Name = "btn_configurations";
            this.btn_configurations.Size = new System.Drawing.Size(156, 44);
            this.btn_configurations.TabIndex = 1;
            this.btn_configurations.Text = "Open Configurations Manager";
            this.btn_configurations.UseVisualStyleBackColor = true;
            this.btn_configurations.Click += new System.EventHandler(this.btn_configurations_Click);
            // 
            // txt_configurations
            // 
            this.txt_configurations.Location = new System.Drawing.Point(13, 24);
            this.txt_configurations.Multiline = true;
            this.txt_configurations.Name = "txt_configurations";
            this.txt_configurations.Size = new System.Drawing.Size(301, 148);
            this.txt_configurations.TabIndex = 0;
            // 
            // SetupSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(702, 670);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel7);
            this.Controls.Add(this.panel8);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel_url);
            this.Name = "SetupSettings";
            this.Text = "Setup Settings";
            this.panel_url.ResumeLayout(false);
            this.panel_url.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox cb_openURL;
        private System.Windows.Forms.TextBox txt_url;
        private System.Windows.Forms.Panel panel_url;
        private System.Windows.Forms.TextBox txt_setupFolder;
        private System.Windows.Forms.Button btn_searchFolder;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btn_logo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn_ok;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btn_cancel;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.LinkLabel lba_readmePath;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.LinkLabel lba_icon;
        private System.Windows.Forms.Button btn_icon;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txt_Author;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txt_email;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.TextBox txt_configurations;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btn_configurations;
        private System.Windows.Forms.Button btn_load;
        private System.Windows.Forms.Button btn_save;
    }
}