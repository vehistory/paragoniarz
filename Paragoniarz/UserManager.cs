using System.Collections.Generic;
using System;

public static class UserManager
{
    private static Dictionary<string,Tuple<string,string>> users = new Dictionary<string,Tuple<string,string>>
    {
        { "sudap87", Tuple.Create("sudap87@example.com", "Azure1") },
        { "admin", Tuple.Create("admin@example.com", "admin") },
        { "bascio", Tuple.Create("bascio@example.com", "sdfsdf") },
        { "wersjon", Tuple.Create("wersjon@example.com", "TestHaslo") }
    };

    public static bool IsUsernameOrEmailTaken(string username,string email)
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

    public static void RegisterUser(string username,string email,string password)
    {
        users[username] = Tuple.Create(email,password);
    }

    public static bool ValidateUser(string username,string password)
    {
        return users.ContainsKey(username) && users[username].Item2 == password;
    }
    public static Dictionary<string,Tuple<string,string>> GetAllUsers()
    {
        return users;
    }
}
