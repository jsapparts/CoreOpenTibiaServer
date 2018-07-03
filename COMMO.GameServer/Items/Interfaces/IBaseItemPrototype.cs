namespace COMMO.GameServer.Items {
    public interface IBaseItemPrototype {
        ItemPrototypeType PrototypeType {get;}
        ushort ServerID {get;}
        ushort ClientID {get;}
        string Name {get; set;}
        string Article {get; set;}
        string Plural {get; set;}
        string Description {get; set;}

        bool CanUse {get;}

        IBaseItemComponent ItemComponent {get; set;}
        DecayInfo DecayInfo {get; set;}
        WriteInfo WriteInfo {get; set;}
    }
}