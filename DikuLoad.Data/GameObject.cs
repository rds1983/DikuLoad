﻿using System;
using System.Collections.Generic;

namespace DikuLoad.Data
{
	public enum ItemType
	{
		Light,
		Scroll,
		Wand,
		Staff,
		Weapon,
		Treasure,
		Armor,
		Potion,
		Clothing,
		Furniture,
		Trash,
		Container,
		Drink,
		Key,
		Food,
		Money,
		Boat,
		NpcCorpse,
		PcCorpse,
		Fountain,
		Pill,
		Protect,
		Map,
		Portal,
		WarpStone,
		RoomKey,
		Gem,
		Jewelry,
		JukeBox
	}

	public enum WearType
	{
		None = -1,
		Light,
		FingerLeft,
		FingerRight,
		Neck1,
		Neck2,
		Body,
		Head,
		Legs,
		Feet,
		Hands,
		Arms,
		Shield,
		About,
		Waist,
		WristLeft,
		WristRight,
		Wield,
		Hold,
		Float
	}

	public enum WeaponType
	{
		Exotic,
		Sword,
		Mace,
		Dagger,
		Axe,
		Staff,
		Flail,
		Whip,
		Polearm
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
		NoPurge = 1 << 14,
		RotDeath = 1 << 15,
		VisDeath = 1 << 16,
		NonMetal = 1 << 18,
		NoLocate = 1 << 19,
		MeltDrop = 1 << 20,
		HadTimer = 1 << 21,
		SellExtract = 1 << 22,
		BurnProof = 1 << 24,
		NounCurse = 1 << 25,
		Corroded = 1 << 26,
	}

	[Flags]
	public enum ItemWearFlags
	{
		None = 0,
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