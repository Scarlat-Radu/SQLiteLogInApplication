using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using MaterialDesignThemes.Wpf;

namespace WPF_SQLite_Demo
{
    /// <summary>
    /// Interaction logic for LogInControl.xaml
    /// </summary>
    public partial class LogInControl : UserControl
    {
        public event EventHandler<UserLogInOutStatetArgs> UserLogedOut;
        public event EventHandler<DbUpdateArgs> DbUpdated;

        private SQLiteConnection sqlconn;
        private SQLiteCommand sqlCmd;
        private DataTable sqlDT = new DataTable();
        private DataSet DS = new DataSet();
        private SQLiteDataAdapter DB;
        public static UserClass CurrentUser { get; set; } = new UserClass();
        public LogInControl()
        {
            InitializeComponent();
            ChangeDialog(Visibility.Visible);
        }
        public DataView LoadData()
        {
            SetConnection();
            sqlconn.Open();
            sqlCmd = sqlconn.CreateCommand();
            string CommandText = "select * from Users";
            DB = new SQLiteDataAdapter(CommandText, sqlconn);
            DS.Reset();
            DB.Fill(DS);
            sqlDT = DS.Tables[0];
            sqlconn.Close();

            return sqlDT.DefaultView;
        }
        private void SetConnection()
        {
            string path = Helper.Get.StartUpDirectory() + "Data\\" + "SQLite";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = Helper.Get.StartUpDirectory() + "Data" + "\\SQLite" + "\\UsersDB.db";
            if (File.Exists(path))
                sqlconn = new SQLiteConnection("Data Source =" + path);
            else
            {
                sqlconn = new SQLiteConnection("Data Source =" + path);

                sqlconn.Open();
                sqlCmd = sqlconn.CreateCommand();
                string CommandText = "CREATE TABLE Users" +
                         "(UserName CHAR(50)," +
                         "UserPassword CHAR(50)," +
                         "IsAdmin INTEGER)";

                DB = new SQLiteDataAdapter(CommandText, sqlconn);
                DS.Reset();
                DB.Fill(DS);

                string Command = "Insert into Users (UserName,UserPassword,IsAdmin)" +
                "values('Radmin','123','1')";
                ExecuteQuery(Command);
                LoadData();
                sqlconn.Close();
            }
        }

       

        private void ExecuteQuery(string EmployeeIDq)
        {
            SetConnection();
            sqlconn.Open();
            sqlCmd = sqlconn.CreateCommand();
            sqlCmd.CommandText = EmployeeIDq;
            sqlCmd.ExecuteNonQuery();
            sqlCmd.Dispose();
            sqlconn.Close();

        }
        public void UpdateUsersDB(string userName, string password, string oldUserName)
        {
            string Command = "update Users set UserName='" + userName +
               "',UserPassword='" + password +
               "' where UserName='" + oldUserName + "'";
            ExecuteQuery(Command);
            DbUpdated?.Invoke(this, new DbUpdateArgs() { DView  = LoadData()});
            
        }

        private void bLogMeIn_Click(object sender, RoutedEventArgs e)
        {
            if (VerifyCredentials() && !CurrentUser.IsLoggedIn)
            {
                CurrentUser.IsLoggedIn = true;
                UserLogedOut?.Invoke(this, new UserLogInOutStatetArgs() { IsLogedIn = true, user = CurrentUser});
                dialogLogin.IsOpen = !dialogLogin.IsOpen;
            }
            else if (CurrentUser.IsLoggedIn)
            {
                CurrentUser.IsLoggedIn = false;
                UserLogedOut?.Invoke(this, new UserLogInOutStatetArgs() { IsLogedIn = false, user = CurrentUser });
                //dialogLogin.IsOpen = !dialogLogin.IsOpen;
                ChangeDialog(Visibility.Visible);
            }
            else
            {
                CurrentUser.IsLoggedIn = false;

                tbTextMessage.Text = "User or password is incorect";
            }
        }

        public bool VerifyCredentials()
        {
            bool allowLogIn = false;
            string uName = tbUserName.Text;
            string uPassword = tbUserPassword.Text;

            if (CredentialExists(uName, uPassword))
                allowLogIn = true;
            else
                allowLogIn = false;

            return allowLogIn;
        }

        public bool CredentialExists(string uName, string uPass)
        {

            SetConnection();
            sqlconn.Open();
            sqlCmd = sqlconn.CreateCommand();
            string CommandText = "SELECT UserName, UserPassword, IsAdmin FROM Users WHERE UserName='" + uName + "'";
            SQLiteDataAdapter adaptor = new SQLiteDataAdapter(CommandText, sqlconn);
            DataSet ds = new DataSet();
            ds.Reset();
            adaptor.Fill(ds);

            bool goodName = false,goodPass = false, isAdmin = false;
            var rowColl = ds.Tables[0].AsEnumerable();
            try
            {
                string name = (from r in rowColl
                               where r.Field<string>("UserName") == uName
                               select r.Field<string>("UserName")).First<string>();
                goodName = true;
                string pass = (from r in rowColl
                               where r.Field<string>("UserPassword") == uPass
                               select r.Field<string>("UserPassword")).First<string>();
                goodPass = true;
                long admin = (from r in rowColl
                               where r.Field<System.Int64>("IsAdmin") == 1
                               select r.Field<System.Int64>("IsAdmin")).First();
                isAdmin = true;
                //CurrentUser.UserName = uName
            }
            catch (Exception ex)
            {
                _ = ex.Message;
                if (ex.Message == "Sequence contains no elements")
                {
                    if (goodName && goodPass)
                    {
                        CurrentUser.UserName = uName;
                        CurrentUser.USerPassword = uPass;
                        return true;
                    }
                        
                    else
                        return false;
                }
                else
                    return false;
            }
            if (goodName && goodPass)
            {
                if (isAdmin)
                    CurrentUser.IsAdmin = true;
                CurrentUser.UserName = uName;
                CurrentUser.USerPassword = uPass;
            }
            return true;
        }
        internal void ChangeDialog(Visibility v)
        {
            tbUserName.Visibility = v;
            tbUserPassword.Visibility = v;
            ckbRemeberLogIn.Visibility = v;
            if (v == Visibility.Visible)
            {
                tbTextMessage.Text = "WELCOME!";
                bLogMeIn.Content = PackIconKind.Login;
                bExitLoginDialog.IsEnabled = false;
            }
            else
            {
                tbTextMessage.Text = "WELCOME " + CurrentUser.UserName + " !";
                bLogMeIn.Content = PackIconKind.Logout;
                bExitLoginDialog.IsEnabled = true;
            }

        }
        
        private void bExitLoginDialog_Click(object sender, RoutedEventArgs e)
        {
            dialogLogin.IsOpen = !dialogLogin.IsOpen;
        }
    }

    public class DbUpdateArgs
    {
        public DataView DView { get; set; }
    }

    public class UserLogInOutStatetArgs
    {
        public UserClass user { get; set; }
        public bool IsLogedIn { get; set; }
        public string UserName { get; set; }
        public bool NeedLogOut { get; set; }
    }
}
