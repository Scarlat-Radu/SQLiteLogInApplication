using SuperSimpleTcp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_SQLite_Demo
{

    public class ServerClass
    {
        public  static SimpleTcpServer server { get; set; }
        public event EventHandler<ServerChangedStatusArgs> ServerStatus;

        public ServerClass(string serverAddress)
        {
            server = new SimpleTcpServer(serverAddress);
            server.Events.ClientConnected += Events_ClientConnected;
            server.Events.ClientDisconnected += Events_ClientDisconnected;
            server.Events.DataReceived += Events_DataReceived;
        }

        private void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {

        }

        private void Events_ClientDisconnected(object sender, ConnectionEventArgs e)
        {

        }

        private void Events_ClientConnected(object sender, ConnectionEventArgs e)
        {

        }

        internal void Start()
        {
           server.Start();
            ServerStatus?.Invoke(this, new ServerChangedStatusArgs() { IsServerStarted = true});

        }
        internal void Stop()
        {
            server.Stop();
            ServerStatus?.Invoke(this, new ServerChangedStatusArgs() { IsServerStarted = false });

        }

        public class ServerChangedStatusArgs
        {
            public bool IsServerStarted { get; set; }
        }
    }

   
}
