﻿using System;
using System.Drawing;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Paragoniarz
{
    public partial class Form2 : Form
    {
        private UserControl currentControl = null;
        private UploadFileControl uploadFileControl;
        private YourFilesControl yourFilesControl;
        private FindFileControl findFileControl;

        public Form2()
        {
            InitializeComponent();

            uploadFileControl = new UploadFileControl();
            yourFilesControl = new YourFilesControl();
            findFileControl = new FindFileControl(UserSession.UserId);
            ShowControl(uploadFileControl);

            WindowHelper.SetWindowRoundCorners(this, 20);
            WindowHelper.EnableWindowDragging(panel1, this);

            pnlNav.Height = sendFile.Height;
            pnlNav.Top = sendFile.Top;
            pnlNav.Left = sendFile.Left;
            sendFile.BackColor = Color.FromArgb(46, 51, 73);

            if (!string.IsNullOrEmpty(UserSession.Username))
            {
                label1.Text = "Witaj " + UserSession.Username + "!";
            }
            else
            {
                label1.Text = "Witaj nieznajomy!";
            }
        }

        private void ShowControl(UserControl control)
        {
            if (currentControl != null)
            {
                panel4.Controls.Remove(currentControl);
            }

            panel4.Controls.Add(control);
            control.Dock = DockStyle.Fill;
            currentControl = control;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void SetNavigationPanel(Control clickedButton)
        {
            pnlNav.Height = clickedButton.Height;
            pnlNav.Top = clickedButton.Top;
            pnlNav.Left = clickedButton.Left;
            clickedButton.BackColor = Color.FromArgb(46, 51, 73);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SetNavigationPanel(sendFile);
            ShowControl(uploadFileControl);
            label3.Text = "Wyślij plik";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SetNavigationPanel(yoursFile);
            ShowControl(findFileControl);
            label3.Text = "Twoje pliki";
            sendFile.BackColor = Color.FromArgb(24, 30, 54);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SetNavigationPanel(findFile);
            ShowControl(yourFilesControl);
            label3.Text = "Znajdź plik";
            sendFile.BackColor = Color.FromArgb(24, 30, 54);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SetNavigationPanel(button6);
            Form1 form1 = new Form1();
            form1.StartPosition = FormStartPosition.Manual;
            form1.Location = this.Location;
            form1.Show();
            this.Hide();
        }

        private void SetButtonDefaultColor(Button button)
        {
            button.BackColor = Color.FromArgb(24, 30, 54);
        }

        private void button3_Leave(object sender, EventArgs e)
        {
            SetButtonDefaultColor(sendFile);
        }

        private void button4_Leave(object sender, EventArgs e)
        {
            SetButtonDefaultColor(yoursFile);
        }

        private void button5_Leave(object sender, EventArgs e)
        {
            SetButtonDefaultColor(findFile);
        }

        private void button6_Leave(object sender, EventArgs e)
        {
            SetButtonDefaultColor(button6);
        }
    }
}
