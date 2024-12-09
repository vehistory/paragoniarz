using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Paragoniarz
{
    public partial class RegisterForm : Form
    {

        private Dictionary<string,Tuple<string,string>> users;
        public RegisterForm()
        {
            InitializeComponent();
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
            this.Hide();
        }

        //obsluga butonna "x" do zamkniecia okna
        private void exitButton_Click(object sender,EventArgs e)
        {

            //Form1 form1 = new Form1();
            //form1.StartPosition = FormStartPosition.Manual;
            //int x = this.Location.X + (this.Width - form1.Width) / 2;
            //int y = this.Location.Y + (this.Height - form1.Height) / 2;
            //form1.Location = new Point(x,y);
            //form1.Show();
            //this.Hide();
            Environment.Exit(0);

        }

        //oblsuga przycisku zarejestruj
        private void button3_Click(object sender,EventArgs e)
        {
            string password = passBox.Text;
            string confirmPassword = rePassBox.Text;
            string email = textBox2.Text;
            string username = textBox1.Text;

            if (UserManager.IsUsernameOrEmailTaken(username,email))
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

            UserManager.RegisterUser(username,email,password);
            MessageBox.Show("Rejestracja zakończona sukcesem!");
            Form1 form1 = new Form1();
            form1.StartPosition = FormStartPosition.Manual;
            int x = this.Location.X + (this.Width - form1.Width) / 2;
            int y = this.Location.Y + (this.Height - form1.Height) / 2;
            form1.Location = new Point(x,y);
            form1.Show();
            this.Hide();
        }

        public bool IsUsernameOrEmailTaken(string username,string email)
        {
            // Sprawdzamy, czy nazwa użytkownika już istnieje
            if (users.ContainsKey(username))
            {
                return true;  // Nazwa użytkownika już istnieje
            }

            // Sprawdzamy, czy email już istnieje
            foreach (var user in users.Values)
            {
                if (user.Item1 == email)
                {
                    return true;  // E-mail już istnieje
                }
            }

            // Jeśli nazwa użytkownika ani e-mail nie istnieją, zwracamy false
            return false;
        }

        private void textBox1_KeyDown(object sender,KeyEventArgs e)
        {
            if((e.KeyCode == Keys.Enter))
            {
                button3.PerformClick();
            }
            
        }

        private void textBox2_KeyDown(object sender,KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter))
            {
                button3.PerformClick();
            }
        }

        private void passBox_KeyDown(object sender,KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter))
            {
                button3.PerformClick();
            }
        }

        private void rePassBox_KeyDown(object sender,KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter))
            {
                button3.PerformClick();
            }
        }
    }
}
