using System.Collections.Generic;

namespace COMMO.GameServer.Items {
    public class ContainerComponent : IBaseItemComponent {
        public ComponentType ComponentType => ComponentType.Container;

        public byte Size {get;}
        public ContainerType ContainerType {get;}

        public ContainerComponent(byte size) {
            Size = size > 0 ? size : (byte) 1;
        }
    }
}