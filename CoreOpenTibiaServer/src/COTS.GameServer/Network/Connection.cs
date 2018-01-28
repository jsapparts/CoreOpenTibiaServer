using System;
using System.Net.Sockets;
using System.Threading;
using System.Net;

using COTS.Infra.CrossCutting.Security;

namespace COTS.GameServer.Network
{
    public class Connection
    {
        private Socket _socket;
        private Protocol _protocol;
        private NetworkService _networkService;
        private NetworkMessage _networkmessage;
        private OutputMessage _outputmessage;
        private bool _first = true;
        private bool _socket_disposed = false;

        public Connection(NetworkService netServ, Protocol proto, Socket sock)
        {
            Console.WriteLine("New Connection");
            this._networkService = netServ;
            this._protocol = proto;
            this._protocol.Connection = this;
            this._socket = sock;
        }

        public void ProcessMessage(NetworkMessage msg)
        {
            Console.WriteLine("Processing message to " + _protocol.GetProtocolName());
            if(_first)
            {
                _protocol.onReceiveFirstMsg(msg);
                _first = false;
            }
            else // Should decrypt XTEA before send to method
                _protocol.onReceiveMsg(msg);
        }
        XERROR:
        // TODO: Something is not letting Receive works well to ProtocolGame
        public void Receive()
        {
            Console.WriteLine("Receiving message to " + _protocol.GetProtocolName());
            if(!IsConnected() || (_socket.Available <= 0))
                return;

            _networkmessage = new NetworkMessage(new byte[NetworkMessage.HEADER_LENGTH]);
            _socket.Receive(_networkmessage.Buffer);

            UInt16 size = _networkmessage.GetUInt16();
            if(size < NetworkMessage.HEADER_LENGTH + NetworkMessage.CHECKSUM_LENGTH)
                return;

            _networkmessage = new NetworkMessage(new byte[size]);
            _socket.Receive(_networkmessage.Buffer);

            UInt32 checksum = 0;
            Int32 len = _networkmessage.Length - NetworkMessage.CHECKSUM_LENGTH;

            if(len > 0)
                checksum = Tools.AdlerChecksum(_networkmessage.Buffer, NetworkMessage.CHECKSUM_LENGTH, len);

            UInt32 recvchecksum = _networkmessage.GetUInt32();
            if(recvchecksum != checksum)
                return;

            UInt16 protocol = (UInt16) _networkmessage.GetByte();
            if(protocol != _protocol.getProtocolID())
                return;

            ProcessMessage(_networkmessage);
        }

        public void Send()
        {
            OutputMessage output = GetOutputMessage();
            if(output.Length <= 0)
                return;

            output.WriteMessageLength();

            if(!XTea.EncryptXtea(ref output, _protocol.XTeaKey))
                return;

            output.AddCryptoHeader(true);

            _socket.Send(output.Buffer);
            GetOutputMessage().Reset();
        }

        public void Disconnect()
        {
            if(!IsConnected() || !_socket.Connected)
                return;
            
            _socket.Disconnect(false);
            _socket.Close();
            _socket_disposed = true;
        }

        public bool IsConnected()
        {
            if(_socket_disposed)
                return false;

            try
            {
                return !(_socket.Poll(1, SelectMode.SelectRead) && _socket.Available == 0);
            }
            catch(SocketException) { return false; }
        }

        public NetworkMessage GetNetworkMessage() => _networkmessage;

        public OutputMessage GetOutputMessage()
        {
            if(_outputmessage == null)
                _outputmessage = new OutputMessage();

            return _outputmessage;
        }
        
        public UInt32 IPAddr(string ip) // Move this to tools. Or other place. Should not be here
        {
            var ipBytes = IPAddress.Parse(ip).GetAddressBytes();
            var serverIPAdress = (uint)ipBytes[0] << 24;
                serverIPAdress += (uint)ipBytes[1] << 16;
                serverIPAdress += (uint)ipBytes[2] << 8;
                serverIPAdress += (uint)ipBytes[3];
            
            return serverIPAdress;
        }
    }
}