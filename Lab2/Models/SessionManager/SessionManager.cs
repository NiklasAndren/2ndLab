using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lab2.Models.Repositories;
using Lab2.Models.Entities;
using Lab2.Utils;

namespace Lab2.Models.SessionManager
{
    public class SessionManager
    {
        // Jag gör endast en publik getter - Jag vill inte att CurrentUser skall gå att ändra på utanför SessionManager
        private static User currentUser;
        public static User CurrentUser { get { return currentUser; } }

        public static bool ValidateUser(string userName, string password)
        {
            // Plocka ut aktuell user
            var user = Repository.Instance.All<User>().Where(u => u.UserName == userName).FirstOrDefault();

            // Kontrollera att användaren finns
            if (null == user)
                return false;

            // Kontrollera lösenord - hashfunktionen bör eg. läggas i en metod då den även används i user
            if (user.Password != Helpers.CreateHash(password))
                return false;

            // Login OK! Sätt CurrentUser
            currentUser = user;
            return true;
        }

        public static void SignOut()
        {
            currentUser = null;
        }
    }
}