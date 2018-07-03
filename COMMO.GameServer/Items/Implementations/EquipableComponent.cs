using System.Collections.Generic;

namespace COMMO.GameServer.Items {
    public class EquipableComponent : IBaseItemComponent {
        public ComponentType ComponentType => ComponentType.Equipable;
        public readonly byte Slot;
        public readonly uint MinimumRequiredLevel;
        public readonly ushort Defense;
        private List<byte> _vocationsIdList;

        public void AddAllowedVocation(byte vocationId) => _vocationsIdList.Add(vocationId);
        public bool CanEquip(uint level, byte vocationId, byte slot) => level >= MinimumRequiredLevel && _vocationsIdList.Contains(vocationId) && slot == Slot; // Or slot == none

        public readonly BonusDamageInfo DamageBonus;
        public readonly BonusDamageInfo DamageDefenseBonus;
        public readonly HabilitiesInfo Habilities;

        public EquipableComponent(byte slot = 0 /*none*/, ushort defense = 0, uint minLevel = 0) {
            Slot = slot;
            Defense = defense;
            MinimumRequiredLevel = minLevel;
            _vocationsIdList = new List<byte>();
        }

        public EquipableComponent(EquipableComponent base_component) {
            Slot = base_component.Slot;
            Defense = base_component.Defense;
            MinimumRequiredLevel = base_component.MinimumRequiredLevel;
            //_vocationsIdList = base_component / Create a getvocation list method
        }
    }
}