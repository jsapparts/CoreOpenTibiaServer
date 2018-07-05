namespace COMMO.GameServer.Items {
    public class JewelComponent : EquipableComponent {
        new public ComponentType ComponentType => ComponentType.Jewel;

        public readonly byte Charges;

        public JewelComponent(EquipableComponent equip, byte charges = 0) : base(equip) {
            Charges = charges;
        }
    }
}