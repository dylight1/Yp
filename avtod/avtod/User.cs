using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace avtod
{
    internal class User
    {
        public string Email { get; }
        public int RoleId { get; }

        public User(string email, int roleId)
        {
            Email = email;
            RoleId = roleId;
        }
    }
}
