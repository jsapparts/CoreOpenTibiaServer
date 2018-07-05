using System.Collections.Generic;
using System;

namespace COMMO.GameServer.Items {
    public class Container : Item {
        private List<Item> _items;

        private byte maxSize;

        public Container(ushort protoId) : base(protoId) {
            var protoCon = base.Prototype.ItemComponent as ContainerComponent;
            maxSize = protoCon != null ? protoCon.Size : (byte) 0;
            _items = new List<Item>(maxSize);
        }

        public bool CanAdd() {
            if (_items.Count < maxSize)
                return true;
            return false;
        }
        // Create parent, and parent methods to get all itens of master container, in all child containers
        // also, check max weight of container, or hold a link to the HOLDER of container, which can be only a player
        // and base the max itens and weight based on it

        public bool Add(Item item, byte pos = 0) {
            if (!CanAdd())
                return false;

            _items.Add(item);

            return true;
        }

        public bool Remove(Item item) {
            if (!_items.Contains(item))
                return false;
        
            return _items.Remove(item);
        }

        public Item RemoveAt(byte index = 0) {
            if (index >= maxSize || _items[index] == null)
                return null;

            var item = _items[index];
            _items.RemoveAt(index);
            return item;
        }

        public byte WhereIs(Item item) => (byte)_items.IndexOf(item);

        public void ListContent() {
            Console.WriteLine("Content of " + GetName() + ". Capacity: " + maxSize + " -> Items Inside: " + Count());
            foreach(Item item in _items) {
                Console.WriteLine(WhereIs(item) + " - " + item.Prototype.ServerID + " : " + item.GetName());
            }
        }

        public byte Count() => (byte) _items.Count;
        public byte Capacity() => maxSize;
    }
}