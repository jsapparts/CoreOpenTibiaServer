namespace COMMO.GameServer.Items {
    public class WorldItemPrototype : IBaseItemPrototype {
        public ItemPrototypeType PrototypeType => ItemPrototypeType.WorldItem;
        public ushort ServerID {get;}
        public ushort ClientID {get;}
        public string Name {get; set;}
        public string Article {get; set;}
        public string Plural {get; set;}
        public string Description {get; set;}
        public bool CanUse {get; set;}
        public readonly bool CanMove;
        public readonly BlockInfo BlockInfo;
        public byte Elevation;
        public byte TopOrder;
        public ushort DestroyTo;
        public ushort RotateTo;
        public ushort TransformTo;

        public IBaseItemComponent ItemComponent {get; set;}
        public DecayInfo DecayInfo {get; set;}
        public WriteInfo WriteInfo {get; set;}

        public WorldItemPrototype(ushort sId, ushort cId, bool useable, bool moveable = false,
                bool blocksolid = false, bool blockvision = false, bool blockpathfind = false,
                bool  blockprojectile = false) {
            ServerID = sId;
            ClientID = cId;
            CanUse = useable;
            ItemComponent = new NoneComponent();
            DecayInfo = new DecayInfo();
            WriteInfo = new WriteInfo();
            BlockInfo = new BlockInfo(blocksolid, blockvision, blockpathfind, blockprojectile);
        }

        public WorldItemPrototype(IBaseItemPrototype item) {
            ServerID = item.ServerID;
            ClientID = item.ClientID;
            Name = item.Name;
            Article = item.Article;
            Plural = item.Plural;
            CanUse = item.CanUse;
            ItemComponent = item.ItemComponent;
            DecayInfo = item.DecayInfo;
            WriteInfo = item.WriteInfo;
        }

        public WorldItemPrototype(IBaseItemPrototype item, ushort destroyTo , ushort rotateTo, ushort transformTo, bool blockprojectile) : this(item) {
            DestroyTo = destroyTo;
            RotateTo = rotateTo;
            TransformTo = transformTo;
            BlockInfo = new BlockInfo(projectile: blockprojectile);
        }
    }
}