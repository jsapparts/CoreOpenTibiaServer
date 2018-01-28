using System;

using COTS.Infra.CrossCutting.Security;

namespace COTS.GameServer.Network
{
    class ProtocolLogin : Protocol
    {
        public override String GetProtocolName() => "ProtocolLogin";

        public override Byte getProtocolID() => 1;

        public override bool ServerSendFirst() => false;

        public override void onReceiveMsg(NetworkMessage msg)
        {

        }

        public override void onReceiveFirstMsg(NetworkMessage msg)
        {
            Console.WriteLine("First Message Received");

            OS = msg.GetUInt16();
            Version = msg.GetUInt16();

            Console.WriteLine("OS: " + OS);
            Console.WriteLine("Version: " + Version);

            if(Version > 971)
                msg.SkipBytes(17); // Skip ProtocolVersion, DAT/SPR/PIC signatures, 1 byte = 0
            else
                msg.SkipBytes(12); // Skip only DAT/SPR/PIC signatures
            
            if(!msg.RsaDecrypt())
            {
                Connection.Disconnect();
                return;
            }

            UInt32[] key = new UInt32[4];
            key[0] = msg.GetUInt32();
            key[1] = msg.GetUInt32();
            key[2] = msg.GetUInt32();
            key[3] = msg.GetUInt32();
            XTeaKey = key;

            string username = msg.GetString();
            string password = msg.GetString();

            Console.WriteLine("User: " + username + " with Password: " + password);
            if(username != "1" || password != "1")
            {
                disconnectClient("Wrong username or password");
                return;
            }

            GetCharacterList();
        }

        public void GetCharacterList()
        {
            OutputMessage output = Connection.GetOutputMessage();

            output.AddByte(0x14);
            output.AddString("0\nCore Open Tibia Server");

            output.AddByte(0x64);
            output.AddByte(0x01);
            output.AddString("BetaTester");
            output.AddString("COTS Server");
            output.AddUInt32(Connection.IPAddr("127.0.0.1"));
            output.AddInt16(7172);

            output.AddByte(0x01);
            output.AddByte(0x00);

            Connection.Send();
            Connection.Disconnect();
        }
    }
}