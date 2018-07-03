using System.Collections.Generic;

namespace COMMO.GameServer.Items {
    public class AmmunitionComponent : IBaseItemComponent {
        public ComponentType ComponentType => ComponentType.Ammunition;

        public readonly uint Attack;
        public readonly AmmunitionType AmmunitionType;
        public byte ShootType {get;}

        public readonly BonusDamageInfo DamageBonus;

        private List<bool> _conditions; // Burn, Freeze, Poison, Electrify
        public void SetCondition(byte conditionKey, bool value) => _conditions[conditionKey] = value;
        public bool InfligeCondition(byte conditionKey) => _conditions[conditionKey];

        public AmmunitionComponent() {
            //_bonusElementalDamage = new List<ushort>() {0, 0, 0, 0};
            _conditions = new List<bool>() {false, false ,false, false};
        }
    }
}