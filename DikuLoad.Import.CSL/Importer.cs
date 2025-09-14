using DikuLoad.Data;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace DikuLoad.Import.CSL
{
	public class Importer : BaseImporter
	{
		public ImporterSettings Settings { get; private set; }

		public Importer(ImporterSettings settings)
		{
			Settings = settings ?? throw new ArgumentNullException(nameof(settings));
		}


		private void ProcessRooms(XElement root, Area area)
		{
			Log("Rooms");

			var roomsElement = root.Element("Rooms");
			foreach (var roomElement in roomsElement.Nodes().OfType<XElement>())
			{
				var vnum = roomElement.EnsureInt("VNum");
				var name = roomElement.GetString("Name");
				Log($"Room {name} (#{vnum})");

				var room = new Room
				{
					VNum = vnum,
					Name = name,
					Description = roomElement.GetString("Description"),
					SectorType = Enum.Parse<SectorType>(roomElement.GetString("Sector")),
					Flags = roomElement.ParseFlags<RoomFlags>("Flags")
				};

				var exitsElement = roomElement.Element("Exits");
				foreach (var exitElement in exitsElement.Nodes().OfType<XElement>())
				{
					var exit = new RoomExit
					{
						Direction = exitElement.EnsureEnum<Direction>("Direction"),
						Display = exitElement.GetString("Display"),
						Description = exitElement.GetString("Description"),
						Keyword = exitElement.GetString("Keywords"),
						Flags = exitElement.ParseFlags<RoomExitFlags>("Flags")
					};

					var exitInfo = new RoomExitInfo
					{
						SourceRoom = room,
						RoomExit = exit
					};

					var keyVNum = exitElement.GetInt("Keys", -1);
					if (keyVNum != -1)
					{
						exitInfo.KeyObjectVNum = keyVNum;
					}

					var targetVnum = exitElement.EnsureInt("Destination");
					if (targetVnum != -1 && targetVnum != 0)
					{
						exitInfo.TargetRoomVNum = targetVnum;
					}

					if (exit.Flags.Contains(RoomExitFlags.Locked))
					{
						// Add corresponding reset
						var reset = new AreaReset
						{
							ResetType = AreaResetType.Door,
							Value2 = vnum,
							Value3 = (int)exit.Direction,
							Value4 = 2
						};

						area.Resets.Add(reset);
					}

					AddRoomExitToCache(exitInfo);

				}

				// Extra description
				var extraDescsElement = roomElement.Element("ExtraDescriptions");
				if (extraDescsElement != null && extraDescsElement.Nodes().OfType<XElement>().Count() > 0)
				{
					var extraDesc = extraDescsElement.Nodes().OfType<XElement>().First();
					room.ExtraKeyword = extraDesc.GetString("Keyword");
					room.ExtraDescription = extraDesc.GetString("Description");
				}

				if (!CheckForbidden(name))
				{
					area.Rooms.Add(room);
					AddRoomToCache(vnum, room);
				}
			}
		}

		private void ProcessMobiles(XElement root, Area area)
		{
			Log("Mobiles");

			var mobilesElement = root.Element("NPCs");
			foreach (var mobileElement in mobilesElement.Nodes().OfType<XElement>())
			{
				var vnum = mobileElement.EnsureInt("Vnum");
				var name = mobileElement.GetString("name");
				Log($"Mobile {name} (#{vnum})");

				var mobile = new Mobile
				{
					VNum = vnum,
					Name = name,
					ShortDescription = mobileElement.GetString("shortDescription"),
					LongDescription = mobileElement.GetString("longDescription"),
					Description = mobileElement.GetString("description"),
					Race = mobileElement.GetString("race"),
					Flags = mobileElement.ParseFlags<MobileFlags>("flags"),
					AffectedByFlags = mobileElement.ParseFlags<AffectedByFlags>("affectedBy"),
					Alignment = mobileElement.EnsureEnum<Alignment>("alignment"),
					Level = mobileElement.EnsureInt("level"),
					HitDice = mobileElement.EnsureDice("HitPointDice"),
					ManaDice = mobileElement.EnsureDice("ManaPointDice"),
					ImmuneFlags = mobileElement.ParseFlags<ResistanceFlags>("immune"),
					ResistanceFlags = mobileElement.ParseFlags<ResistanceFlags>("resist"),
					VulnerableFlags = mobileElement.ParseFlags<ResistanceFlags>("vulnerable"),
					Position = mobileElement.GetString("DefaultPosition"),
					Sex = mobileElement.GetString("Sex"),
					Size = mobileElement.GetString("Size"),
					Guild = mobileElement.GetString("Guild"),
					ArmorClassPierce = mobileElement.EnsureInt("ArmorPierce"),
					ArmorClassBash = mobileElement.EnsureInt("ArmorBash"),
					ArmorClassSlash = mobileElement.EnsureInt("ArmorSlash"),
					ArmorClassExotic = mobileElement.EnsureInt("ArmorExotic"),
				};

				mobile.SetAttack(mobileElement.GetString("WeaponDamageMessage"),
					mobileElement.EnsureInt("hitroll"),
					mobileElement.GetDice("DamageDice", new Dice(1, 1, 1)));

				var gold = mobileElement.GetInt("gold", 0);
				var silver = mobileElement.GetInt("silver", 0);

				mobile.Wealth = gold * 10 + silver;

				if (!CheckForbidden(name))
				{
					area.Mobiles.Add(mobile);
					AddMobileToCache(vnum, mobile);
				}
			}
		}

		private void ProcessObjects(XElement root, Area area)
		{
			Log("Objects");

			var itemsElement = root.Element("Items");
			foreach (var itemElement in itemsElement.Nodes().OfType<XElement>())
			{
				var vnum = itemElement.EnsureInt("VNum");
				var name = itemElement.GetString("name");
				Log($"Item {name} (#{vnum})");

				var obj = new GameObject
				{
					VNum = vnum,
					Name = name,
					ShortDescription = itemElement.GetString("ShortDescription"),
					LongDescription = itemElement.GetString("LongDescription"),
					Description = itemElement.GetString("Description"),
					Material = itemElement.GetString("Material"),
					Level = itemElement.GetInt("Level"),
					Value1 = itemElement.GetString("ArmorPierce"),
					Value2 = itemElement.GetString("DiceCount"),
					Value3 = itemElement.GetString("DiceSides"),
					Value5 = itemElement.GetString("DiceBonus"),
					ItemType = itemElement.GetEnum<ItemType>("ItemTypes"),
					WearFlags = itemElement.GetEnum<ItemWearFlags>("WearFlags"),
				};

				if (!CheckForbidden(name))
				{
					area.Objects.Add(obj);
					AddObjectToCache(vnum, obj);
				}

				var affectsElement = itemElement.Element("Affects");
				foreach (var affectElement in affectsElement.Nodes().OfType<XElement>())
				{
					var effect = new GameObjectEffect
					{
						EffectType = affectElement.GetEnum<EffectType>("location"),
						Modifier = affectElement.GetInt("modifier")
					};

					obj.Effects.Add(effect);
				}
			}

		}

		private void ProcessResets(XElement root, Area area)
		{
			Log("Resets");

			var resetsElement = root.Element("Resets");

			int? mobileId = null;
			foreach (var resetElement in resetsElement.Nodes().OfType<XElement>())
			{
				var reset = new AreaReset
				{
					ResetType = resetElement.EnsureEnum<AreaResetType>("Type"),
					Value2 = resetElement.EnsureInt("Vnum"),
					Value4	 = resetElement.EnsureInt("Destination"),
				};

				if ((reset.ResetType == AreaResetType.Equip || reset.ResetType == AreaResetType.Give) && mobileId != null)
				{
					reset.MobileVNum = mobileId.Value;
				}

				if (reset.ResetType == AreaResetType.NPC)
				{
					mobileId = reset.Value2;
					reset.MobileVNum = reset.Value2;
				}

				area.Resets.Add(reset);
			}
		}

		public override void Process()
		{
			var areaFiles = Directory.EnumerateFiles(Settings.InputFolder, "*.xml", SearchOption.AllDirectories).ToArray();

			foreach (var areaFile in areaFiles)
			{
				var xDoc = XDocument.Load(areaFile);
				var root = xDoc.Root;

				if (root.Name != "Area")
				{
					continue;
				}

				Log($"Processing {areaFile}...");

				// Area data
				var areaData = root.Element("AreaData");
				var area = new Area
				{
					Filename = areaFile,
					Name = areaData.GetString("Name"),
					Credits = areaData.GetString("Credits"),
					Builders = areaData.GetString("Builders")
				};

				if (Settings.AreasNames != null && !Settings.AreasNames.Contains(area.Name))
				{
					Log($"Skipping the area due to the areas names filter");
					continue;
				}

				// Try to get levels range from the credits
				area.ParseLevelsBuilds();

				ProcessRooms(root, area);
				ProcessMobiles(root, area);
				ProcessObjects(root, area);
				ProcessResets(root, area);

				Areas.Add(area);
			}

			// Process directions
			UpdateRoomExitsReferences();

			// Update resets
			UpdateResets();
		}
	}
}
