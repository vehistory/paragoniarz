using System;
using static Paragoniarz.DatabaseHelper;

namespace Paragoniarz
{
    public static class UserSession
    {
        public static int UserId { get; private set; }
        public static string Username { get; private set; }
        public static string Email { get; private set; }
        public static bool IsLoggedIn { get; private set; }

        public static void Login(User user)
        {
            UserId = user.Id;
            Username = user.Username;
            Email = user.Email;
            IsLoggedIn = true;
        }

        public static void Login(int userId, string username, string email)
        {
            UserId = userId;
            Username = username;
            Email = email;
            IsLoggedIn = true;
        }

        public static void Logout()
        {
            UserId = 0;
            Username = null;
            Email = null;
            IsLoggedIn = false;
        }
    }
}
