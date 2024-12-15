using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Paragoniarz
{
    public partial class Form2 : Form
    {
       
        public Form2(string username)
        {
            InitializeComponent();
            // Zaokrąglij rogi okna 
            WindowHelper.SetWindowRoundCorners(this,20);

            // Umożliw przesuwanie okna 
            WindowHelper.EnableWindowDragging(panel1,this);

            pnlNav.Height = button3.Height;
            pnlNav.Top = button3.Top;
            pnlNav.Left = button3.Left;
            button3.BackColor = Color.FromArgb(46,51,73);
           
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

        private void button1_Click(object sender,EventArgs e)
        {
            Environment.Exit(0);
        }
       
        private void button2_Click(object sender,EventArgs e)
        {


            Form1 form1 = new Form1();
            form1.StartPosition = FormStartPosition.Manual;
            form1.Location = this.Location;
            form1.Show();
            this.Hide();

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
            SetNavigationPanel(button3);
        }


        private void button4_Click(object sender,EventArgs e)
        {
            SetNavigationPanel(button4);
        }

        private void button5_Click(object sender,EventArgs e)
        {
            SetNavigationPanel(button5);
        }

        private void button6_Click(object sender,EventArgs e)
        {
            SetNavigationPanel(button6);
        }

        private void SetButtonDefaultColor(Button button)
        {
            button.BackColor = Color.FromArgb(24,30,54);
        }
        private void button3_Leave(object sender,EventArgs e)
        {
            SetButtonDefaultColor(button3);
        }


        private void button4_Leave(object sender,EventArgs e)
        {
            SetButtonDefaultColor(button4);
        }

        private void button5_Leave(object sender,EventArgs e)
        {
            SetButtonDefaultColor(button5);
        }

        private void button6_Leave(object sender,EventArgs e)
        {
            SetButtonDefaultColor(button6);
        }

        private void Form2_Load(object sender,EventArgs e)
        {

        }
    }
}
