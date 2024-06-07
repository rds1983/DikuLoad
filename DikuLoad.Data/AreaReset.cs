namespace DikuLoad.Data
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
		public int Value1 { get; set; }
		public int Value2 { get; set; }
		public int Value3 { get; set; }
		public int Value4 { get; set; }
		public int Value5 { get; set; }
		
		/// <summary>
		/// Actual for Equip and Give reset types
		/// </summary>
		public int MobileVNum { get; set; }

		public override string ToString() => $"{ResetType}, {Value1}, {Value2}, {Value3}, {Value4}, {Value5}, {MobileVNum}";
	}
}
