using DikuLoad.Data;

namespace DikuLoad.Import
{
	public class RoomExitInfo
	{
		public Room SourceRoom { get; set; }
		public RoomExit RoomExit { get; set; }
		public int? TargetRoomVNum { get; set; }
		public int? KeyObjectVNum { get; set; }
	}
}
