using System;
using System.Collections.Generic;

namespace DikuLoad.Data
{
	public enum ItemType
	{
		None = 0,
		Light = 1,
		Scroll = 2,
		Wand = 3,
		Staff = 4,
		Weapon = 5,
		Treasure = 8,
		Armor = 9,
		Potion = 10,
		Furniture = 12,
		Trash = 13,
		Container = 15,
		DrinkContainer = 17,
		Key = 18,
		Food = 19,
		Money = 20,
		Boat = 22,
		NpcCorpse = 23,
		PcCorpse = 24,
		Fountain = 25,
		Pill = 26,
	}

	[Flags]
	public enum ItemExtraFlags
	{
		None = 0,
		Glow = 1 << 0,
		Humming = 1 << 1,
		Dark = 1 << 2,
		Lock = 1 << 3,
		Evil = 1 << 4,
		Invisible = 1 << 5,
		Magic = 1 << 6,
		NoDrop = 1 << 7,
		Bless = 1 << 8,
		AntiGood = 1 << 9,
		AntiEvil = 1 << 10,
		AntiNeutral = 1 << 11,
		NoRemove = 1 << 12,
		Inventory = 1 << 13,
		Poisoned = 1 << 14,
		VampireBane = 1 << 15,
		Holy = 1 << 16,
	}

	[Flags]
	public enum ItemWearFlags
	{
		Take = 1 << 0,
		Finger = 1 << 1,
		Neck = 1 << 2,
		Body = 1 << 3,
		Head = 1 << 4,
		Legs = 1 << 5,
		Feet = 1 << 6,
		Hands = 1 << 7,
		Arms = 1 << 8,
		Shield = 1 << 9,
		About = 1 << 10,
		Waist = 1 << 11,
		Wrist = 1 << 12,
		Wield = 1 << 13,
		Hold = 1 << 14,
		NoSac = 1 << 15,
		Float = 1 << 16,
		Light = 1 << 17
	}

	public class GameObject : AreaEntity
	{
		public string Name { get; set; }
		public string ShortDescription { get; set; }
		public string Description { get; set; }
		public string Material { get; set; }
		public ItemType ItemType { get; set; }
		public ItemWearFlags WearFlags { get; set; }
		public ItemExtraFlags ExtraFlags { get; set; }

		public string Value1 { get; set; }
		public string Value2 { get; set; }
		public string Value3 { get; set; }
		public string Value4 { get; set; }
		public string Value5 { get; set; }
		public int Level { get; set; }
		public int Weight { get; set; }
		public int Cost { get; set; }
		public int RentCost { get; set; }
		public int Condition { get; set; }
		public string ExtraKeyword { get; set; }
		public string ExtraDescription { get; set; }
		public List<GameObjectEffect> Effects { get; } = new List<GameObjectEffect>();
	}
}