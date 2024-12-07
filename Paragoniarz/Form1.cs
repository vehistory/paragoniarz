using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Paragoniarz
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            tbPassword.UseSystemPasswordChar = true;
        }

       

        bool isPasswordVisible = false;
        private void pictureBox4_Click(object sender, EventArgs e)
        {
            if (isPasswordVisible)
            {
                tbPassword.UseSystemPasswordChar = true;
                
                pictureBox4.Image = Properties.Resources.Closed_Eye;
            }
            else
            {
                tbPassword.UseSystemPasswordChar = false;
                
                pictureBox4.Image = Properties.Resources.Eye;
            }
            isPasswordVisible = !isPasswordVisible;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool ReleaseCapture();
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (tbUserName.Text == "admin" && tbPassword.Text == "admin")
            {
                
                Form2 form2 = new Form2();
                form2.Show();
                this.Hide();


            }
            else
            {
                MessageBox.Show("Błędne dane logowania!");
            }
        }

        private void tbUserName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                button2.PerformClick();
            }
        }

        private void tbPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                button2.PerformClick();
            }
        }

       
    }
}
