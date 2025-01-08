using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Paragoniarz
{
    internal class FormHelper
    {
        private bool isPasswordVisible = false;

        public bool IsUsernameOrEmailTaken(
            string username,
            string email,
            Dictionary<string, Tuple<string, string>> users
        )
        {
            if (users.ContainsKey(username))
            {
                return true;
            }

            foreach (var user in users.Values)
            {
                if (user.Item1 == email)
                {
                    return true;
                }
            }

            return false;
        }

        public void TogglePasswordVisibility(
            TextBox tbPassword,
            PictureBox pictureBox,
            Image visibleImage,
            Image hiddenImage
        )
        {
            if (isPasswordVisible)
            {
                tbPassword.UseSystemPasswordChar = true;
                pictureBox.Image = hiddenImage;
            }
            else
            {
                tbPassword.UseSystemPasswordChar = false;
                pictureBox.Image = visibleImage;
            }
            isPasswordVisible = !isPasswordVisible;
        }

        public bool ValidatePassword(string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                MessageBox.Show("Hasła nie pasują do siebie.");
                return false;
            }
            if (password.Length < 8)
            {
                MessageBox.Show("Hasło musi mieć co najmniej 8 znaków.");
                return false;
            }
            if (!Regex.IsMatch(password, @"[a-z]"))
            {
                MessageBox.Show("Hasło musi zawierać co najmniej jedną małą literę.");
                return false;
            }
            if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                MessageBox.Show("Hasło musi zawierać co najmniej jedną dużą literę.");
                return false;
            }
            if (!Regex.IsMatch(password, @"[0-9]"))
            {
                MessageBox.Show("Hasło musi zawierać co najmniej jedną cyfrę.");
                return false;
            }
            if (!Regex.IsMatch(password, @"[!@#$%^&*()_+}{:|>?<]"))
            {
                MessageBox.Show("Hasło musi zawierać co najmniej jeden znak specjalny.");
                return false;
            }
            return true;
        }

        public bool ValidateEmail(string email)
        {
            if (email.Length == 0)
            {
                MessageBox.Show("Email nie może być pusty");
                return false;
            }
            else if (!Regex.IsMatch(email, @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$"))
            {
                MessageBox.Show("Niepoprawny format email");
                return false;
            }
            return true;
        }
    }
}
