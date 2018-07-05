namespace COMMO.GameServer.Items {
    public class BonusDamageInfo {
        public readonly bool AsPercent;
        public readonly uint Fire;
        public readonly uint Ice;
        public readonly uint Earth;
        public readonly uint Energy;
        public readonly uint Death;
        public readonly uint Holy;
        public readonly uint Physical;

        public BonusDamageInfo(bool percent = false, uint fire = 0, uint ice = 0, uint earth = 0, uint energy = 0) {
            AsPercent = percent;
            Fire = fire;
            Ice = ice;
            Earth = earth;
            Energy = energy;
        }
    }
}