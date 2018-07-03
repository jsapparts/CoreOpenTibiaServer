using System;
using System.Diagnostics; // For Debug only. Remove later
using System.Xml.Linq;
using System.Collections.Generic;

using COMMO.GameServer.OTBParsing;
using COMMO.GameServer.World.TFSLoading;

namespace COMMO.GameServer.Items {
    public static class ItemManager {
        private static List<IBaseItemPrototype> _items;
        private static bool _loaded = false;
        public const byte FirstItemId = 100;

        public static IBaseItemPrototype GetItemPrototype(ushort id) {
            return _items != null && id < _items.Count ? _items[id - FirstItemId] : null;
        }

        public static int Count => _items != null ? _items.Count : 0;

        public static bool Loaded => _loaded;

        private static bool HasFlag(uint flags, uint flag) {
			return (flags & flag) != 0;
		}

        public static void LoadItems() {
            LoadItemsFromOTB(FileManager.GetFilePath(FileManager.FileType.ITEMS_OTB));
            LoadItemsFromXML(FileManager.GetFilePath(FileManager.FileType.ITEMS_XML));
            _loaded = true;
        }


        private static void LoadItemsFromOTB(string path) {
            var data = FileManager.ReadFileToByteArray(path);
            var tree = new OTBTreeBuilder(data).BuildTree();


            // SKIP to nodes

            _items = new List<IBaseItemPrototype>(30000);

            foreach (var itemNode in tree.Children) {
                var itemStream = new ReadOnlyMemoryStream(itemNode.Data.Span);

                var flags = itemStream.ReadUInt32();

                ushort serverId = 0;
                ushort clientId = 0;
                ushort wareId = 0;
                ushort speed = 0;
                byte lightLevel = 0;
                byte lightColor = 0;
                byte topOrder = 0;

				var toSkip = 0;
				bool escaped = false;
                while (!itemStream.IsOver) {
					var attr = itemStream.ReadByte();
					var isEscape = (OTBMarkupByte)attr == OTBMarkupByte.Escape;
					if (toSkip > 0) {
						if (isEscape) {
							if (escaped) {
								toSkip++;
								escaped = false;
							} else {
								toSkip--;
								escaped = true;
							}
						} else {
							toSkip--;
						}

						if (toSkip == 0)
							break;

						continue;
					}

                    var dataSize = itemStream.ReadUInt16();

                    switch(attr) {
                        case 0x10: // ServerID 0x10 = 16
                        serverId = itemStream.ReadUInt16();
                        break;

                        case 0x11: // ClientID 0x11 = 17
                        clientId = itemStream.ReadUInt16();
                        break;

                        case 0x14: // Speed 0x14 = 20
                        speed = itemStream.ReadUInt16();
                        break;

                        case 0x2A: // LightBlock 0x2A = 42
                        lightLevel = (byte) itemStream.ReadUInt16();
                        lightColor = (byte) itemStream.ReadUInt16();
                        break;

                        case 0x2B: // TopOrder 0x2B = 43
                        topOrder = itemStream.ReadByte();
                        break;

                        case 0x2D: // WareID 0x2D = 45
                        wareId = (byte) itemStream.ReadUInt16();
                        break;

                        default:
						toSkip = dataSize;
                        break;
                    }
                }
                var blockSolid = HasFlag(flags, 1 << 0);
                var blockProjectile = HasFlag(flags, 1 << 1);
                var blockPathFind = HasFlag(flags, 1 << 2);
                //var hasElevation = HasFlag(flags, 1 << 3); // Irrelevant
                var isUsable = HasFlag(flags, 1 << 4);
                var isPickupable = HasFlag(flags, 1 << 5);
                var isMoveable = HasFlag(flags, 1 << 6);
                //var isStackable = HasFlag(flags, 1 << 7);
                var alwaysOnTop = HasFlag(flags, 1 << 13); // Maybe irrelevant
                var isReadable = HasFlag(flags, 1 << 14);
                var isRotatable = HasFlag(flags, 1 << 15);
                var isHangable = HasFlag(flags, 1 << 16);
                var isVertical = HasFlag(flags, 1 << 17);
                var isHorizontal = HasFlag(flags, 1 << 18);
                var allowDistRead = HasFlag(flags, 1 << 20);
                var lookTrough = HasFlag(flags, 1 << 21);
                var isAnimation = HasFlag(flags, 1 << 22); // Maybe irrelevant too?

                IBaseItemPrototype item = null;
                if (!isPickupable) {// A world item
                    item = new WorldItemPrototype(serverId, clientId, isUsable,
                                isMoveable, blockSolid, lookTrough, blockPathFind, blockProjectile);
                } else {// An item
                    item = new ItemPrototype(serverId, clientId, isUsable);
                }

                if(item != null)
                    _items.Add(item);
            }

            _items.TrimExcess();
        }

        private static Dictionary<string, string> properties = new Dictionary<string, string>(); // This is a cache to all xml key-value properties
        private static void LoadItemsFromXML(string path) {
            var rootElement = XElement.Load(path, LoadOptions.SetLineInfo);

            if (rootElement.Name != "items")
                return;

            foreach (var rootChild in rootElement.Elements()) {
                if (rootChild.Name != "item")
                    continue;

                if (rootChild.Attribute("id") == null
                    && (rootChild.Attribute("fromid") == null || rootChild.Attribute("toid") == null))
                    continue;

                var multiple = rootChild.Attribute("id") == null;
                ushort id = 0;
                ushort count = 1;
                if (multiple) {
                    if (!ushort.TryParse(rootChild.Attribute("fromid").Value, out id)
                        || !ushort.TryParse(rootChild.Attribute("toid").Value, out count))
                        continue;

                    if (id > count) {
                        var b = count; count = id; id = b;
                    }

                    count -= id;
                } else {
                    if (!ushort.TryParse(rootChild.Attribute("id").Value, out id))
                        continue;
                }

                if (id > _items.Count || id + count > _items.Count)
                    continue;

                List<IBaseItemPrototype> items = _items.GetRange(id - FirstItemId, count);
                foreach (var item in items) {
                    var _item = item;
                    foreach (var attribute in rootChild.Attributes()) {
                        if (attribute.Name == "name")
                            _item.Name = attribute.Value;
                        else if (attribute.Name == "article")
                            _item.Article = attribute.Value;
                        else if (attribute.Name == "plural")
                            _item.Plural = attribute.Value;
                        // Don't send warning. Just get what we support it's better to do here
                    }

                    properties.Clear();
                    foreach (var child in rootChild.Elements()) {
                        var key = child.Attribute("key");
                        var value = child.Attribute("value");

                        if (key == null || value == null)
                            continue;

                        if (!properties.ContainsKey(key.Value.ToLower()))
                            properties.Add(key.Value.ToLower(), value.Value);
                    }

                    ParseBasic(ref _item);
                    ParseContainerComponent(ref _item);

                    if (ParseItem(ref _item)) {
                        if (ParseEquipableComponent(ref _item)) {
                            ParseMeeleWeaponComponent(ref _item);
                        }
                        //if (ParseEquipableComponent(ref _item))
                            //continue;

                        //continue;
                    }

                    if (ParseWorldItem(ref _item)) {
                        //continue;
                    }

                    /*
                    foreach (var child in rootChild.Elements()) {
                        var key = child.Attribute("key");
                        var value = child.Attribute("value");

                        if (key == null || value == null)
                            continue;

                        switch(key.Value.ToLower()) {
                            case "description":
                            _item.Description = value.Value;
                            break;

                            case "decayto":
                            _item.DecayInfo = new DecayInfo(true, ushort.Parse(value.Value));
                            break;

                            case "duration":
                            _item.DecayInfo = new DecayInfo(true, _item.DecayInfo.DecayTo, uint.Parse(value.Value));
                            break;

                            default:
                            if (ParseItem(ref _item, child)) {
                                // Already know that it is a Normal Item.
                                // Now just keep trying to parse other things of this type of item
                                // As equips, runes and so on.
                                // If parse a rune use continue to ignore others parses for example
                                // This last constinue make it not waste time parsing world things
                                continue;
                            }

                            if (ParseWorldItem(ref _item, child)) {
                                // Same as above. But with world items
                                continue;
                            }

                            //---------------------------------------------------------
                            // This is just to skip somethings. Remove later
                            switch(key.Value.ToLower()) { // Use this to skip some attribute we don't want now
                            case "type": // We already now the type in the way it is now?
                            case "fluidsource": // Used with vials, could be in script
                            case "corpsetype": // Is not really used for nothing
                            case "floorchange":
                            case "pickupable" : // Ignore. Already defined by OTB
                            case "effect" : // Should not be in script? Scripts should be easy
                            continue;
                            }

                            Debug.WriteLine(key.Value + " is not a valid attribute . [Value:" + value.Value + "]");
                            // If reach this point the attribute is unknow. Show warning
                            //----------------------------------------------------------

                            break;
                        }
                    }
                    */
                    _items[_item.ServerID - FirstItemId] = _item;
                }
            }
        }

        private static string TryGetValue(string key, ref bool modified) {
            bool hasKey = ((properties != null) && (properties.ContainsKey(key.ToLower())));
            if (hasKey && !modified)
                modified = true;
            return hasKey ? properties[key.ToLower()] : string.Empty;
        }

        private static bool TryGetBool(string key, ref bool modified) {
            bool value = false;
            string str_value = TryGetValue(key, ref modified);
            if (!bool.TryParse(str_value, out value))
                modified = value = (str_value == "1" ? true : false);

            return value;
        }

        private static int TryGetInt(string key, ref bool modified) {
            int value = 0;
            int.TryParse(TryGetValue(key, ref modified), out value);

            return value;
        }

        private static uint TryGetUInt(string key, ref bool modified) {
            uint value = 0;
            uint.TryParse(TryGetValue(key, ref modified), out value);

            return value;
        }

        private static ushort TryGetUShort(string key, ref bool modified) {
            ushort value = 0;
            ushort.TryParse(TryGetValue(key, ref modified), out value);

            return value;
        }

        private static void ParseBasic(ref IBaseItemPrototype item) {
            bool modified = false;
            string description = TryGetValue("description", ref modified);
            uint duration = TryGetUInt("duration", ref modified);
            ushort toId = TryGetUShort("decayTo", ref modified);
            ushort maxTextLen = TryGetUShort("maxTextLen", ref modified);
            ushort writeOnceId = TryGetUShort("writeOnceItemId", ref modified);

            if (modified) {
                item.Description = description;
                item.DecayInfo = new DecayInfo(toId != 0 && duration != 0, toId, duration);
                item.WriteInfo = new WriteInfo(maxTextLen > 0, writeOnceId, maxTextLen);
            }
        }

        private static bool ParseItem(ref IBaseItemPrototype item) {
            bool modified = false;

            int weight = TryGetInt("weight", ref modified);

            if (modified) {
                item = new ItemPrototype(item, weight);
            }

            return modified;
        }

        private static bool ParseWorldItem(ref IBaseItemPrototype item) {
            bool modified = false;

            ushort destroyTo = TryGetUShort("destroyTo", ref modified);
            ushort rotateTo = TryGetUShort("rotateTo", ref modified);
            ushort transformTo = TryGetUShort("transformTo", ref modified);
            bool blockprojectile = TryGetBool("blockProjectile", ref modified);

            if (modified) {
                item = new WorldItemPrototype(item, destroyTo, rotateTo, transformTo, blockprojectile);
            }

            return modified;
        }

        private static bool ParseContainerComponent(ref IBaseItemPrototype item) {
            bool modified = false;

            byte size = (byte) TryGetUShort("containerSize", ref modified);

            if (modified) {
                item.ItemComponent = new ContainerComponent(size);
            }

            return modified;
        }

        private static bool ParseEquipableComponent(ref IBaseItemPrototype item) {
            bool modified = false;

            uint level = TryGetUInt("requiredLevel", ref modified);
            ushort defense = TryGetUShort("defense", ref modified);

            if (modified) {
                item.ItemComponent = new EquipableComponent(defense : defense, minLevel: level);
            }

            return modified;
        }

        private static bool ParseMeeleWeaponComponent(ref IBaseItemPrototype item) {
            bool modified = false;

            uint attack = TryGetUInt("attack", ref modified);

            if (modified) {
                var equip = item.ItemComponent as EquipableComponent;
                item.ItemComponent = new MeleeWeaponComponent(equip, 1, 1, attack);
            }

            return modified;
        }

        /*
        private static bool ParseItem(ref IBaseItemPrototype item, XElement element) {
            var itProto = new ItemPrototype(item);

            var key = element.Attribute("key");
            var value = element.Attribute("value");
            var modified = true;
            switch(key.Value.ToLower()) {
                case "weight":
                itProto.Weight = int.Parse(value.Value);
                //ItemManager.SetItemIntegerProperty(int.Parse(value.Value))
                break;

                default:
                modified = false;
                break;
            }

            if (modified)
                item = itProto;

            return modified;
        }

        private static bool ParseItem() {
            // interate over the childs with name attribute
            // switch on keys
            // map values to dict
            // if some property was caught, then return a new instance with dict properties or default
            // else don't change the ref item and return false
            return false;
        }

        private static bool ParseWorldItem(ref IBaseItemPrototype item, XElement element) {
            var itProto = new WorldItemPrototype(item);

            var key = element.Attribute("key");
            var value = element.Attribute("value");
            var modified = true;
            switch(key.Value.ToLower()) {
                case "blocking":
                itProto.BlockSolid = true;
                break;

                case "blockprojectile":
                itProto.BlockProjectile = true;
                break;

                default:
                modified = false;
                break;
            }

            if (modified)
                item = itProto;

            return modified;
        }

        private static bool ParseEquipableComponent(ref ItemPrototype prototype, XElement element) {
            var component = new EquipableComponent();

            var key = element.Attribute("key");
            var value = element.Attribute("value");
            var modified = true;
            switch(key.Value.ToLower()) {
                case "":
                component = new EquipableComponent(component.Slot, ushort.Parse(value.Value), component.MinimumRequiredLevel);
                break;

                default:
                modified = false;
                break;
            }

            return modified;
        }
        */

		public static Item createItem(UInt16 protoId) {
			var proto = GetItemPrototype(protoId);
			if (proto != null) {
				var component = proto.ItemComponent as ContainerComponent;
				if (component != null)
					return new Container(protoId);
				else
					return new Item(protoId);
			}

			return null;
		}

    }
}