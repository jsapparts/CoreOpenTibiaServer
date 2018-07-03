namespace COMMO.GameServer.Items {
    public class DistanceWeaponComponent : EquipableComponent, IBaseWeaponComponent {
        new public ComponentType ComponentType => ComponentType.Weapon;
        public WeaponWieldType WieldType {get;}
        public WeaponType WeaponType => WeaponType.Distance;

        public readonly byte Range;
        public readonly byte HitChance;
        public readonly byte ShootType;
        public readonly uint ManaCost;
        public readonly DistanceWeaponType DistanceWeaponType;

        public DistanceWeaponComponent(EquipableComponent equip, WeaponWieldType wieldType, DistanceWeaponType distType, byte shootT, byte range = 2, byte hitChance = 100, byte manaCost = 0) : base(equip) {
            WieldType = wieldType;
            DistanceWeaponType = distType;
            ShootType = shootT;
            Range = range;
            HitChance = hitChance;
            ManaCost = manaCost;
        }
    }
}