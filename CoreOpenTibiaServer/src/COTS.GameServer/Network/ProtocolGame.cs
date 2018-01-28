using System;

using COTS.Infra.CrossCutting.Security;

namespace COTS.GameServer.Network
{
    class ProtocolGame : Protocol
    {
        public override String GetProtocolName() => "ProtocolGame";

        public override Byte getProtocolID() => 2;

        public override bool ServerSendFirst() => false;

        public override void onReceiveMsg(NetworkMessage msg)
        {
            Console.WriteLine("Received msg in " + GetProtocolName());
        }

        public override void onReceiveFirstMsg(NetworkMessage msg)
        {
            Console.WriteLine("Received first msg in " + GetProtocolName());
            OS = msg.GetUInt16();
            Version = msg.GetUInt16();

            msg.SkipBytes(7); // U32 client version, U8 client type, U16 dat revision

            Console.WriteLine("Pos:" + msg.Position);
            XTeaKey = GetXTEAKey();
            Console.WriteLine("Pos: " + msg.Position);
        }
    }
}