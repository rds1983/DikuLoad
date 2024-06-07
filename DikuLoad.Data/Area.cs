using System.Collections.Generic;

namespace DikuLoad.Data
{
	public enum ResetMode
	{
		None,
		ResetIfNoPC,
		ResetAlways
	}

	public class Area
	{

		public string Filename { get; set; }

		public string Name { get; set; }

		public string Credits { get; set; }

		public string Builders { get; set; }
		public int StartVNum { get; set; }
		public int EndVNum { get; set; }
		public int Version { get; set; }
		public string ResetMessage { get; set; }

		public string MinimumLevel { get; set; }

		public string MaximumLevel { get; set; }

		public List<Room> Rooms { get; set; } = new List<Room>();

		public List<Mobile> Mobiles { get; set; } = new List<Mobile>();

		public List<GameObject> Objects { get; set; } = new List<GameObject>();

		public List<AreaReset> Resets { get; set; } = new List<AreaReset>();

		public override string ToString() => $"{MinimumLevel}-{MaximumLevel} {Builders} {Name}";

		public bool CanObjectBePopped(int objectId)
		{
			foreach(var reset in Resets)
			{
				switch (reset.ResetType)
				{
					case AreaResetType.Item:
					case AreaResetType.Put:
					case AreaResetType.Give:
					case AreaResetType.Equip:
						if (reset.Value2 == objectId)
						{
							return true;
						}
						break;
				}
			}

			return false;
		}
	}
}