using System;
using System.Linq;
using System.Collections.Generic;

namespace COMMO.GameServer {

	public sealed class Program {

		private static void Main(string[] args) {
			Console.WriteLine(FileManager.BaseDirectory);
			Console.WriteLine(FileManager.GetDirectory(FileManager.Dir.ITEMS));
			Console.WriteLine(FileManager.GetFilePath(FileManager.FileType.ITEMS_OTB));
			Items.ItemManager.LoadItems();
			Console.WriteLine("Loaded: " + Items.ItemManager.Loaded);
			Console.WriteLine("Count: " + Items.ItemManager.Count);

			var item = Items.ItemManager.GetItemPrototype(1845);
			Console.WriteLine(item.Name);
			Console.WriteLine(item is Items.ItemPrototype);
		}
	}
}
