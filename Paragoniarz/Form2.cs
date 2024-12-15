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

        private string username;

        public Form2(string username)
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0,0,Width,Height,5,5));
            pnlNav.Height = button3.Height;
            pnlNav.Top = button3.Top;
            pnlNav.Left = button3.Left;
            button3.BackColor = Color.FromArgb(46,51,73);
            this.username = username;
            label1.Text = "Witaj " + username + " !";
        }

        private void button1_Click(object sender,EventArgs e)
        {
            Environment.Exit(0);
        }
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd,int Msg,int wParam,int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool ReleaseCapture();
        private void Form2_MouseDown(object sender,MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle,WM_NCLBUTTONDOWN,HT_CAPTION,0);
            }
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
