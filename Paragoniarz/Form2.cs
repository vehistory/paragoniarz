using System;
using System.Drawing;
using System.Windows.Forms;

namespace Paragoniarz
{
    public partial class Form2 : Form
    {
        // Deklaracja kontrolek
        private UserControl currentControl = null;
        private UploadFileControl uploadFileControl;
        private YourFilesControl yourFilesControl;
        private FindFileControl findFileControl;




        public Form2(string username)
        {
            InitializeComponent();
           
            uploadFileControl = new UploadFileControl();
            yourFilesControl = new YourFilesControl();
            findFileControl = new FindFileControl();
            ShowControl(uploadFileControl);

            // Zaokrąglij rogi okna 
            WindowHelper.SetWindowRoundCorners(this,20);

            // Umożliw przesuwanie okna 
            WindowHelper.EnableWindowDragging(panel1,this);

            pnlNav.Height = sendFile.Height;
            pnlNav.Top = sendFile.Top;
            pnlNav.Left = sendFile.Left;
            sendFile.BackColor = Color.FromArgb(46,51,73);

            // Sprawdź, czy nazwa użytkownika została przekazana i przypisz ją do labela
            if (!string.IsNullOrEmpty(username))
            {
                label1.Text = "Witaj " + username + "!";
            }
            else
            {
                label1.Text = "Witaj nieznajomy!";
            }

        }
        private void ShowControl(UserControl control)
        {
            // Usuń aktualną kontrolkę, jeśli jakaś jest
            if (currentControl != null)
            {
                panel4.Controls.Remove(currentControl);
            }

            // Dodaj nową kontrolkę
            panel4.Controls.Add(control);
            control.Dock = DockStyle.Fill;
            currentControl = control;
        }


        private void button1_Click(object sender,EventArgs e)
        {
            Environment.Exit(0);
        }

        private void SetNavigationPanel(Control clickedButton)
        {
            pnlNav.Height = clickedButton.Height;
            pnlNav.Top = clickedButton.Top;
            pnlNav.Left = clickedButton.Left;
            clickedButton.BackColor = Color.FromArgb(46,51,73);


        }



        private void button3_Click(object sender,EventArgs e)
        {
            SetNavigationPanel(sendFile);
            ShowControl(uploadFileControl);
            label3.Text = "Wyślij plik";
        }


        private void button4_Click(object sender,EventArgs e)
        {
            SetNavigationPanel(yoursFile);
            ShowControl(findFileControl);
            label3.Text = "Twoje pliki";
            sendFile.BackColor = Color.FromArgb(24,30,54);



        }

        private void button5_Click(object sender,EventArgs e)
        {
            SetNavigationPanel(findFile);
            ShowControl(yourFilesControl);
            label3.Text = "Znajdź plik";
            sendFile.BackColor = Color.FromArgb(24,30,54);



        }

        private void button6_Click(object sender,EventArgs e)
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
            button.BackColor = Color.FromArgb(24,30,54);
        }
        private void button3_Leave(object sender,EventArgs e)
        {
            SetButtonDefaultColor(sendFile);
        }


        private void button4_Leave(object sender,EventArgs e)
        {
            SetButtonDefaultColor(yoursFile);
        }

        private void button5_Leave(object sender,EventArgs e)
        {
            SetButtonDefaultColor(findFile);
        }

        private void button6_Leave(object sender,EventArgs e)
        {
            SetButtonDefaultColor(button6);
        }






    }
}
