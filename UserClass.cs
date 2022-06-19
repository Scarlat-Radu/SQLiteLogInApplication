using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_SQLite_Demo
{
   public class UserClass
    {
        public UserClass()
        {
        }
        public static UserClass DefaultUser(UserClass defaultUser)
        {
            defaultUser.UserName = "";
            defaultUser.USerPassword = "";
            defaultUser.IsLoggedIn = false;
            defaultUser.RememberMe = false;
            defaultUser.IsAdmin = false;

            return defaultUser;
        }
        public UserClass(string uName, string uPassword, bool isAdmin, bool remeberMe)
        {
            UserName = uName;
            USerPassword = uPassword;
            LogedInTime = DateTime.Now;
            IsAdmin = isAdmin;
            RememberMe = remeberMe;
        }

        public string UserName { get; set; }
        public string USerPassword { get; set; }
        public bool IsLoggedIn { get; set; }
        public bool IsAdmin { get; set; }
        public bool RememberMe { get; set; }
        public DateTime LogedInTime { get; set; }
    }
}
