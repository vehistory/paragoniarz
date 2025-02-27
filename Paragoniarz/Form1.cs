﻿using System;
using System.Drawing;
using System.Windows.Forms;
using static Paragoniarz.DatabaseHelper;

namespace Paragoniarz
{
    public partial class Form1 : Form
    {
        private FormHelper formHelper = new FormHelper();

        public Form1()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            tbPassword.UseSystemPasswordChar = true;
            WindowHelper.SetWindowRoundCorners(this, 20);
            WindowHelper.EnableWindowDragging(panel1, this);
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            formHelper.TogglePasswordVisibility(
                tbPassword,
                pictureBox4,
                Properties.Resources.Eye,
                Properties.Resources.Closed_Eye1
            );
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string enteredUsername = tbUserName.Text;
            string enteredPassword = tbPassword.Text;

            if (string.IsNullOrEmpty(enteredUsername) || string.IsNullOrEmpty(enteredPassword))
            {
                MessageBox.Show("Wprowadź nazwę użytkownika i hasło.");
                return;
            }

            DatabaseHelper dbHelper = new DatabaseHelper();

            User user = dbHelper.ValidateUser(enteredUsername, enteredPassword);
            if (user != null)
            {
                UserSession.Login(user);

                Form2 form2 = new Form2
                {
                    StartPosition = FormStartPosition.Manual,
                    Location = this.Location,
                };
                form2.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Błędne dane logowania!");
            }
        }

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                button2.PerformClick();
            }
        }

        private void linkLabel2_MouseClick(object sender, MouseEventArgs e)
        {
            RegisterForm registerForm = new RegisterForm
            {
                StartPosition = FormStartPosition.Manual,
            };
            int x = this.Location.X + (this.Width - registerForm.Width) / 2;
            int y = this.Location.Y + (this.Height - registerForm.Height) / 2;
            registerForm.Location = new Point(x, y);
            registerForm.FormClosed += (s, args) => this.Show();

            this.Hide();
            registerForm.Show();
        }

        private void linkLabel1_MouseClick(object sender, MouseEventArgs e)
        {
            ForgottenPass forgottenPass = new ForgottenPass
            {
                StartPosition = FormStartPosition.Manual,
            };
            int x = this.Location.X + (this.Width - forgottenPass.Width) / 2;
            int y = this.Location.Y + (this.Height - forgottenPass.Height) / 2;
            forgottenPass.Location = new Point(x, y);
            forgottenPass.FormClosed += (s, args) => this.Show();
            this.Hide();
            forgottenPass.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            UserSession.Login(1, "admin", "admin@example.com");

            Form2 form2 = new Form2
            {
                StartPosition = FormStartPosition.Manual,
                Location = this.Location,
            };
            form2.Show();
            this.Hide();
        }

        private void label5_Click(object sender, EventArgs e)
        {
            UserSession.Login(1, "admin", "admin@example.com");

            Form2 form2 = new Form2
            {
                StartPosition = FormStartPosition.Manual,
                Location = this.Location,
            };
            form2.Show();
            this.Hide();
        }
    }
}
