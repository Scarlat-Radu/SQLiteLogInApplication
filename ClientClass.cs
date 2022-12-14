using SuperSimpleTcp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPF_SQLite_Demo
{
   public class ClientClass
    {
        public static SimpleTcpClient client { get; set; }
        public static UserClass CurrentUser = new UserClass();


        public event EventHandler<ConnectionChangedStatusArgs> ConnectionChanged;

        public ClientClass(string serverAddres)
        {
            client = new SimpleTcpClient(serverAddres);
            client.Events.Connected += Events_Connected;
            client.Events.Disconnected += Events_Disconnected;
            client.Events.DataReceived += Events_DataReceived;
        }

        private void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Events_Disconnected(object sender, ConnectionEventArgs e)
        {
            ConnectionChanged?.Invoke(this, new ConnectionChangedStatusArgs() { IsConnected = false });
        }

        private void Events_Connected(object sender, ConnectionEventArgs e)
        {
            ConnectionChanged?.Invoke(this, new ConnectionChangedStatusArgs() { IsConnected = true });

        }

        internal void Connect() 
        {
            try
            {
                client.Connect();
            }
            catch (Exception ex)
            {
            }
        }


        public class ConnectionChangedStatusArgs
        {
            public bool IsConnected { get; set; }

        }
    }
}
