using DikuLoad.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DikuLoad.Import
{
	public abstract class BaseImporter
	{
		public static string[] ForbiddenWords = { "Allah" };

		private readonly Dictionary<int, Room> _roomsByVnums = new Dictionary<int, Room>();
		private readonly Dictionary<int, Mobile> _mobilesByVnums = new Dictionary<int, Mobile>();
		private readonly Dictionary<int, GameObject> _objectsByVnums = new Dictionary<int, GameObject>();
		private readonly Dictionary<int, Shop> _shopsByKeepersVnums = new Dictionary<int, Shop>();

		private readonly List<RoomExitInfo> _tempDirections = new List<RoomExitInfo>();

		public List<Area> Areas { get; } = new List<Area>();
		public List<Social> Socials { get; } = new List<Social>();

		public static void Log(string message) => ImportUtility.Log(message);

		public Room GetRoomByVnum(int vnum) =>
			(from m in _roomsByVnums where m.Key == vnum select m.Value).FirstOrDefault();

		public Room EnsureRoomByVnum(int vnum) =>
			(from m in _roomsByVnums where m.Key == vnum select m.Value).First();

		public Mobile GetMobileByVnum(int vnum) =>
			(from m in _mobilesByVnums where m.Key == vnum select m.Value).FirstOrDefault();

		public Mobile EnsureMobileByVnum(int vnum) =>
			(from m in _mobilesByVnums where m.Key == vnum select m.Value).First();

		public GameObject GetObjectByVnum(int vnum) =>
			(from m in _objectsByVnums where m.Key == vnum select m.Value).FirstOrDefault();

		public void AddRoomToCache(int vnum, Room room) => _roomsByVnums[vnum] = room;
		public void AddMobileToCache(int vnum, Mobile mobile) => _mobilesByVnums[vnum] = mobile;
		public void AddObjectToCache(int vnum, GameObject obj) => _objectsByVnums[vnum] = obj;
		public void AddRoomExitToCache(RoomExitInfo exitInfo) => _tempDirections.Add(exitInfo);
		public void AddShopToCache(int keeperVNum, Shop shop) => _shopsByKeepersVnums[keeperVNum] = shop;

		protected bool CheckForbidden(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return false;
			}

			var forbiddenWord = (from w in ForbiddenWords where name.Contains(w, StringComparison.OrdinalIgnoreCase) select w).FirstOrDefault();
			if (forbiddenWord != null)
			{
				Log($"Skipping since it contains forbidden word");
				return true;
			}

			return false;
		}

		public void UpdateShops()
		{
			Log("Updating shops");
			foreach (var pair in _shopsByKeepersVnums)
			{
				var keeper = GetMobileByVnum(pair.Key);
				if (keeper == null)
				{
					throw new Exception($"Could not find shop keeper with vnum {pair.Key}");
				}

				keeper.Shop = pair.Value;
			}
		}

		public void UpdateRoomExitsReferences()
		{
			Log("Updating rooms exits references");
			foreach (var dir in _tempDirections)
			{
				var exit = dir.RoomExit;
				if (dir.TargetRoomVNum != null)
				{
					var targetRoom = GetRoomByVnum(dir.TargetRoomVNum.Value);
					if (targetRoom == null)
					{
						Log($"WARNING: Unable to set target room for exit. Room with vnum {dir.TargetRoomVNum.Value} doesnt exist.");
					}

					exit.TargetRoom = targetRoom;
				}

				if (dir.KeyObjectVNum != null && dir.KeyObjectVNum.Value != 0)
				{
					var keyObj = GetObjectByVnum(dir.KeyObjectVNum.Value);
					if (keyObj != null)
					{
						exit.KeyObject = keyObj;
					}
					else
					{
						Log($"WARNING: Unable to find key with vnum {dir.KeyObjectVNum.Value}");
					}
				}

				dir.SourceRoom.Exits[exit.Direction] = exit;
			}
		}

		public void UpdateResets()
		{
			Log("Updating resets");
			foreach (var area in Areas)
			{
				var toDelete = new List<AreaReset>();
				for (var i = 0; i < area.Resets.Count; ++i)
				{
					var reset = area.Resets[i];
					switch (reset.ResetType)
					{
						case AreaResetType.NPC:
							/*							var room = GetRoomByVnum(reset.Id1);
														if (room == null)
														{
															Log($"WARNING: Unable to find room with vnum {reset.Id2} for #{i} reset of area {area.Name}");
															toDelete.Add(reset);
															break;
														}

														var mobile = GetMobileByVnum(reset.Id2);
														if (mobile == null)
														{
															Log($"WARNING: Unable to find mobile with vnum {reset.Id1} for #{i} reset of area {area.Name}");
															toDelete.Add(reset);
															break;
														}
														reset.Id1 = mobile.VNum;
														reset.Id2 = room.VNum;*/
							break;
						case AreaResetType.Item:
							break;
						case AreaResetType.Put:
							break;
						case AreaResetType.Give:
							break;
						case AreaResetType.Equip:
							break;
						case AreaResetType.Door:
							break;
						case AreaResetType.Randomize:
							break;
					}
				}

				foreach (var reset in toDelete)
				{
					area.Resets.Remove(reset);
				}
			}
		}

		public abstract void Process();
	}
}
