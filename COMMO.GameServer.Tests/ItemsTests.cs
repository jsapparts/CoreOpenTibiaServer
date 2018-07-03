using COMMO.GameServer.Items;
using Xunit;
using System;

namespace COMMO.GameServer.Tests {
	public sealed class ItemsTests {
		[Fact]
		public void CheckPrototype() {
			ItemManager.LoadItems();
			Assert.Equal(ItemManager.Loaded, true);
		}
	}
}