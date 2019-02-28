using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tuc2.ViewModels;

namespace tuc2
{
    public static class RolesInfo
    {
        public const int AdminId = 1;
        public const int UserId = 2;
        public const string Admin = "admin";
        public const string User = "user";
    }
    public enum UserRoles
    {
        None,
        Admin,
        User
    }
}
