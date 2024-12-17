namespace Paragoniarz
{
    partial class UploadFileControl
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
            this.UDlabel = new System.Windows.Forms.Label();
            this.CDlabel = new System.Windows.Forms.Label();
            this.button7 = new System.Windows.Forms.Button();
            this.FSlabel = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.FNlabel = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // UDlabel
            // 
            this.UDlabel.AutoSize = true;
            this.UDlabel.ForeColor = System.Drawing.SystemColors.Highlight;
            this.UDlabel.Location = new System.Drawing.Point(383, 227);
            this.UDlabel.Name = "UDlabel";
            this.UDlabel.Size = new System.Drawing.Size(76, 13);
            this.UDlabel.TabIndex = 14;
            this.UDlabel.Text = "Data wgrania: ";
            // 
            // CDlabel
            // 
            this.CDlabel.AutoSize = true;
            this.CDlabel.ForeColor = System.Drawing.SystemColors.Highlight;
            this.CDlabel.Location = new System.Drawing.Point(383, 168);
            this.CDlabel.Name = "CDlabel";
            this.CDlabel.Size = new System.Drawing.Size(90, 13);
            this.CDlabel.TabIndex = 13;
            this.CDlabel.Text = "Data utworzenia: ";
            // 
            // button7
            // 
            this.button7.BackColor = System.Drawing.Color.Transparent;
            this.button7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button7.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.button7.ForeColor = System.Drawing.SystemColors.Highlight;
            this.button7.Location = new System.Drawing.Point(220, 289);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(119, 35);
            this.button7.TabIndex = 10;
            this.button7.Text = "Wyslij";
            this.button7.UseVisualStyleBackColor = false;
            // 
            // FSlabel
            // 
            this.FSlabel.AutoSize = true;
            this.FSlabel.ForeColor = System.Drawing.SystemColors.Highlight;
            this.FSlabel.Location = new System.Drawing.Point(383, 108);
            this.FSlabel.Name = "FSlabel";
            this.FSlabel.Size = new System.Drawing.Size(51, 13);
            this.FSlabel.TabIndex = 12;
            this.FSlabel.Text = "Rozmiar: ";
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.Transparent;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.button2.ForeColor = System.Drawing.SystemColors.Highlight;
            this.button2.Location = new System.Drawing.Point(51, 289);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(119, 35);
            this.button2.TabIndex = 9;
            this.button2.Text = "Usun";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // FNlabel
            // 
            this.FNlabel.AutoSize = true;
            this.FNlabel.ForeColor = System.Drawing.SystemColors.Highlight;
            this.FNlabel.Location = new System.Drawing.Point(383, 38);
            this.FNlabel.Name = "FNlabel";
            this.FNlabel.Size = new System.Drawing.Size(71, 13);
            this.FNlabel.TabIndex = 11;
            this.FNlabel.Text = "Nazwa pliku: ";
            // 
            // panel5
            // 
            this.panel5.AllowDrop = true;
            this.panel5.Controls.Add(this.pictureBox2);
            this.panel5.Location = new System.Drawing.Point(38, 32);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(324, 234);
            this.panel5.TabIndex = 8;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox2.Location = new System.Drawing.Point(0, 0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(324, 234);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 0;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.DragDrop += new System.Windows.Forms.DragEventHandler(this.pictureBox2_DragDrop);
            this.pictureBox2.DragEnter += new System.Windows.Forms.DragEventHandler(this.pictureBox2_DragEnter);
            this.pictureBox2.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox2_Paint_1);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.ForeColor = System.Drawing.SystemColors.Highlight;
            this.label1.Location = new System.Drawing.Point(266, 404);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(560, 86);
            this.label1.TabIndex = 15;
            this.label1.Text = "UploadFileCOntrol";
            // 
            // UploadFileControl
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(30)))), ((int)(((byte)(54)))));
            this.Controls.Add(this.label1);
            this.Controls.Add(this.UDlabel);
            this.Controls.Add(this.CDlabel);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.FSlabel);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.FNlabel);
            this.Controls.Add(this.panel5);
            this.Name = "UploadFileControl";
            this.Size = new System.Drawing.Size(1000, 600);
            this.panel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label UDlabel;
        private System.Windows.Forms.Label CDlabel;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Label FSlabel;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label FNlabel;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label1;
    }
}