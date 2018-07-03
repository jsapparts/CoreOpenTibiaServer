namespace COMMO.GameServer.Items {
    public class MeleeWeaponComponent : EquipableComponent, IBaseWeaponComponent {
        new public ComponentType ComponentType => ComponentType.Weapon;
        public WeaponWieldType WieldType {get;}
        public WeaponType WeaponType => WeaponType.Melee;

        public readonly uint Attack;
        public readonly MeleeWeaponType MeleeWeaponType;

        public MeleeWeaponComponent(EquipableComponent equip, byte weaponT, byte wieldT, uint attack) : base(equip) {
            //MeleeWeaponType = weaponT;
            //WieldType = wieldT;
            Attack = attack;
        }
    }
}