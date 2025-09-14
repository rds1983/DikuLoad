using System;
using System.Collections.Generic;

namespace DikuLoad.Data
{
	public enum MobileFlags
	{
		Sentinel,
		StayArea,
		NoWander,
		NPC,
		Bash,
		Berserk,
		Disarm,
		Dodge,
		Kick,
		DirtKick,
		Parry,
		Trip,
		AreaAttack,
		Aggressive,
		Undead,
		Wimpy,
		Warrior,
		Mage,
		Cleric,
		Thief,
		Healer,
		Scavenger,
		ShopKeeper,
		Gain,
		Banker,
		NoPurge,
		GuildMaster,
		NoAlign,
		Pet,
		Fade,
		Changer,
		Rescue,
		Outdoors,
		Indoors,
		NoTrack,
		Practice,
		UpdateAlways,
		Backstab,
		Crush,
		Fast,
		Tail,
		AssistRace,
		AssistAlign,
		AssistPlayer,
		AssistAll,
		AssistGuard,
		AssistId,
		NoKill,
		Spec,
		NiceThief,
		Berserker,
		Pound,
		Slice,
		Stab,
		Pierce,
		Cleave,
		Beat,
		Slash,
		Rake,
		Whip,
		Zap,
	}

	public enum AffectedByFlags
	{
		Infrared,
		Flying,
		DetectIllusion,
		DetectEvil,
		DetectGood,
		DetectHidden,
		DetectInvis,
		DetectMagic,
		Haste,
		Sanctuary,
		Hide,
		PassDoor,
		DarkVision,
		AcuteVision,
		Sneak,
		ProtectEvil,
		ProtectGood,
		Plague,
		Berserk,
		Invisible,
		Swim,
		Slow,
		FaerieFire,
		Regeneration,
		Weaken,
		Blind,
		Poison,
		Curse,
		Camouflage,
		Charm,
		Sleep,
		Calm,
		Liquid,
		AirElm,
		NoBlast,
		NoStab,
		Aether,
		WaterElm,
		HealHurts,
		EarthElm,
		Ethereal,
		Frozen,
		ResistWebs,
		FireElm,
		Aegis,
		Ride,
		Drowning,
		Infravision,
		Gate,
		Climb,
		Group,
		Astral,
	}

	public enum ResistanceFlags
	{
		Disease,
		Poison,
		Fire,
		Charm,
		Bash,
		Pierce,
		Cold,
		Light,
		Lightning,
		Drowning,
		Summon,
		Magic,
		Weapon,
		Mental,
		Negative,
		Holy,
		Slash,
		Acid,
		Energy,
		Iron,
		Silver,
		Sound,
		Wood,
	}

	[Flags]
	public enum FormFlags
	{
		None = 0,
		// Body Forms
		Edible = 1 << 0,
		Poison = 1 << 1,
		Magical = 1 << 2,
		InstantDecay = 1 << 3,
		Other = 1 << 4,

		// Actual Forms
		Animal = 1 << 6,
		Sentinent = 1 << 7,
		Undead = 1 << 8,
		Construct = 1 << 9,
		Mist = 1 << 10,
		Intangible = 1 << 11,
		Biped = 1 << 12,
		Centaur = 1 << 13,
		Insect = 1 << 14,
		Spider = 1 << 15,
		Crustacean = 1 << 16,
		Worm = 1 << 17,
		Blob = 1 << 18,
		Mammal = 1 << 21,
		Bird = 1 << 22,
		Reptile = 1 << 23,
		Snake = 1 << 24,
		Dragon = 1 << 25,
		Amphibian = 1 << 26,
		Fish = 1 << 27,
		ColdBlood = 1 << 28,

		FormsHumanoid = Edible | Sentinent | Biped | Mammal,
		FormsMammal = Edible | Animal | Mammal,
		FormsBird = Edible | Animal | Bird,
		FormsBug = Edible | Animal | Insect
	}

	[Flags]
	public enum PartFlags
	{
		None = 0,
		Head = 1 << 0,
		Arms = 1 << 1,
		Legs = 1 << 2,
		Heart = 1 << 3,
		Brains = 1 << 4,
		Guts = 1 << 5,
		Hands = 1 << 6,
		Feet = 1 << 7,
		Fingers = 1 << 8,
		Ear = 1 << 9,
		Eye = 1 << 10,
		LongTongue = 1 << 11,
		EyeStalks = 1 << 12,
		Tentacles = 1 << 13,
		Fins = 1 << 14,
		Wings = 1 << 15,
		Tail = 1 << 16,
		Claws = 1 << 20,
		Fangs = 1 << 21,
		Horns = 1 << 22,
		Scales = 1 << 23,
		Tusks = 1 << 24,

		PartsAlive = Heart | Brains | Guts,
		PartsQuadRuped = Head | Legs | PartsAlive | Feet | Ear | Eye,
		PartsBiped = Head | Arms | Legs | PartsAlive | Feet | Ear | Eye,
		PartsHumanoid = PartsBiped | Hands | Fingers,
		PartsFeline = PartsQuadRuped | Fangs | Tail | Claws,
		PartsCanine = PartsQuadRuped | Fangs,
		PartsReptile = PartsAlive | Head | Eye | LongTongue | Tail | Scales,
		PartsLizard = PartsQuadRuped | PartsReptile,
		PartsBird = PartsAlive | Head | Legs | Feet | Eye | Wings
	}

	public enum Alignment
	{
		Good,
		Neutral,
		Evil
	}

	public class Attack
	{
		public string AttackType { get; set; }
		public int Hit { get; set; }
		public Dice DamageDice;

		public Attack(string attackType, int hit, Dice damageDice)
		{
			AttackType = attackType;
			Hit = hit;
			DamageDice = damageDice;
		}
	}

	public class Mobile : AreaEntity
	{
		public string Name { get; set; }
		public string ShortDescription { get; set; }
		public string LongDescription { get; set; }
		public string Description { get; set; }
		public string Race { get; set; }
		public HashSet<MobileFlags> Flags { get; set; }
		public HashSet<AffectedByFlags> AffectedByFlags { get; set; }
		public Alignment Alignment { get; set; }
		public int Group { get; set; }
		public int Level { get; set; }
		public int ArmorClass { get; set; }
		public Dice HitDice { get; set; }
		public Dice ManaDice { get; set; }
		public string Guild { get; set; }
		public int ArmorClassPierce { get; set; }
		public int ArmorClassBash { get; set; }
		public int ArmorClassSlash { get; set; }
		public int ArmorClassExotic { get; set; }
		public HashSet<ResistanceFlags> ImmuneFlags { get; set; }
		public HashSet<ResistanceFlags> ResistanceFlags { get; set; }
		public HashSet<ResistanceFlags> VulnerableFlags { get; set; }
		public string StartPosition { get; set; }
		public string Position { get; set; }
		public string Sex { get; set; }
		public int Wealth { get; set; }
		public int Xp { get; set; }
		public FormFlags FormFlags { get; set; }
		public PartFlags PartFlags { get; set; }
		public string Size { get; set; }
		public string Material { get; set; }

		public Shop Shop { get; set; }
		public List<Attack> Attacks { get; set; } = new List<Attack>();
		public List<MobileSpecialAttack> SpecialAttacks { get; set; }

		public Mobile()
		{
			Flags = new HashSet<MobileFlags>();
			AffectedByFlags = new HashSet<AffectedByFlags>();
			ImmuneFlags = new HashSet<ResistanceFlags>();
			ResistanceFlags = new HashSet<ResistanceFlags>();
			VulnerableFlags = new HashSet<ResistanceFlags>();
			SpecialAttacks = new List<MobileSpecialAttack>();
		}

		public void SetAttack(string attackType, int hit, Dice damageDice)
		{
			Attacks.Clear();

			var attack = new Attack(attackType, hit, damageDice);
			Attacks.Add(attack);
		}

		public override string ToString() => $"{ShortDescription} (#{VNum})";
	}
}