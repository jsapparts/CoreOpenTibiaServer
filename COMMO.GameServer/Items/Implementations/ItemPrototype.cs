namespace COMMO.GameServer.Items {
    public class ItemPrototype : IBaseItemPrototype {
        public ItemPrototypeType PrototypeType => ItemPrototypeType.NormalItem;
        public ushort ServerID {get;}
        public ushort ClientID {get;}
        public string Name {get; set;}
        public string Article {get; set;}
        public string Plural {get; set;}
        public string Description {get; set;}
        public bool CanUse {get; set;}

        public byte MaxStackSize {get; set;}

        public int Weight {get; set;}

        public IBaseItemComponent ItemComponent {get; set;}
        public DecayInfo DecayInfo {get; set;}
        public WriteInfo WriteInfo {get; set;}

        public ItemPrototype(ushort sId, ushort cId, bool useable) {
            ServerID = sId;
            ClientID = cId;
            CanUse = useable;
            ItemComponent = new NoneComponent();
            DecayInfo = new DecayInfo();
            WriteInfo = new WriteInfo();
        }

        public ItemPrototype(IBaseItemPrototype item) {
            ServerID = item.ServerID;
            ClientID = item.ClientID;
            Name = item.Name;
            Article = item.Article;
            Plural = item.Plural;
            Description = item.Description;
            CanUse = item.CanUse;
            ItemComponent = item.ItemComponent;
            DecayInfo = item.DecayInfo;
            WriteInfo = item.WriteInfo;
        }

        public ItemPrototype(IBaseItemPrototype item, int weight = 0, byte maxStackSize = 100) : this(item) {
            Weight = weight;
            MaxStackSize = maxStackSize;
        }
    }
}