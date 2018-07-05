using System.Collections.Generic;

namespace COMMO.GameServer.Items {
    public class RuneComponent : IBaseItemComponent {
        public ComponentType ComponentType => ComponentType.Rune;

        public readonly uint MinimumRequiredLevel;
        public readonly byte MinimumRequiredMagicLevel;

        private List<byte> _vocationsIdList;
        public void AddAllowedVocation(byte vocationId) => _vocationsIdList.Add(vocationId);
        public void IsAllowedVocation(byte vocationId) => _vocationsIdList.Contains(vocationId); // Should be allowed if vocations is empty

        // Todo: Add Cooldown, GroupCooldown, Group ?and BlockType?
        public RuneComponent(uint level, byte magicLevel) {
            MinimumRequiredLevel = level;
            MinimumRequiredMagicLevel = magicLevel;
        }
    }
}