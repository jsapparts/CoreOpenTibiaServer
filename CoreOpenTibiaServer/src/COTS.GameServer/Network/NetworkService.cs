using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using COTS.Infra.CrossCutting.Security;

namespace COTS.GameServer.Network
{
    public class NetworkService
    {
        private Socket _listener;
        private UInt16 _port;
        private Protocol _protocol;
        private volatile List<Connection> _connections = new List<Connection>();
        private bool _first = true;

        private volatile bool stop = false;

        public NetworkService(Protocol protocol, UInt16 port)
        {
            this._protocol = protocol;
            this._port = port;
        }

        public void Start()
        {
            //await Task.Run(() => StartListening());
            Thread a = new Thread(StartListening);
            a.IsBackground = true;
            a.Start();
            Console.WriteLine("B STARTED");
        }

        private void StartListening()
        {
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _listener.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), _port));
            _listener.Listen(0);

            Thread b = new Thread(UpdateConnections);
            b.IsBackground = true;
            b.Start();

            while(!stop)
            {
                //Socket incoming = _listener.Accept();
                Console.WriteLine("Hello World!");
                //NetworkStream netstream = new NetworkStream(_listener.Accept());
                //NetworkMessage netmsg = new NetworkMessage();
                //netstream.Read(netmsg.Buffer, 0, 1);
                //ProcessMessage(netmsg);
                _connections.Add(new Connection(this, _protocol, _listener.Accept()));
                Console.WriteLine("Connections: " + _connections.Count);
            }

            Console.WriteLine("Closing Socket");
            _listener.Shutdown(SocketShutdown.Both);
            _listener.Close();
        }

        public void UpdateConnections()
        {
            while(!stop)
            {
                //_connections.ForEach( c =>
                //{
                //    c.Receive();
                //});
                List<int> garbage = new List<int>();
                for(int i = 0; i < _connections.Count; i++)
                {
                    Connection connection = _connections[i];
                    if(connection != null && connection.IsConnected())
                        connection.Receive();
                    else
                        garbage.Add(i);
                }

                for(int i = 0; i < garbage.Count; i++)
                {
                    _connections.RemoveAt(garbage[i]);
                    Console.WriteLine("Connection removed.");
                }
                if(garbage.Count > 0)
                    Console.WriteLine("Connections: " + _connections.Count);
                
                //Console.WriteLine("Updt: Connections = " + _connections.Count);
            }
        }

        public void RemoveConnection(Connection connection)
        {
            _connections.Remove(connection);
            Console.WriteLine("Connections: " + _connections.Count);
        }

        public void ProcessMessage(NetworkMessage msg) // Delete. It's now in Connection
        {
            if(_first)
            {
                _protocol.onReceiveFirstMsg(msg);
                _first = false;
            }
            else
                _protocol.onReceiveMsg(msg);
        }

        public void Close()
        {
            Console.WriteLine("Closing NetworkService");
            stop = true;
        }
    }
}