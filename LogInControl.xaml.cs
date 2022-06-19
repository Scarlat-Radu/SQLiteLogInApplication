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

namespace WPF_SQLite_Demo
{
    /// <summary>
    /// Interaction logic for LogInControl.xaml
    /// </summary>
    public partial class LogInControl : UserControl
    {
        private SQLiteConnection sqlconn;
        private SQLiteCommand sqlCmd;
        private DataTable sqlDT = new DataTable();
        private DataSet DS = new DataSet();
        private SQLiteDataAdapter DB;

        public static UserClass CurrentUser { get; set; } = new UserClass();
        public LogInControl()
        {
            InitializeComponent();
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

        private void bLogMeIn_Click(object sender, RoutedEventArgs e)
        {
            CurrentUser
        }
    }
}
