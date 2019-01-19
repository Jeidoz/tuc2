using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tuc2.Entities;

namespace tuc2
{
    public static class RolesInfo
    {
        public const int AdminId = 1;
        public const int UserId = 2;
    }
    public enum UserRoles
    {
        None,
        Admin,
        User
    }
}
