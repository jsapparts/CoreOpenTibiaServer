namespace COMMO.GameServer.Items {
    public class DecayInfo {
        public bool Decay {get;}
        public ushort DecayTo {get;}
        public uint DecayAfter {get;}

        public DecayInfo(bool decay = false, ushort decayToId = 0, uint decayAfter = 0) {
            Decay = decay;
            DecayTo = decayToId;
            DecayAfter = decayAfter;
        }
    }
}