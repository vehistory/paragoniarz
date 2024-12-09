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
using System.Runtime.InteropServices;

namespace Paragoniarz
{
    public partial class Form1 : Form
    {

        //private Dictionary<string,Tuple<string,string>> users = new Dictionary<string,Tuple<string,string>>
        //{
        //    { "sudap87", Tuple.Create("sudap87@example.com", "Azure1") },
        //    { "admin", Tuple.Create("admin@example.com", "admin") },
        //    { "bascio", Tuple.Create("bascio@example.com", "sdfsdf") },
        //    { "wersjon", Tuple.Create("wersjon@example.com", "TestHaslo") }

        //};
        //private void OpenRegisterForm()
        //{
        //    // Tworzymy obiekt RegisterForm i przekazujemy słownik
        //    RegisterForm registerForm = new RegisterForm(users);
        //    registerForm.ShowDialog();
        //}

        private FormHelper formHelper = new FormHelper();

        public Form1()
        {
            InitializeComponent();
            tbPassword.UseSystemPasswordChar = true;
            this.StartPosition = FormStartPosition.CenterScreen;
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

            if (UserManager.ValidateUser(enteredUsername,enteredPassword))
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


        private void tbUserName_KeyPress(object sender,KeyPressEventArgs e)
        {
            if (e.KeyChar == (char) Keys.Enter)
            {
                button2.PerformClick();
            }
        }

        private void tbPassword_KeyPress(object sender,KeyPressEventArgs e)
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
    }
}
