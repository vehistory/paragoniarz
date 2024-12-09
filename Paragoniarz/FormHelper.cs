using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Text.RegularExpressions;


namespace Paragoniarz
{
    internal class FormHelper
    {

        public bool IsUsernameOrEmailTaken(string username,string email,Dictionary<string,Tuple<string,string>>users)
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


        // funkcja odpowiedzialna za widocznosc hasla
        private bool isPasswordVisible = false;

        public void TogglePasswordVisibility(TextBox tbPassword,PictureBox pictureBox,Image visibleImage,Image hiddenImage)
        {
            if (isPasswordVisible)
            {
                tbPassword.UseSystemPasswordChar = true;
                pictureBox.Image = hiddenImage;  // Ustawienie obrazu dla ukrytego hasła
            }
            else
            {
                tbPassword.UseSystemPasswordChar = false;
                pictureBox.Image = visibleImage;  // Ustawienie obrazu dla widocznego hasła
            }
            isPasswordVisible = !isPasswordVisible;
        }


        // funkcja sprawdzajaca poprawnosc hasla
        public bool ValidatePassword(string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                return false;
            }
            if (password.Length < 8)
            {
                MessageBox.Show("Hasło musi mieć co najmniej 8 znaków");
                return false;
            }
            else if (!Regex.IsMatch(password,@"[a-z]"))
            {
                MessageBox.Show("Hasło musi zawierać co najmniej jedną małą literę");
                return false;
            }
            else if (!Regex.IsMatch(password,@"[A-Z]"))
            {
                MessageBox.Show("Hasło musi zawierać co najmniej jedną dużą literę");
                return false;
            }
            else if (!Regex.IsMatch(password,@"[0-9]"))
            {
                MessageBox.Show("Hasło musi zawierać co najmniej jedną cyfrę");
                return false;
            }
            else if (!Regex.IsMatch(password,@"[!@#$%^&*()_+}{:|>?<]"))
            {
                MessageBox.Show("Hasło musi zawierać co najmniej jeden znak specjalny");
                return false;
            }
            return true;
        }
        //funkcja walidacji email
        public bool ValidateEmail(string email)
        {
            if (email.Length == 0)
            {
                MessageBox.Show("Email nie może być pusty");
                return false;
            }
            else if (!Regex.IsMatch(email,@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$"))
            {
                MessageBox.Show("Niepoprawny format email");
                return false;
            }
            return true;
        }

    }
}
