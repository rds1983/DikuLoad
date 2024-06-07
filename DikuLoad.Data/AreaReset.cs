﻿namespace DikuLoad.Data
{
	public enum AreaResetType
	{
		NPC,
		Item,
		Put,
		Give,
		Equip,
		Door,
		Randomize
	}

	public class AreaReset
	{
		public AreaResetType ResetType { get; set; }
		public int Id1 { get; set; }
		public int Count { get; set; }
		public int Max { get; set; }
		public int Id2 { get; set; }
	}
}
