using Dragablz;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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

namespace WPF_SQLite_Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static DataView DbUsersDataTable { get; set; } = new DataView();
        public Func<HeaderedItemViewModel> ItemFactory
        {
            get
            {
                return
                    () =>
                    {
                        return new HeaderedItemViewModel()
                        {
                            Header ="Test",
                            IsSelected = true
                        };
                    };
            }
        }
        public MainWindow()
        {
            if(Helper.FirstStart)
            {
                Helper.setTheme(false);
            }

            InitializeComponent();
            DbUsersDataTable = logInControl.LoadData();
            TabMain.NewItemFactory = ItemFactory;
            logInControl.UserLogedOut += LogInControl_UserLogedOut;
            logInControl.DbUpdated += LogInControl_DbUpdated;
            Helper.mainWindow.Add(this);

        }

        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Helper.FirstStart == false)
                return;
            dbTable.DataContext = DbUsersDataTable;
            logInControl.dialogLogin.IsOpen = true;
            Helper.FirstStart = false;
        }

        private void bLogIn_Click(object sender, RoutedEventArgs e)
        {
            if(LogInControl.CurrentUser.IsLoggedIn)
            {
                logInControl.ChangeDialog(Visibility.Collapsed);
            }
            else
            {
                logInControl.ChangeDialog(Visibility.Visible);

            }
            logInControl.dialogLogin.IsOpen = !logInControl.dialogLogin.IsOpen;

        }
        private void LogInControl_UserLogedOut(object sender, UserLogInOutStatetArgs e)
        {
            if (e.IsLogedIn)
            {
                tbCurrentUser.Text = LogInControl.CurrentUser.UserName;
                tbUserNameSettings.Text = LogInControl.CurrentUser.UserName;
                tbUserPasswordSettings.Password = LogInControl.CurrentUser.USerPassword;
                if(LogInControl.CurrentUser.IsAdmin)
                    tabDbViewer.Visibility = Visibility.Visible;
                else
                    tabDbViewer.Visibility = Visibility.Collapsed;


                //TabMain.Items.Add();
            }

            else
            {
                tbCurrentUser.Text = "Log In";
                tbUserNameSettings.Text = "";
                tbUserPasswordSettings.Password = "";
            }
        }
        private void SetUSerData(bool isLogedIn)
        {
            if (isLogedIn)
            {
                tbUserNameSettings.Text = LogInControl.CurrentUser.UserName;
                tbUserPasswordSettings.Password = LogInControl.CurrentUser.USerPassword;
            }
            else
            {
                tbUserNameSettings.Text = "";
                tbUserPasswordSettings.Password = "";
            }
            
        }

        private void btModifyUserCredentials_Click(object sender, RoutedEventArgs e)
        {
            if(LogInControl.CurrentUser.IsLoggedIn)
            {
                logInControl.UpdateUsersDB(tbUserNameSettings.Text, tbUserPasswordSettings.Password.ToString(), LogInControl.CurrentUser.UserName);
                tbCurrentUser.Text = tbUserNameSettings.Text;
            }
        }
        private void LogInControl_DbUpdated(object sender, DbUpdateArgs e)
        {
            dbTable.DataContext =e.DView;
        }

        private void tgColor_Click(object sender, RoutedEventArgs e)
        {
            Helper.setTheme(tgColor.IsChecked ?? false);
        }
    }
}
