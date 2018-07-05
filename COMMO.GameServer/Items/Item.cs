using System.Collections.Generic;

namespace COMMO.GameServer.Items {
    public class Item {
        private readonly ushort PrototypeID;

        public IBaseItemPrototype Prototype => ItemManager.GetItemPrototype(PrototypeID);
        public IBaseItemComponent Component => Prototype.ItemComponent;

        // Store the custom properties of an Item
        private Dictionary<string, string> _CustomProperties;

        public Item(ushort protoId) {
            PrototypeID = protoId < ItemManager.FirstItemId ? ItemManager.FirstItemId : protoId;
            _CustomProperties = new Dictionary<string, string>();
        }

        public bool SetAttribute(string key, string value) {
            // Make it always set what was passed. Overwrite
            return _CustomProperties.TryAdd(key, value);
        }
        // Rename to property both get and set // always to lower in key
        public bool GetAttribute(string key, out string value) {
            value = string.Empty;
            if (_CustomProperties.ContainsKey(key)) {
                value = _CustomProperties[key];
                return true;
            }

            return false;
        }

        public bool GetNumberAttribute(string key, out int value) {
            value = 0;
            string str_value;
            if (GetAttribute(key, out str_value)) {
                if (int.TryParse(str_value, out value))
                    return true;
            }

            return false;
        }

        public bool GetBoolAttribute(string key, out bool value) {
            value = false;
            string str_value;
            if (GetAttribute(key, out str_value)) {
                if (bool.TryParse(str_value, out value))
                    return true;
            }

            return false;
        }

        public string GetName() {
            string value;
            if (!GetAttribute("name", out value))
                value = Prototype.Name;

            return value;
        }
    }
}