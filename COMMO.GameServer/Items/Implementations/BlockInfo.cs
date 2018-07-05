namespace COMMO.GameServer.Items {
    public class BlockInfo {
        public bool BlockMovement {get;}
        public bool BlockProjectile {get;}
        public bool BlockVision {get;}
        public bool BlockPathfind {get;}

        public BlockInfo(bool move = false, bool projectile = false, bool vision = false, bool path = false) {
            BlockMovement = move;
            BlockProjectile = projectile;
            BlockVision = vision;
            BlockPathfind = path;
        }

        public BlockInfo(BlockInfo block, bool move = false, bool projectile = false, bool vision = false, bool path = false) :
        this(block.BlockMovement || move, block.BlockProjectile || projectile, block.BlockVision || vision, block.BlockPathfind || path) {}
    }
}