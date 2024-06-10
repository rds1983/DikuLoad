using DikuLoad.Data;
using DikuLoad.Import;
using System;
using System.IO;
using System.Linq;

namespace AbarimMUD.Import.Envy
{
	public class Importer : BaseImporter
	{
		public ImporterSettings Settings { get; private set; }

		public Importer(ImporterSettings settings)
		{
			Settings = settings ?? throw new ArgumentNullException(nameof(settings));
		}

		private Area CircleReadAreaFromZon(Stream stream)
		{
			var vnum = int.Parse(stream.ReadId());
			var credits = stream.ReadDikuString();
			var name = stream.ReadDikuString();

			var result = new Area
			{
				Name = name.Replace('/', '_'),
				Credits = credits,
				Builders = credits
			};

			if (stream.EndOfStream())
			{
				return result;
			}

			var line = stream.ReadLine();
			var parts = line.Split(' ');

			if (parts.Length > 8)
			{
				result.MinimumLevel = parts[8].Trim();
			}

			if (parts.Length > 9)
			{
				result.MaximumLevel = parts[9].Trim();
			}

			ProcessResets(stream, result);

			return result;
		}

		private void ProcessArea(Stream stream, Area area)
		{
			area.Filename = stream.ReadDikuString();
			area.Name = stream.ReadDikuString();
			area.Credits = stream.ReadDikuString();
			area.ParseLevelsBuilds();

			area.StartVNum = stream.ReadNumber(); // Start vnum
			area.EndVNum = stream.ReadNumber(); // End vnum

			Log($"Created area {area.Name}");
		}

		private void ProcessAreaData(Stream stream, Area area)
		{
			while (true)
			{
				var word = stream.EndOfStream() ? "End" : stream.ReadWord();

				switch (char.ToUpper(word[0]))
				{
					case 'C':
						area.Credits = stream.ReadDikuString();
						area.ParseLevelsBuilds();
						break;
					case 'N':
						area.Name = stream.ReadDikuString();
						break;
					case 'S':
						stream.ReadNumber(); // Security
						break;
					case 'V':
						if (word == "VNUMs")
						{
							stream.ReadNumber();
							stream.ReadNumber();
						}
						break;
					case 'E':
						if (word == "End")
						{
							goto finish;
						}
						break;
					case 'B':
						area.Builders = stream.ReadDikuString();
						break;
				}
			}

		finish:;
			Log($"Processed area data for {area.Name}");
		}

		private void ProcessMobiles(Stream stream, Area area)
		{
			while (!stream.EndOfStream())
			{
				stream.SkipWhitespace();
				var line = stream.ReadLine().Trim();
				if (line.StartsWith("$"))
				{
					break;
				}

				var vnum = int.Parse(line.Substring(1));
				if (vnum == 0 && Settings.SourceType != SourceType.Circle)
				{
					break;
				}

				var name = stream.ReadDikuString().Replace("oldstyle ", "");
				Log($"Processing mobile {name}...");

				var mobile = new Mobile
				{
					VNum = vnum,
					Name = name,
					ShortDescription = stream.ReadDikuString(),
					LongDescription = stream.ReadDikuString(),
					Description = stream.ReadDikuString(),
				};

				if (!CheckForbidden(name))
				{
					area.Mobiles.Add(mobile);
					AddMobileToCache(vnum, mobile);
				}

				if (Settings.SourceType == SourceType.ROM)
				{
					mobile.Race = stream.ReadDikuString();
				}

				OldMobileFlags flags = 0;
				OldAffectedByFlags affectedByFlags = 0;

				var isCircleSimpleMob = true;
				if (Settings.SourceType == SourceType.Circle)
				{
					line = stream.ReadLine().Trim();
					if (line.EndsWith("E"))
					{
						isCircleSimpleMob = false;
					}
				}
				else
				{

					flags = (OldMobileFlags)stream.ReadFlag();
					affectedByFlags = (OldAffectedByFlags)stream.ReadFlag();

					mobile.Alignment = stream.ReadNumber().ToAlignment();

					if (Settings.SourceType == SourceType.ROM)
					{
						mobile.Group = stream.ReadNumber();
					}
					else
					{
						var c = stream.ReadSpacedLetter();
					}
				}

				mobile.Level = stream.ReadNumber();
				mobile.HitRoll = stream.ReadNumber();

				if (Settings.SourceType != SourceType.ROM)
				{
					mobile.ArmorClass = stream.ReadNumber();
				}
				
				mobile.HitDice = stream.ReadDice();

				if (Settings.SourceType == SourceType.ROM)
				{
					mobile.ManaDice = stream.ReadDice();
				}

				mobile.DamageDice = stream.ReadDice();

				var offenseFlags = OldMobileOffensiveFlags.None;
				var immuneFlags = OldResistanceFlags.None;
				var resistanceFlags = OldResistanceFlags.None;
				var vulnerableFlags = OldResistanceFlags.None;
				var formsFlags = FormFlags.None;
				var partsFlags = PartFlags.None;
				if (Settings.SourceType == SourceType.ROM)
				{
					mobile.AttackType = stream.ReadWord();
					mobile.ArmorClassPierce = stream.ReadNumber();
					mobile.ArmorClassBash = stream.ReadNumber();
					mobile.ArmorClassSlash = stream.ReadNumber();
					mobile.ArmorClassExotic = stream.ReadNumber();


					offenseFlags = (OldMobileOffensiveFlags)stream.ReadFlag();
					immuneFlags = (OldResistanceFlags)stream.ReadFlag();
					resistanceFlags = (OldResistanceFlags)stream.ReadFlag();
					vulnerableFlags = (OldResistanceFlags)stream.ReadFlag();

					mobile.StartPosition = stream.ReadWord();
					mobile.Position = stream.ReadWord();
					mobile.Sex = stream.ReadWord();
					mobile.Wealth = stream.ReadNumber();
					mobile.FormFlags = (FormFlags)stream.ReadFlag();
					mobile.PartFlags = (PartFlags)stream.ReadFlag();
					mobile.Size = stream.ReadWord();
					mobile.Material = stream.ReadWord();
				}
				else if (Settings.SourceType == SourceType.Envy)
				{
					mobile.Wealth = stream.ReadNumber();
					mobile.Xp = stream.ReadNumber();
					mobile.Position = stream.ReadWord();
					mobile.Race = stream.ReadDikuString();
					mobile.Sex = stream.ReadWord();
				}
				else if (Settings.SourceType == SourceType.Circle)
				{
					mobile.Wealth = stream.ReadNumber();
					mobile.Xp = stream.ReadNumber();
					mobile.StartPosition = stream.ReadWord();
					mobile.Position = stream.ReadWord();
					mobile.Sex = stream.ReadWord();

					if (!isCircleSimpleMob)
					{
						while (!stream.EndOfStream())
						{
							line = stream.ReadLine().Trim();
							if (line == "E")
							{
								break;
							}
						}
					}
				}

				if (Settings.SourceType == SourceType.ROM)
				{
					// Add race flags
					while (!stream.EndOfStream())
					{
						var c = stream.ReadSpacedLetter();

						if (c == 'F')
						{
							var word = stream.ReadWord();
							var vector = stream.ReadFlag();

							switch (word.Substring(0, 3).ToLower())
							{
								case "act":
									flags &= (OldMobileFlags)(~vector);
									break;
								case "aff":
									affectedByFlags &= (OldAffectedByFlags)(~vector);
									break;
								case "off":
									offenseFlags &= (OldMobileOffensiveFlags)(~vector);
									break;
								case "imm":
									immuneFlags &= (OldResistanceFlags)(~vector);
									break;
								case "res":
									resistanceFlags &= (OldResistanceFlags)(~vector);
									break;
								case "vul":
									vulnerableFlags &= (OldResistanceFlags)(~vector);
									break;
								case "for":
									formsFlags &= (FormFlags)(~vector);
									break;
								case "par":
									partsFlags &= (PartFlags)(~vector);
									break;
								default:
									stream.RaiseError($"Unknown flag {word}");
									break;
							}
						}
						else if (c == 'M')
						{
							var word = stream.ReadWord();
							var mnum = stream.ReadNumber();
							var trig = stream.ReadDikuString();

							Log("Warning: mob triggers are ignored.");
						}
						else
						{
							stream.GoBackIfNotEOF();
							break;
						}
					}
				}

				// Set flags
				mobile.Flags = flags.ToNewFlags();
				var newOffensiveFlags = offenseFlags.ToNewFlags();
				foreach (var f in newOffensiveFlags)
				{
					mobile.Flags.Add(f);
				}

				mobile.AffectedByFlags = affectedByFlags.ToNewFlags();
				mobile.ImmuneFlags = immuneFlags.ToNewFlags();
				mobile.ResistanceFlags = resistanceFlags.ToNewFlags();
				mobile.VulnerableFlags = vulnerableFlags.ToNewFlags();

				if (Settings.SourceType == SourceType.Circle)
				{
					// Optional triggers after the end of the room
					while (!stream.EndOfStream())
					{
						var c = stream.ReadSpacedLetter();
						if (c == 'T')
						{
							// Circle trigger
							var n = stream.ReadNumber();
						}
						else
						{
							stream.GoBackIfNotEOF();
							break;
						}
					}
				}
			}
		}

		private void ProcessObjects(Stream stream, Area area)
		{
			while (!stream.EndOfStream())
			{
				stream.SkipWhitespace();
				var line = stream.ReadLine().Trim();
				if (line.StartsWith("$"))
				{
					break;
				}

				var vnum = int.Parse(line.Substring(1));
				if (vnum == 0 && Settings.SourceType != SourceType.Circle)
				{
					break;
				}

				var name = stream.ReadDikuString();
				Log($"Processing object {name}...");

				var obj = new GameObject
				{
					VNum = vnum,
					Name = name,
					ShortDescription = stream.ReadDikuString(),
					Description = stream.ReadDikuString(),
					Material = stream.ReadDikuString()
				};

				if (!CheckForbidden(name))
				{
					area.Objects.Add(obj);
					AddObjectToCache(vnum, obj);
				}

				obj.ItemType = stream.ReadEnumFromWord<ItemType>();
				if (Settings.SourceType == SourceType.Circle)
				{
					line = stream.ReadLine();
					var parts = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
					if (parts.Length >= 12)
					{
						// 3 flags, each followed by 3 zeroes
						obj.ExtraFlags = (ItemExtraFlags)parts[0].Trim().ParseFlag();
						obj.WearFlags = (ItemWearFlags)parts[4].Trim().ParseFlag();
						obj.AffectedByFlags = ((OldAffectedByFlags)parts[8].ParseFlag(1)).ToNewFlags();
					} else
					{
						if (parts.Length > 0)
						{
							obj.ExtraFlags = (ItemExtraFlags)parts[0].Trim().ParseFlag();
						}

						if (parts.Length > 1)
						{
							obj.WearFlags = (ItemWearFlags)parts[1].Trim().ParseFlag();
						}

						if (parts.Length > 2)
						{
							obj.AffectedByFlags = ((OldAffectedByFlags)parts[2].ParseFlag(1)).ToNewFlags();
						}
					}
				}
				else
				{
					obj.ExtraFlags = (ItemExtraFlags)stream.ReadFlag();
					obj.WearFlags = (ItemWearFlags)stream.ReadFlag();
				}

				if (Settings.SourceType == SourceType.Circle)
				{
					obj.Value1 = stream.ReadWord();
					obj.Value2 = stream.ReadWord();
					obj.Value3 = stream.ReadWord();
					obj.Value4 = stream.ReadWord();

					// Rest
					line = stream.ReadLine();
					var parts = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
					if (parts.Length > 0)
					{
						obj.Weight = int.Parse(parts[0].Trim());
					}

					if (parts.Length > 1)
					{
						obj.Cost = int.Parse(parts[1].Trim());
					}

					if (parts.Length > 3)
					{
						obj.Level = int.Parse(parts[3].Trim());
					}
				}
				else if (Settings.SourceType == SourceType.ROM)
				{
					if (obj.ItemType == ItemType.Armor)
					{
						obj.Value1 = stream.ReadFlag().ToString();
					}
					else
					{
						obj.Value1 = stream.ReadWord();
					}
					obj.Value2 = stream.ReadWord();
					obj.Value3 = stream.ReadWord();
					obj.Value4 = stream.ReadWord();
					obj.Value5 = stream.ReadWord();
					obj.Level = stream.ReadNumber();
				}
				else
				{
					obj.Value1 = stream.ReadDikuString();
					obj.Value2 = stream.ReadDikuString();
					obj.Value3 = stream.ReadDikuString();
					obj.Value4 = stream.ReadDikuString();
				}

				if (Settings.SourceType != SourceType.Circle)
				{
					obj.Weight = stream.ReadNumber();
					obj.Cost = stream.ReadNumber();

					if (Settings.SourceType == SourceType.Envy)
					{
						obj.RentCost = stream.ReadNumber();
					}
					else
					{
						var letter = stream.ReadSpacedLetter();
						switch (letter)
						{
							case 'P':
								obj.Condition = 100;
								break;
							case 'G':
								obj.Condition = 90;
								break;
							case 'A':
								obj.Condition = 75;
								break;
							case 'W':
								obj.Condition = 50;
								break;
							case 'D':
								obj.Condition = 25;
								break;
							case 'B':
								obj.Condition = 10;
								break;
							case 'R':
								obj.Condition = 0;
								break;
							default:
								obj.Condition = 100;
								break;
						}
					}
				}

				while (!stream.EndOfStream())
				{
					var c = stream.ReadSpacedLetter();

					if (c == 'A')
					{
						var effect = new GameObjectEffect
						{
							EffectBitType = EffectBitType.Object,
							EffectType = (EffectType)stream.ReadNumber(),
							Modifier = stream.ReadNumber()
						};

						obj.Effects.Add(effect);
					}
					else if (c == 'F')
					{
						var effect = new GameObjectEffect();
						c = stream.ReadSpacedLetter();
						switch (c)
						{
							case 'A':
								effect.EffectBitType = EffectBitType.None;
								break;
							case 'I':
								effect.EffectBitType = EffectBitType.Immunity;
								break;
							case 'R':
								effect.EffectBitType = EffectBitType.Resistance;
								break;
							case 'V':
								effect.EffectBitType = EffectBitType.Vulnerability;
								break;
							default:
								stream.RaiseError($"Unable to parse effect bit '{c}'");
								break;
						}

						effect.EffectType = (EffectType)stream.ReadNumber();
						effect.Modifier = stream.ReadNumber();
						effect.Bits = (AffectedByFlags)stream.ReadFlag();
						obj.Effects.Add(effect);
					}
					else if (c == 'E')
					{
						obj.ExtraKeyword = stream.ReadDikuString();
						obj.ExtraDescription = stream.ReadDikuString();
					}
					else if (c == 'L' || c == 'C')
					{
						var n = stream.ReadFlag();
					}
					else if (c == 'R' || c == 'D' || c == 'O' || c == 'X' || c == 'M' ||
						c == 'Y' || c == 'J' || c == 'G' || c == 'K' || c == 'V' || c == 'P' || c == 'd')
					{
					}
					else if (c == 'T')
					{
						// Circle trigger
						var n = stream.ReadNumber();
					}
					else
					{
						stream.GoBackIfNotEOF();
						break;
					}
				}
			}
		}

		private void ProcessRooms(Stream stream, Area area)
		{
			while (!stream.EndOfStream())
			{
				stream.SkipWhitespace();
				var line = stream.ReadLine().Trim();
				if (line.StartsWith("$"))
				{
					break;
				}

				var vnum = int.Parse(line.Substring(1));
				if (vnum == 0 && Settings.SourceType != SourceType.Circle)
				{
					break;
				}

				var name = stream.ReadDikuString();
				Log($"Processing room {name} (# {vnum})...");

				var room = new Room
				{
					VNum = vnum,
					Name = name,
					Description = stream.ReadDikuString(),
				};

				if (!CheckForbidden(name))
				{
					area.Rooms.Add(room);
					AddRoomToCache(vnum, room);
				}

				line = stream.ReadLine();
				var parts = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

				var areaNumber = int.Parse(parts[0].Trim());
				room.Flags = ((OldRoomFlags)parts[1].Trim().ParseFlag()).ToNewFlags();

				var add = 2;
				if (parts.Length >= 6)
				{
					// Circle 3 zeroes
					add += 3;
				}

				room.SectorType = stream.ToEnum<SectorType>(parts[add]);

				while (!stream.EndOfStream())
				{
					var c = stream.ReadSpacedLetter();

					if (c == 'S')
					{
						break;
					}
					else if (c == 'H')
					{
						room.HealRate = stream.ReadNumber();
					}
					else if (c == 'M')
					{
						room.ManaRate = stream.ReadNumber();
					}
					else if (c == 'C')
					{
						string clan = stream.ReadDikuString();
					}
					else if (c == 'D')
					{
						var exit = new RoomExit
						{
							Direction = (Direction)stream.ReadNumber(),
							Description = stream.ReadDikuString(),
							Keyword = stream.ReadDikuString(),
						};

						var locks = stream.ReadNumber();

						var exitFlags = OldRoomExitFlags.None;
						switch (locks)
						{
							case 1:
								exitFlags = OldRoomExitFlags.Door;
								break;
							case 2:
								exitFlags = OldRoomExitFlags.Door | OldRoomExitFlags.PickProof;
								break;
							case 3:
								exitFlags = OldRoomExitFlags.Door | OldRoomExitFlags.NoPass;
								break;
							case 4:
								exitFlags = OldRoomExitFlags.Door | OldRoomExitFlags.PickProof | OldRoomExitFlags.NoPass;
								break;
							default:
								break;
						}

						exit.Flags = exitFlags.ToNewFlags();

						var exitInfo = new RoomExitInfo
						{
							SourceRoom = room,
							RoomExit = exit
						};

						var keyVNum = stream.ReadNumber();
						if (exitFlags != OldRoomExitFlags.None && keyVNum != -1)
						{
							exitInfo.KeyObjectVNum = keyVNum;
						}

						var targetVnum = stream.ReadNumber();
						if (targetVnum != -1)
						{
							exitInfo.TargetRoomVNum = targetVnum;
							room.Exits[exit.Direction] = exit;
						}

						AddRoomExitToCache(exitInfo);
					}
					else if (c == 'E')
					{
						room.ExtraKeyword = stream.ReadDikuString();
						room.ExtraDescription = stream.ReadDikuString();
					}
					else if (c == 'O')
					{
						room.Owner = stream.ReadDikuString();
					}
					else
					{
						throw new Exception($"Unknown room command '{c}'");
					}
				}

				if (Settings.SourceType == SourceType.Circle)
				{
					// Optional triggers after the end of the room
					while (!stream.EndOfStream())
					{
						var c = stream.ReadSpacedLetter();
						if (c == 'T')
						{
							// Circle trigger
							var n = stream.ReadNumber();
						}
						else
						{
							stream.GoBackIfNotEOF();
							break;
						}
					}
				}
			}
		}

		private void ProcessResets(Stream stream, Area area)
		{
			int? mobileId = null;
			while (!stream.EndOfStream())
			{
				var line = stream.ReadLine().Trim();
				if (string.IsNullOrEmpty(line))
				{
					continue;
				}

				var c = line[0];
				if (c == 'S')
				{
					break;
				}

				if (c == '*')
				{
					continue;
				}

				var parts = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

				for(var i = 0; i < parts.Length; ++i)
				{
					parts[i] = parts[i].Trim();
				}

				var reset = new AreaReset();
				switch (c)
				{
					case 'M':
						reset.ResetType = AreaResetType.NPC;
						reset.Value1 = int.Parse(parts[1]);
						reset.Value2 = int.Parse(parts[2]);
						reset.Value3 = int.Parse(parts[3]);
						reset.Value4 = int.Parse(parts[4]);

						if (Settings.SourceType == SourceType.ROM)
						{
							reset.Value5 = int.Parse(parts[5]);
						}

						mobileId = reset.Value2;
						break;

					case 'O':
						reset.ResetType = AreaResetType.Item;
						reset.Value1 = int.Parse(parts[1]);
						reset.Value2 = int.Parse(parts[2]);
						reset.Value3 = int.Parse(parts[3]);
						reset.Value4 = int.Parse(parts[4]);
						break;

					case 'P':
						reset.ResetType = AreaResetType.Put;
						reset.Value1 = int.Parse(parts[1]);
						reset.Value2 = int.Parse(parts[2]);
						reset.Value3 = int.Parse(parts[3]);
						reset.Value4 = int.Parse(parts[4]);

						if (Settings.SourceType == SourceType.ROM)
						{
							reset.Value5 = int.Parse(parts[5]);
						}
						break;

					case 'G':
						reset.ResetType = AreaResetType.Give;
						reset.Value1 = int.Parse(parts[1]);
						reset.Value2 = int.Parse(parts[2]);
						reset.Value3 = int.Parse(parts[3]);

						if (mobileId == null)
						{
							stream.RaiseError("Mobile id not set for Give reset type");
						}

						reset.MobileVNum = mobileId.Value;

						break;

					case 'E':
						reset.ResetType = AreaResetType.Equip;
						reset.Value1 = int.Parse(parts[1]);
						reset.Value2 = int.Parse(parts[2]);
						reset.Value3 = int.Parse(parts[3]);
						reset.Value4 = int.Parse(parts[4]);

						if (mobileId == null)
						{
							stream.RaiseError("Mobile id not set for Equip reset type");
						}

						reset.MobileVNum = mobileId.Value;
						break;

					case 'D':
						reset.ResetType = AreaResetType.Door;
						reset.Value1 = int.Parse(parts[1]);
						reset.Value2 = int.Parse(parts[2]);
						reset.Value3 = int.Parse(parts[3]);
						reset.Value4 = int.Parse(parts[4]);

						break;

					case 'R':
						reset.ResetType = AreaResetType.Randomize;
						reset.Value1 = int.Parse(parts[1]);
						reset.Value2 = int.Parse(parts[2]);
						reset.Value3 = int.Parse(parts[3]);

						break;

					case 'T':
						// Trigger
						break;

					default:
						throw new Exception($"Unknown reset command {c}");
				}

				area.Resets.Add(reset);
			}
		}

		private void ProcessShops(Stream stream)
		{
			while (!stream.EndOfStream())
			{
				var keeperVnum = stream.ReadNumber();
				if (keeperVnum == 0)
				{
					break;
				}

				var shop = new Shop
				{
					BuyType1 = stream.ReadNumber(),
					BuyType2 = stream.ReadNumber(),
					BuyType3 = stream.ReadNumber(),
					BuyType4 = stream.ReadNumber(),
					BuyType5 = stream.ReadNumber(),
					ProfitBuy = stream.ReadNumber(),
					ProfitSell = stream.ReadNumber(),
					OpenHour = stream.ReadNumber(),
					CloseHour = stream.ReadNumber(),
				};

				if (Settings.SourceType != SourceType.ROM)
				{
					stream.ReadLine();
				}

				AddShopToCache(keeperVnum, shop);

				Log($"Added shop for mobile {keeperVnum}");
			}
		}

		private void ProcessSpecials(Stream stream)
		{
			while (!stream.EndOfStream())
			{
				var c = stream.ReadSpacedLetter();
				switch (c)
				{
					case 'S':
						return;

					case '*':
						break;

					case 'M':
						var mobVnum = stream.ReadNumber();
						var mobile = GetMobileByVnum(mobVnum);
						if (mobile == null)
						{
							Log($"Warning: could not find mobile with vnum {mobVnum}");
							break;
						}

						var special = new MobileSpecialAttack
						{
							AttackType = stream.ReadWord()
						};

						mobile.SpecialAttacks.Add(special);
						break;
				}

				stream.ReadLine();
			}
		}

		private void ProcessSocials(Stream stream)
		{
			while (!stream.EndOfStream())
			{
				var temp = stream.ReadWord();
				if (temp == "#0")
				{
					break;
				}

				var social = new Social
				{
					Name = temp,
				};

				Log($"Processing social {temp}...");

				do
				{
					string s = string.Empty;
					if (!stream.ReadSocialString(ref s))
					{
						break;
					}
					social.CharNoArgument = s;

					s = string.Empty;
					if (!stream.ReadSocialString(ref s))
					{
						break;
					}
					social.OthersNoArgument = s;

					s = string.Empty;
					if (!stream.ReadSocialString(ref s))
					{
						break;
					}
					social.CharFound = s;

					s = string.Empty;
					if (!stream.ReadSocialString(ref s))
					{
						break;
					}
					social.OthersFound = s;

					s = string.Empty;
					if (!stream.ReadSocialString(ref s))
					{
						break;
					}
					social.VictimFound = s;

					s = string.Empty;
					if (!stream.ReadSocialString(ref s))
					{
						break;
					}
					social.CharAuto = s;

					s = string.Empty;
					if (!stream.ReadSocialString(ref s))
					{
						break;
					}
					social.CharAuto = s;

					s = string.Empty;
					if (!stream.ReadSocialString(ref s))
					{
						break;
					}
					social.OthersAuto = s;
				}
				while (false);

				Socials.Add(social);
			}
		}

		private void SkipSection(Stream stream)
		{
			while (!stream.EndOfStream())
			{
				var c = stream.ReadLetter();
				if (c == '$')
				{
					// Skip ~
					stream.ReadLetter();
					break;
				}
			}
		}

		private void ProcessFile(string areaFile)
		{
			Log($"Processing {areaFile}...");

			var jsonFileName = Path.ChangeExtension(Path.GetFileName(areaFile), "json");

			Area area = null;
			using (var stream = File.OpenRead(areaFile))
			{
				while (!stream.EndOfStream())
				{
					var type = stream.ReadId();

					if (type.StartsWith("AREA") && type.EndsWith("~"))
					{
						var credits = type.Substring(4).RemoveTrailingTilda().Trim();
						area = new Area
						{
							Filename = jsonFileName,
							Credits = credits
						};

						if (!area.ParseLevelsBuilds())
						{
							Log($"WARNING: Couldn't parse levels/builders info from '{credits}'");
						}
						else
						{
							// Area name should be after buildes
							var i = credits.IndexOf(area.Builders);
							credits = credits.Substring(i + area.Builders.Length + 1);
						}

						area.Name = credits.Trim();

						continue;
					}
					else if (type.StartsWith("RECALL"))
					{
						Log($"Skipping '{type}'");
						continue;
					}
					else if (type.StartsWith("VERSION"))
					{
						var data = type.Substring(8);
						if (string.IsNullOrWhiteSpace(data))
						{
							stream.RaiseError($"No area version specified");
						}

						area.Version = int.Parse(data.Trim());
						Log($"Area version: {area.Version}");
						continue;
					}
					else if (type.StartsWith("AUTHOR"))
					{
						var data = type.Substring(7);
						if (string.IsNullOrWhiteSpace(data))
						{
							stream.RaiseError($"No area author specified");
						}

						area.Credits = data.RemoveTrailingTilda().Trim();
						Log($"Area Author: {area.Credits}");
						continue;
					}
					else if (type.StartsWith("RESETMSG"))
					{
						var data = type.Substring(9);
						if (string.IsNullOrWhiteSpace(data))
						{
							stream.RaiseError($"No area author specified");
						}

						area.ResetMessage = data.RemoveTrailingTilda().Trim();
						Log($"Area Reset Message: {area.ResetMessage}");
						continue;
					}

					switch (type)
					{
						case "AREA":
							area = new Area
							{
								Filename = jsonFileName
							};
							ProcessArea(stream, area);
							break;
						case "AREADATA":
							area = new Area
							{
								Filename = jsonFileName
							};
							ProcessAreaData(stream, area);
							break;
						case "MOBILES":
							ProcessMobiles(stream, area);
							break;
						case "OBJECTS":
							ProcessObjects(stream, area);
							break;
						case "ROOMS":
							ProcessRooms(stream, area);
							break;
						case "RESETS":
							ProcessResets(stream, area);
							break;
						case "SHOPS":
							ProcessShops(stream);
							break;
						case "SPECIALS":
							ProcessSpecials(stream);
							break;
						case "SOCIALS":
							ProcessSocials(stream);
							goto finish;
						case "RANGES":
							{
								var r1 = stream.ReadNumber();
								var r2 = stream.ReadNumber();
								var r3 = stream.ReadNumber();
								var r4 = stream.ReadNumber();

								Log($"Parsed levels range: {r1} {r2} {r3} {r4}");
								area.MinimumLevel = r1.ToString();
								area.MaximumLevel = r2.ToString();

								// Skip $
								var c = stream.ReadSpacedLetter();
							}
							break;
						case "$":
							goto finish;
						default:
							Log($"Skipping section {type}");
							SkipSection(stream);
							break;
					}
				}

			finish:;
				if (area != null)
				{
					Areas.Add(area);
				}
			}
		}

		public override void Process()
		{
			Utility.RevertFlag = Settings.SourceType == SourceType.ROM;

			if (Settings.SourceType != SourceType.Circle)
			{
				var areaFiles = Directory.EnumerateFiles(Settings.InputFolder, "*.are", SearchOption.AllDirectories).ToArray();
				foreach (var areaFile in areaFiles)
				{
					var fn = Path.GetFileName(areaFile);
					if (fn == "proto.are")
					{
						Log($"Skipping prototype area {areaFile}");
						continue;
					}

					ProcessFile(areaFile);
				}
			}
			else
			{
				var wldFolder = Path.Combine(Settings.InputFolder, "wld");
				var indexFile = Path.Combine(wldFolder, "index");
				var indexData = File.ReadAllText(indexFile);
				var lines = indexData.Split("\n");

				foreach (var line in lines)
				{
					var areaFileName = line.Trim();
					if (areaFileName == "$")
					{
						break;
					}

					var wldFile = Path.Combine(wldFolder, areaFileName);
					var zonPath = Path.Combine(Settings.InputFolder, "zon");
					zonPath = Path.Combine(zonPath, Path.ChangeExtension(areaFileName, "zon"));
					Area area;
					if (File.Exists(zonPath))
					{
						Log($"Getting area name from {zonPath}...");
						using (var stream = File.OpenRead(zonPath))
						{
							area = CircleReadAreaFromZon(stream);
							area.Filename = wldFile;
							Log($"Area name is '{area.Name}'");
						}
					}
					else
					{
						area = new Area
						{
							Name = Path.GetFileName(wldFile).Replace('/', '_')
						};
					}

					var objPath = Path.Combine(Settings.InputFolder, "obj");
					objPath = Path.Combine(objPath, Path.ChangeExtension(areaFileName, "obj"));
					if (File.Exists(objPath))
					{
						Log($"Loading objects from {objPath}");
						using (var stream = File.OpenRead(objPath))
						{
							ProcessObjects(stream, area);
						}
					}

					var mobPath = Path.Combine(Settings.InputFolder, "mob");
					mobPath = Path.Combine(mobPath, Path.ChangeExtension(areaFileName, "mob"));
					if (File.Exists(mobPath))
					{
						Log($"Loading mobiles from {mobPath}");
						using (var stream = File.OpenRead(mobPath))
						{
							ProcessMobiles(stream, area);
						}
					}

					using (var stream = File.OpenRead(wldFile))
					{
						ProcessRooms(stream, area);
					}

					Areas.Add(area);
				}
			}

			// Process shops
			UpdateShops();

			// Process directions
			UpdateRoomExitsReferences();

			// Update resets
			UpdateResets();

			/*						Log("Locking doors");
									using (var db = new DataContext())
									{
										var areas = (from a in db.Areas select a).ToArray();
										foreach (var area in areas)
										{
											var resets = (from r in db.AreaResets where r.Area == area select r).ToArray();
											foreach (var reset in resets)
											{
												if (reset.ResetType != AreaResetType.Door)
												{
													continue;
												}

												if (reset.Value3 < 0 || reset.Value3 >= 6)
												{
													throw new Exception($"Reset {reset.Id}. Room direction with value {reset.Value3} is outside of range.");
												}

												var dir = (Direction)reset.Value3;

												var roomVnum = reset.Value2;

												var room = (from r in db.Rooms where r.Id == roomVnum select r).FirstOrDefault();
												if (room == null)
												{
													throw new Exception($"Reset {reset.Id}. Can't find room with vnum {roomVnum}");
												}

												var exit = (from e in db.RoomsExits where e.SourceRoomId == room.Id && 
															e.Direction == (Direction)reset.Value3 
															select e).FirstOrDefault();
												if (exit == null || !exit.Flags.HasFlag(RoomExitFlags.Door))
												{
													throw new Exception($"Reset {reset.Id}. Can't find exit {dir}");
												}

												switch (reset.Value4)
												{
													case 0:
														break;
													case 1:
														exit.Flags |= RoomExitFlags.Closed;
														db.SaveChanges();
														break;
													case 2:
														exit.Flags |= RoomExitFlags.Closed | RoomExitFlags.Locked;
														db.SaveChanges();
														break;
													default:
														throw new Exception($"Reset {reset.Id}. Bad locks {reset.Value4}");

												}
											}
										}
									}*/

			Log("Success");
		}
	}
}