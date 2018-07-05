using COMMO.GameServer.Items;
using Xunit;
using System;

namespace COMMO.GameServer.Tests {
	public sealed class ItemsTests {
		[Fact]
		public void TestLoadItems() {
			ItemManager.LoadItems();
			Assert.Equal(true, ItemManager.Loaded);
		}

		[Fact]
		public void TestDustbin() {
			ItemManager.LoadItems();
			Assert.Equal("dustbin", ItemManager.GetItemPrototype(1777).Name);
		}

		[Fact]
		public void TestWorldItemContainer() {
			ItemManager.LoadItems();
			var item = ItemManager.GetItemPrototype(1749);
			Assert.Equal(true, item is WorldItemPrototype);
			var container = item.ItemComponent as ContainerComponent;
			Assert.Equal(15, container.Size);
		}

		[Fact]
		public void TestSpikes() {
			ItemManager.LoadItems();
			var item = ItemManager.GetItemPrototype(1513);
			Assert.Equal("spikes", item.Name);
			Assert.Equal(4, (int) item.DecayInfo.DecayAfter);
			Assert.Equal(1512, (int) item.DecayInfo.DecayTo);
		}
	}
}