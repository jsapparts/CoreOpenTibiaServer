using System;

using COTS.Infra.CrossCutting.Security;

namespace COTS.GameServer.Network
{
    public abstract class Protocol
    {
        private Connection _connection;
        public Connection Connection
        {
            get {return _connection;}
            set {_connection = value;}
        }

        public UInt16 _os;
        public UInt16 OS
        {
            get {return _os;}
            set {_os = value;}
        }

        private UInt16 _version;
        public UInt16 Version
        {
            get {return _version;}
            set {_version = value;}
        }

        private UInt32[] _xteaKey;
        public UInt32[] XTeaKey
        {
            get {return _xteaKey;}
            set {_xteaKey = value;}
        }

        public abstract String GetProtocolName();

        public abstract Byte getProtocolID();
        
        public abstract bool ServerSendFirst();

        public abstract void onReceiveMsg(NetworkMessage msg);

        public abstract void onReceiveFirstMsg(NetworkMessage msg);

        public void disconnectClient(string msg)
        {
            var output = Connection.GetOutputMessage();
            output.AddByte(0x0A);
            output.AddString(msg);

            Connection.Send();
            Connection.Disconnect();
        }

        public UInt32[] GetXTEAKey()
        {
            NetworkMessage msg = Connection.GetNetworkMessage();
            UInt32[] key = new UInt32[4];
            
            key[0] = msg.GetUInt32();
            key[1] = msg.GetUInt32();
            key[2] = msg.GetUInt32();
            key[3] = msg.GetUInt32();

            return key;
        }
    }
}