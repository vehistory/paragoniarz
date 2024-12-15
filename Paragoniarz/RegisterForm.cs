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
       
        
        public RegisterForm()
        {
            InitializeComponent();
            // Zaokrąglij rogi okna 
            WindowHelper.SetWindowRoundCorners(this,20);
            // Umożliw przesuwanie okna 
            WindowHelper.EnableWindowDragging(panel1,this);
            passBox.UseSystemPasswordChar = true;
            rePassBox.UseSystemPasswordChar = true;

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

        private void linkLabel1_MouseClick(object sender,MouseEventArgs e)
        {
            Form1 form1 = new Form1();
            form1.StartPosition = FormStartPosition.Manual;
            int x = this.Location.X + (this.Width - form1.Width) / 2;
            int y = this.Location.Y + (this.Height - form1.Height) / 2;
            form1.Location = new Point(x,y);
            form1.Show();
            this.Hide();
        }
    }
}
