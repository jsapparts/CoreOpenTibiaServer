namespace COMMO.GameServer.Items {
    public class WriteInfo {
        public bool Write {get;}
        public ushort WriteOnceItemId {get;}
        public ushort MaximumTextLenght {get;}

        public WriteInfo(bool write = false, ushort writeOnceItemId = 0, ushort maximumTextLenght = 512) {
            Write = write;
            WriteOnceItemId = writeOnceItemId;
            MaximumTextLenght = maximumTextLenght;
        }
    }
}