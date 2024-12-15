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
    public partial class Form1 : Form
    {
        [DllImport("Gdi32.dll",EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
       (
           int nLeftRect,
           int nTopRect,
           int nRightRect,
           int nBottomRect,
           int nWidthEllipse,
           int nHeightEllipse
       );

        private FormHelper formHelper = new FormHelper();

        public Form1()
        {
            InitializeComponent();

            tbPassword.UseSystemPasswordChar = true;
            
            //Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0,0,Width,Height,20,20));
            //this.StartPosition = FormStartPosition.CenterScreen;

            // Zaokrąglij rogi okna NIEDZIALA
            //WindowHelper.SetWindowRoundCorners(this,20);

            // Umożliw przesuwanie okna NIEDZIALA
            //WindowHelper.EnableWindowDragging(this);



        }
        //funckja pozwalajaca na przesuwanie okna
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd,int Msg,int wParam,int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        private void Form1_MouseDown(object sender,MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle,WM_NCLBUTTONDOWN,HT_CAPTION,0);
            }
        }


        private void pictureBox4_Click(object sender,EventArgs e)
        {
            formHelper.TogglePasswordVisibility(tbPassword,pictureBox4,Properties.Resources.Eye,Properties.Resources.Closed_Eye1);
        }


        private void button1_Click(object sender,EventArgs e)
        {
            Environment.Exit(0);
        }


        private void button2_Click(object sender,EventArgs e)
        {
            string enteredUsername = tbUserName.Text;
            string enteredPassword = tbPassword.Text;

            if (string.IsNullOrEmpty(enteredUsername) || string.IsNullOrEmpty(enteredPassword))
            {
                MessageBox.Show("Wprowadź nazwę użytkownika i hasło.");
                return;
            }

            DatabaseHelper dbHelper = new DatabaseHelper();

            if (dbHelper.ValidateUser(enteredUsername,enteredPassword))
            {
                Form2 form2 = new Form2(enteredUsername);
                form2.StartPosition = FormStartPosition.Manual;
                form2.Location = this.Location;
                form2.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Błędne dane logowania!");
            }
        }





        private void TextBox_KeyPress(object sender,KeyPressEventArgs e)
        {
            if (e.KeyChar == (char) Keys.Enter)
            {
                button2.PerformClick();
            }
        }

        private void linkLabel2_MouseClick(object sender,MouseEventArgs e)
        {
            RegisterForm registerForm = new RegisterForm();
            registerForm.StartPosition = FormStartPosition.Manual;
            int x = this.Location.X + (this.Width - registerForm.Width) / 2;
            int y = this.Location.Y + (this.Height - registerForm.Height) / 2;
            registerForm.Location = new Point(x,y);
            registerForm.FormClosed += (s,args) => this.Show();
            
            this.Hide();
            registerForm.Show();

        }

      

        private void linkLabel1_MouseClick(object sender,MouseEventArgs e)
        {
            ForgottenPass forgottenPass = new ForgottenPass();
            forgottenPass.StartPosition = FormStartPosition.Manual;
            int x = this.Location.X + (this.Width - forgottenPass.Width) / 2;
            int y = this.Location.Y + (this.Height - forgottenPass.Height) / 2;
            forgottenPass.Location = new Point(x,y);
            forgottenPass.FormClosed += (s,args) => this.Show();
            this.Hide();
            forgottenPass.Show();
        }

       
    }
}
