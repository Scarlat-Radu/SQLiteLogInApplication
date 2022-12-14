using System;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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

        /// <summary>
        /// Get users data base path and set SQLite connection
        /// </summary>
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
                         "IsAdmin INTEGER," +
                         "IsOnline INTEGER)";

                DB = new SQLiteDataAdapter(CommandText, sqlconn);
                DS.Reset();
                DB.Fill(DS);

                string Command = "Insert into Users (UserName,UserPassword,IsAdmin,IsOnline)" +
                "values('Radmin','123','1','0')";
                ExecuteQuery(Command);
                LoadData();
                sqlconn.Close();
            }
        }

        /// <summary>
        /// Execute sql given command.
        /// </summary>
        /// <param name="sqlCommand"></param>
        public void ExecuteQuery(string sqlCommand)
        {
            SetConnection();
            sqlconn.Open();
            sqlCmd = sqlconn.CreateCommand();
            sqlCmd.CommandText = sqlCommand;
            sqlCmd.ExecuteNonQuery();
            sqlCmd.Dispose();
            sqlconn.Close();
            DbUpdated?.Invoke(this, new DbUpdateArgs() { DView = LoadData() });

        }

        /// <summary>
        /// Send sql command to update user name and password.
        /// Trigger event with the updated data.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="oldUserName"></param>
        public void UpdateUsersDB(string userName, string password, bool isAdmin, string oldUserName)
        {
            string Command = "update Users set UserName='" + userName +
               "',UserPassword='" + password +
               "',IsAdmin='" + Convert.ToInt32(isAdmin) +
               "' where UserName='" + oldUserName + "'";
            ExecuteQuery(Command);
            DbUpdated?.Invoke(this, new DbUpdateArgs() { DView  = LoadData()});
            
        }
        public void UpdateUsersDB(string userName,bool isOnline )
        {
            string Command = "update Users set UserName='" + userName +
               "',IsOnline='" + Convert.ToInt32(isOnline) +
               "' where UserName='" + userName + "'";
            ExecuteQuery(Command);
           // DbUpdated?.Invoke(this, new DbUpdateArgs() { DView = LoadData() });

        }
        /// <summary>
        /// Change diaglog content visibility based on login status.
        /// </summary>
        /// <param name="v">Visibility paramete to be set</param>
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
        /// <summary>
        /// Open sql connection to db and check user input for a valid login.
        /// </summary>
        /// <param name="uName"></param>
        /// <param name="uPass"></param>
        /// <returns></returns>
        public bool CredentialExists(string uName, string uPass)
        {
            SetConnection();
            sqlconn.Open();
            sqlCmd = sqlconn.CreateCommand();
            string CommandText = "SELECT UserName, UserPassword, IsAdmin, IsOnline FROM Users WHERE UserName='" + uName + "'";
            SQLiteDataAdapter adaptor = new SQLiteDataAdapter(CommandText, sqlconn);
            DataSet ds = new DataSet();
            ds.Reset();
            adaptor.Fill(ds);


            object[] userData = new object[5];

            bool goodName = false,goodPass = false, isAdmin = false, isOnline = false;
            try
            {
                var rowColl = ds.Tables[0].AsEnumerable();
                foreach (DataRow data in ds.Tables[0].AsEnumerable())
                {
                    userData = data.ItemArray;
                }
                if (uName == userData[0].ToString() && uPass == userData[1].ToString())
                {
                    goodName = goodPass = true;
                    isAdmin = Convert.ToBoolean(userData[2]);
                    isOnline = Convert.ToBoolean(userData[3]);
                }
                else
                    return false;
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
                CurrentUser.IsOnline = isOnline;
            }
            return true;
        }
        private void bLogMeIn_Click(object sender, RoutedEventArgs e)
        {
            if (VerifyCredentials() && !CurrentUser.IsLoggedIn)
            {
                CurrentUser.IsLoggedIn = true;
                UserLogedOut?.Invoke(this, new UserLogInOutStatetArgs() { IsLogedIn = true, user = CurrentUser });
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
        private void bExitLoginDialog_Click(object sender, RoutedEventArgs e)
        {
            dialogLogin.IsOpen = !dialogLogin.IsOpen;
        }
    }
    /// <summary>
    /// Data base update argumets.Used to refresh UI data table after db is updated.
    /// </summary>
    public class DbUpdateArgs
    {
        public DataView DView { get; set; }
    }
    /// <summary>
    /// Event arguments for user login state.
    /// </summary>
    public class UserLogInOutStatetArgs
    {
        public UserClass user { get; set; }
        public bool IsLogedIn { get; set; }
        public string UserName { get; set; }
        public bool NeedLogOut { get; set; }
    }
}
