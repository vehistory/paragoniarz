using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paragoniarz
{
    public partial class ForgottenPass : Form
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
        public ForgottenPass()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0,0,Width,Height,20,20));
            this.StartPosition = FormStartPosition.CenterScreen;
        }
        //funckja pozwalajaca na przesuwanie okna
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd,int Msg,int wParam,int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool ReleaseCapture();
        private void ForgottenPass_MouseDown(object sender,MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle,WM_NCLBUTTONDOWN,HT_CAPTION,0);
            }
        }

        private void linkLabel2_LinkClicked(object sender,LinkLabelLinkClickedEventArgs e)
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

        private void linkLabel1_LinkClicked(object sender,LinkLabelLinkClickedEventArgs e)
        {
            Form1 form1 = new Form1();
            form1.StartPosition = FormStartPosition.Manual;
            int x = this.Location.X + (this.Width - form1.Width) / 2;
            int y = this.Location.Y + (this.Height - form1.Height) / 2;
            form1.Location = new Point(x,y);
            form1.FormClosed += (s,args) => this.Show();
            this.Hide();
            form1.Show();
        }

        private void button1_Click(object sender,EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
