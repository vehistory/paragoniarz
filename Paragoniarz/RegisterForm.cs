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
    public partial class RegisterForm : Form
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

        //private Dictionary<string,Tuple<string,string>> users;
        public RegisterForm()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0,0,Width,Height,20,20));
            passBox.UseSystemPasswordChar = true;
            rePassBox.UseSystemPasswordChar = true;

        }

        //funckja pozwalajaca na przesuwanie okna
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd,int Msg,int wParam,int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool ReleaseCapture();
        private void RegisterForm_MouseDown(object sender,MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle,WM_NCLBUTTONDOWN,HT_CAPTION,0);
            }
        }


        private FormHelper formHelper = new FormHelper();


        //obsluga widocznosci hasla dla pola haslo
        private void pictureBox5_Click(object sender,EventArgs e)
        {
            formHelper.TogglePasswordVisibility(passBox,pictureBox5,Properties.Resources.RegisterOpenEye,Properties.Resources.RegisterClosed_Eye2);
        }
        //obsluga widocznosci hasla dla pola powtorz haslo
        private void pictureBox6_Click(object sender,EventArgs e)
        {
            formHelper.TogglePasswordVisibility(rePassBox,pictureBox6,Properties.Resources.RegisterOpenEye,Properties.Resources.RegisterClosed_Eye2);
        }



        //obsluga linkuLabel powrot do logowania
        private void linkLabel1_LinkClicked(object sender,LinkLabelLinkClickedEventArgs e)
        {
            Form1 form1 = new Form1();
            form1.StartPosition = FormStartPosition.Manual;
            int x = this.Location.X + (this.Width - form1.Width) / 2;
            int y = this.Location.Y + (this.Height - form1.Height) / 2;
            form1.Location = new Point(x,y);
            form1.Show();
            this.Close();
        }

        //obsluga butonna "x" do zamkniecia okna
        private void exitButton_Click(object sender,EventArgs e)
        {


            Environment.Exit(0);

        }

        //oblsuga przycisku zarejestruj
        private void button3_Click(object sender,EventArgs e)
        {
            string password = passBox.Text;
            string confirmPassword = rePassBox.Text;
            string email = textBox2.Text;
            string username = textBox1.Text;


            DatabaseHelper dbHelper = new DatabaseHelper();

            if (dbHelper.IsUsernameOrEmailTaken(username,email))
            {
                MessageBox.Show("Nazwa użytkownika lub e-mail są już zajęte.");
                return;
            }

            if (!formHelper.ValidateEmail(email))
            {
                MessageBox.Show("Niepoprawny format e-maila.");
                return;
            }

            if (!formHelper.ValidatePassword(password,confirmPassword))
            {
                MessageBox.Show("Hasło musi mieć co najmniej 8 znaków, zawierać jedną cyfrę, jedną wielką literę i jeden znak specjalny.");
                return;
            }


           
            dbHelper.InsertUser(username,email,password);



            MessageBox.Show("Rejestracja zakończona sukcesem!");
            Form1 form1 = new Form1();
            form1.StartPosition = FormStartPosition.Manual;
            int x = this.Location.X + (this.Width - form1.Width) / 2;
            int y = this.Location.Y + (this.Height - form1.Height) / 2;
            form1.Location = new Point(x,y);
            form1.Show();
            this.Hide();
        }

       



        private void Field_KeyDown(object sender,KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button3.PerformClick();  // Kliknięcie przycisku "Zarejestruj"
            }
        }


       
    }
}
