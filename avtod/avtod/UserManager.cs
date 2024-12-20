using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avtod
{
    internal class UserManager
    {
        public static User CurrentUser { get; private set; }

        public static void SetCurrentUser(string email, int roleId)
        {
            CurrentUser = new User(email, roleId);
        }

        public static void ClearCurrentUser()
        {
            CurrentUser = null;
        }
    }
}
