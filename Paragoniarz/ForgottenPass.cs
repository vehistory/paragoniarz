using System;
using System.Drawing;
using System.Windows.Forms;

namespace Paragoniarz
{
    public partial class ForgottenPass : Form
    {


        public ForgottenPass()
        {
            InitializeComponent();
            // Zaokrąglij rogi okna 
            WindowHelper.SetWindowRoundCorners(this, 20);

            // Umożliw przesuwanie okna 
            WindowHelper.EnableWindowDragging(panel2, this);
            this.StartPosition = FormStartPosition.CenterScreen;
        }


        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RegisterForm registerForm = new RegisterForm();
            registerForm.StartPosition = FormStartPosition.Manual;
            int x = this.Location.X + (this.Width - registerForm.Width) / 2;
            int y = this.Location.Y + (this.Height - registerForm.Height) / 2;
            registerForm.Location = new Point(x, y);
            registerForm.FormClosed += (s, args) => this.Show();
            this.Hide();
            registerForm.Show();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form1 form1 = new Form1();
            form1.StartPosition = FormStartPosition.Manual;
            int x = this.Location.X + (this.Width - form1.Width) / 2;
            int y = this.Location.Y + (this.Height - form1.Height) / 2;
            form1.Location = new Point(x, y);
            form1.FormClosed += (s, args) => this.Show();
            this.Hide();
            form1.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
