using Dragablz;
using SuperSimpleTcp;
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
using static WPF_SQLite_Demo.Helper;

namespace WPF_SQLite_Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Writen by Scarla Mihai-Radu in 06/19/2022
    /// The app purpose is to: 
    /// - Interact with a local SQLite database.
    /// - Create a modern UI interface with material design.
    /// - Try to integrate model view view model components.
    /// Material design and dragable tabs components are used from https://github.com/ButchersBoy/Dragablz, https://dragablz.net/2015/01/06/dragablz-meets-mahapps/,https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// DataView Constructor used to store data base content.
        /// </summary>
        public static DataView DbUsersDataTable { get; set; } = new DataView();
        public static SimpleTcpClient client { get; set; }
        public static SimpleTcpServer server { get; set; }
        public static List<UserClass> UserList { get; set; } = new List<UserClass>();
        public List<string> DataReceived = new List<string>();
        int ClientCount = 0;
        public ServerClass Server;
        public ClientClass Client;
        public static bool ServerIsStarted = false;
        public static bool ClientIsConnected = false;
        //public List<string> UsersList { get; set; } = new List<string>();

        /// <summary>
        /// Dragablz constructor used when open a new tab from the default add button.
        /// </summary>
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

            this.DataContext = this;
            InitializeComponent();
            //DataContext = this;
            DbUsersDataTable = logInControl.LoadData();
            TabMain.NewItemFactory = ItemFactory;
            logInControl.UserLogedOut += LogInControl_UserLogedOut;
            logInControl.DbUpdated += LogInControl_DbUpdated;
            Helper.mainWindow.Add(this);
          //  lbConnectedUsersList.ItemsSource = UsersList;

        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Helper.FirstStart == false)
            {
                if (LogInControl.CurrentUser.IsLoggedIn)
                    tbCurrentUser.Text = LogInControl.CurrentUser.UserName;
                return;
            }
            dbTable.DataContext = DbUsersDataTable;
            //logInControl.dialogLogin.IsOpen = true;
            statusControl.lMachineIp.Content = Get.LocalIPAddress();
            Helper.FirstStart = false;


            UserList.Add(new UserClass()
            {
                UserName = "TEST"
            });

        }




        #region Apperences 
        /// <summary>
        /// Enable Dark Mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tgColor_Click(object sender, RoutedEventArgs e)
        {
            Helper.setTheme(tgColor.IsChecked ?? false);
        }
        #endregion

        #region SQL Login
        private void bLogIn_Click(object sender, RoutedEventArgs e)
        {
            if (LogInControl.CurrentUser.IsLoggedIn)
            {
                logInControl.ChangeDialog(Visibility.Collapsed);
            }
            else
            {
                logInControl.ChangeDialog(Visibility.Visible);

            }
            logInControl.dialogLogin.IsOpen = !logInControl.dialogLogin.IsOpen;

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


        /// <summary>
        /// Update UI data table db after changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"> DataView parameter used to chnage data table context</param>
        private void LogInControl_DbUpdated(object sender, DbUpdateArgs e)
        {
            dbTable.DataContext = e.DView;
            //dbTable.Items.Refresh();
            //sent update to all
           
        }
        private void btModifyUserCredentials_Click(object sender, RoutedEventArgs e)
        {
            if (LogInControl.CurrentUser.IsLoggedIn)
            {
                logInControl.UpdateUsersDB(tbUserNameSettings.Text, tbUserPasswordSettings.Password.ToString(), LogInControl.CurrentUser.IsAdmin, LogInControl.CurrentUser.UserName);
                tbCurrentUser.Text = tbUserNameSettings.Text;
            }
        }
        /// <summary>
        /// Update event for user logIn/Out status.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogInControl_UserLogedOut(object sender, UserLogInOutStatetArgs e)
        {
            if (e.IsLogedIn)
            {
                tbCurrentUser.Text = LogInControl.CurrentUser.UserName;
                tbUserNameSettings.Text = LogInControl.CurrentUser.UserName;
                tbUserPasswordSettings.Password = LogInControl.CurrentUser.USerPassword;
                if (LogInControl.CurrentUser.IsAdmin)
                    tabDbViewer.Visibility = Visibility.Visible;
                else
                    tabDbViewer.Visibility = Visibility.Collapsed;


                TabMain.Items.Add(new TabItem()
                {
                    Header = "Chat Room",
                    IsSelected = true,
                    //Content = sensor
                });
            }

            else
            {
                Helper.mainWindow[0].tbCurrentUser.Text = "Log In";
                Helper.mainWindow[0].tbUserNameSettings.Text = "";
                Helper.mainWindow[0].tbUserPasswordSettings.Password = "";

                //Remove all tabs at logout
                while (Helper.mainWindow[0].TabMain.Items.Count != 1)
                    Helper.mainWindow[0].TabMain.Items.RemoveAt(Helper.mainWindow[0].TabMain.Items.Count - 1);
                //Close draged tabs
                foreach (MainWindow mw in Helper.mainWindow.Skip(1))
                    mw.Close();
                //remove duplicated main windows
                while (Helper.mainWindow.Count != 1)
                    Helper.mainWindow.RemoveAt(Helper.mainWindow.Count - 1);
            }
        }
        private void bRemoveNewUser_Click(object sender, RoutedEventArgs e)
        {
            int rowCount = dbTable.Items.Count;
            if (rowCount > 1)
            {
                string command = "DELETE FROM Users where UserName='" + tbAddUserName.Text + "'";
                logInControl.ExecuteQuery(command);
            }
        }

        private void bModifyUser_Click(object sender, RoutedEventArgs e)
        {
            logInControl.UpdateUsersDB(tbAddUserName.Text, tbUAddserPassword.Text, (bool)ckbAddAsAdmin.IsChecked, selectedUser);
        }

        private void bAddNewUser_Click(object sender, RoutedEventArgs e)
        {
            string uName = tbAddUserName.Text;
            string uPass = tbUAddserPassword.Text;
            int isAdmin = Convert.ToInt32((bool)ckbAddAsAdmin.IsChecked);
            if (uName != string.Empty && uPass != string.Empty)
            {
                if (!CheckNameTaken(uName))
                {
                    string Command = "Insert into Users (UserName, UserPassword,IsAdmin,IsOnline)" + "values('" + uName + "','" + uPass + "','" + isAdmin + "','" + 0.ToString() + "')";
                    logInControl.ExecuteQuery(Command);
                }
                else
                    MessageBox.Show("Name already take.");


            }
        }

        string selectedUser = "";
        private void dbTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int currentRowIndex = dbTable.Items.IndexOf(dbTable.CurrentItem);
            List<string> userInfo = new List<string>();
            DataGridRow row = (DataGridRow)dbTable.ItemContainerGenerator.ContainerFromIndex(currentRowIndex);
            try
            {
                foreach (DataGridColumn column in dbTable.Columns)
                {
                    if (column.GetCellContent(row) is TextBlock)
                    {
                        TextBlock cellContent = column.GetCellContent(row) as TextBlock;
                        userInfo.Add(cellContent.Text);
                    }
                }
                selectedUser = userInfo[0];
                tbAddUserName.Text = userInfo[0];
                tbUAddserPassword.Text = userInfo[1];
                ckbAddAsAdmin.IsChecked = Convert.ToBoolean(int.Parse(userInfo[2]));
            }
            catch (Exception)
            {

            }
        }
        private bool CheckNameTaken(string uInput)
        {
            var rows = Helper.Get.DataGridRows(dbTable);

            foreach (DataGridRow row in rows)
            {
                DataRowView rowView = (DataRowView)row.Item;
                foreach (DataGridColumn column in dbTable.Columns)
                {
                    if (column.GetCellContent(row) is TextBlock)
                    {
                        TextBlock cellContent = column.GetCellContent(row) as TextBlock;
                        int x = column.DisplayIndex;
                        if (cellContent.Text == uInput && x == 0)
                        {
                            return true;
                        }
                    }
                }

            }
            return false;
        }

        #endregion





        #region Server
        private void btStartServer_Click(object sender, RoutedEventArgs e)
        {
            if (ServerIsStarted)
            {
                try
                {
                    DisposeServer();
                    tbInfo.Text += $"Server has stopped...{Environment.NewLine}";
                    ServerIsStarted = false;
                }
                catch (Exception ex)
                {
                    tbInfo.Text += $"{ex.Message}...{Environment.NewLine}";
                }
            }
            else
            {
                try
                {
                    InitServer(tbServerIP.Text);
                    tbInfo.Text += $"Started...{Environment.NewLine}";
                    ServerIsStarted = true;

                }
                catch (Exception ex)
                {
                    tbInfo.Text += $"{ex.Message}...{Environment.NewLine}";
                }
            }
        }

        private void InitServer(string address)
        {
            server = new SimpleTcpServer(address);
            server.Keepalive.EnableTcpKeepAlives = true;

            server.Events.ClientConnected += Evens_ClientConnected;
            server.Events.ClientDisconnected += Events_ClientDisconnected;
            server.Events.DataReceived += Events_ServerDataReceived;
            server.Start();

            ServerIsStarted = true;
        }
        private void DisposeServer()
        {
            server.Events.ClientConnected -= Evens_ClientConnected;
            server.Events.ClientDisconnected -= Events_ClientDisconnected;
            server.Events.DataReceived -= Events_ServerDataReceived;
            server.Stop();
            ServerIsStarted = false;

        }



        private void Evens_ClientConnected(object sender, ConnectionEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                ClientCount++;
                tbInfo.Text += $"{e.IpPort}: connected.{Environment.NewLine}";
                tbInfo.ScrollToEnd();
            }));

        }
        private void Events_ClientDisconnected(object sender, ConnectionEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                ClientCount--;
                tbInfo.Text += $"{e.IpPort}: disconnected.{Environment.NewLine}";
                tbInfo.ScrollToEnd();

                UserList.Remove(UserList.Single(p=>p.IpPort == e.IpPort));
            }));
            RefreshUI();

        }
        private void Events_ServerDataReceived(object sender, DataReceivedEventArgs e)
        {
            DataReceived = new List<string>(ProccesData(e.IpPort, e.Data));

            UpdateInfoBox(DataReceived);
            RefreshUI();

        }



        private List<string> ProccesData(string ipPort, byte[] data)
        {
            List<string> dataReceived = new List<string>();
            int i =0;
            try
            {
                do
                {
                    int l = BitConverter.ToInt16(data, i);i += 4;
                    string userName = Encoding.Default.GetString(data , i, l); i += l;
                    dataReceived.Add(userName);
                    MessageType msgType = (MessageType) BitConverter.ToInt32(data, i); i += 4;
                    dataReceived.Add(msgType.ToString());
                    dataReceived.Add(Encoding.Default.GetString(data, i, data.Length - i));
                    switch (msgType)
                    {
                        case MessageType.Default:
                            //UpdateInfoBox(ipPort, data );
                            break;
                        case MessageType.HandShake:
                            ProccesHandShake(ipPort, data, userName);
                            break;
                        case MessageType.Forward:
                            break;
                        case MessageType.DbUpdate:

                            UpdateDBContent();
                            break;
                        default:
                           // UpdateInfoBox(ipPort, data);
                            break;
                    }

                } while (i > data.Length);
            }
            catch (Exception ex) 
            {
                dataReceived.Add(ex.Message);
                return dataReceived;
                //UpdateInfoBox(ex.Message);
            }
            return dataReceived;
        }

       public void UpdateDBContent()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
               // dbTable.Items.Clear();
                dbTable.DataContext = logInControl.LoadData();
                dbTable.Items.Refresh();
                var v = dbTable.Items.NeedsRefresh;
            }));
        }

        private void ProccesHandShake(string ipPort, byte[] data, string userName)
        {
            UserList.Add(new UserClass()
            {
                UserName = userName,
                IpPort = ipPort,
            });

            //encryption key
        }
        private void RefreshUI()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                lbConnectedUsersList.Items.Refresh();
            }));
        }
        private void tbServerSendMessage_KeyDown(object sender, KeyEventArgs e)
        {
            _ = server.GetClients();
            if (Key.Enter == e.Key)
                if (server.IsListening)
                {
                    if((bool)tgWriteToAll.IsChecked)
                    {
                        var v = server.GetClients();
                        foreach(string s in v )
                        {
                            server.Send(s, tbServerSendMessage.Text);
                            tbInfo.Text += $"Server: {tbServerSendMessage.Text}{Environment.NewLine}";
                            tbInfo.ScrollToEnd();
                        }
                    }else
                    if (!string.IsNullOrEmpty(tbServerSendMessage.Text) && lbConnectedUsersList.SelectedItem !=null)
                    {
                        var v = lbConnectedUsersList.SelectedItem;
                        
                        List<byte> data = new List<byte>();
                        string tempName = "SERVER";

                        data.AddRange(BitConverter.GetBytes(tempName.Length)); //4bytes
                        
                        data.AddRange(Encoding.Default.GetBytes(tempName));//4 bytes
                        data.AddRange(BitConverter.GetBytes((int)MessageType.Default));//4 bytes
                        data.AddRange(Encoding.Default.GetBytes(tbServerSendMessage.Text));//4 bytes

                        server.Send((v as UserClass).IpPort, data.ToArray());


                        //server.Send(lbConnectedUsersList.SelectedItem.ToString(), tbServerSendMessage.Text);
                        tbInfo.Text += $"Server: {tbServerSendMessage.Text}{Environment.NewLine}";
                        tbInfo.ScrollToEnd();
                    }
                    tbServerSendMessage.Text = string.Empty;

                }
        }
        private void UpdateInfoBox(List<string> message)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                tbInfo.Text += message[0] + ": ";

                foreach (string s in message.Skip(2))
                {
                    tbInfo.Text += s + " ";
                }
                tbInfo.Text +=Environment.NewLine;
                tbInfo.ScrollToEnd();
            }));
        }
        public void UpdateInfoBox(string ipPort, byte[] data)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                string s = $"{ipPort}: {Encoding.UTF8.GetString(data)}{Environment.NewLine}";
                string[] split = s.Split(',');
                foreach (string item in split)
                {
                    tbInfo.Text += item + " ";
                    tbInfo.ScrollToEnd();
                }
            }));
        }

        #endregion


        #region Client  

        //TODO Each client should send a default message with their user name when connecting 
        /// <summary>
        /// Create new client object based on Simple tcp class using currret machine addres.
        /// Init Events for the new object
        /// </summary>
        /// <param name="address"></param>
        public void InitClient(string address)
        {
            client = new SimpleTcpClient(address);
            client.Events.Connected += Events_Connected;
            client.Events.Disconnected += Events_Disconnected;
            client.Events.DataReceived += Events_ClientDataReceived;
            client.Connect();

            client.Send(ComposeHandShake());

        }
        private void Events_ClientDataReceived(object sender, DataReceivedEventArgs e)
        {
            DataReceived = ProccesData(e.IpPort, e.Data);
            UpdateClientInfoBox(DataReceived);
            RefreshUI();
        }
        private void UpdateClientInfoBox(List<string> message)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                tbClientInfo.Text += message[0] + ": ";

                foreach (string s in message.Skip(2))
                {
                    tbClientInfo.Text += s + " ";
                }
                tbClientInfo.Text += Environment.NewLine;
                tbClientInfo.ScrollToEnd();
            }));
        }
        public byte[] ComposeHandShake()
        {
            List<byte> data = new List<byte>();
            string tempName = "Radu";

            data.AddRange(BitConverter.GetBytes(tempName.Length)); //4bytes
            data.AddRange(Encoding.Default.GetBytes(tempName));//4 bytes
            data.AddRange(BitConverter.GetBytes((int)MessageType.HandShake));//4 bytes

            return data.ToArray();
        }
        public byte[] ComposeUpdateDB()
        {
            List<byte> data = new List<byte>();
            string tempName = LogInControl.CurrentUser.UserName;

            data.AddRange(BitConverter.GetBytes(tempName.Length)); //4bytes
            data.AddRange(Encoding.Default.GetBytes(tempName));//
            data.AddRange(BitConverter.GetBytes((int)MessageType.DbUpdate));//4 bytes

            return data.ToArray();
        }

        private void Events_Disconnected(object sender, ConnectionEventArgs e)
        {
           

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                tbClientInfo.Text += $"Server disconnected.{Environment.NewLine}";
                LogInControl.CurrentUser.IsOnline = false;
                //Update db 
                logInControl.UpdateUsersDB(LogInControl.CurrentUser.UserName, false);
            }));
            
        }

        private void Events_Connected(object sender, ConnectionEventArgs e)
        {
            LogInControl.CurrentUser.IsOnline = true;
            //Update db 
            logInControl.UpdateUsersDB(LogInControl.CurrentUser.UserName, true);


            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                tbClientInfo.Text += $"Server connected.{Environment.NewLine}";
            }));
        }
        private void btConnecttServer_Click(object sender, RoutedEventArgs e)
        {
            if (ClientIsConnected)
            {
                try
                {
                    if (client != null)
                        client.Send(ComposeUpdateDB());

                    client.Dispose();
                    client.Disconnect();
                    ClientIsConnected = false;

                }
                catch (Exception ex)
                {

                }
            }
            else
                try
                {
                    InitClient(tbClientIp.Text);
                    ClientIsConnected = true;
                    client.Send(ComposeUpdateDB());

                }
                catch (Exception ex)
                {

                }
        }

        private void tbClientSendMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (client.IsConnected)
                {
                    if (!string.IsNullOrEmpty(tbClientSendMessage.Text))
                    {
                        List<byte> data = new List<byte>();
                        string tempName = "Radu";

                        data.AddRange(BitConverter.GetBytes(tempName.Length)); //4bytes
                        data.AddRange(Encoding.Default.GetBytes(tempName));//
                        data.AddRange(BitConverter.GetBytes((int)MessageType.Default));//4 bytes
                        data.AddRange(Encoding.Default.GetBytes(tbClientSendMessage.Text));//4 bytes


                        client.Send(data.ToArray());
                        tbClientInfo.Text += $"Me:{tbClientSendMessage.Text}{Environment.NewLine}";
                        tbClientInfo.ScrollToEnd();
                        tbClientSendMessage.Text = string.Empty;
                    }
                }
            }
        }


        #endregion

        private void Window_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void Window_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {

        }
    }
}
