using System;

namespace AbarimMUD.Import.Envy
{
	[Flags]
	public enum OldRoomFlags
	{
		None = 0,
		Dark = 1 << 0,
		NoMob = 1 << 2,
		InDoors = 1 << 3,
		Private = 1 << 9,
		Safe = 1 << 10,
		Solitary = 1 << 11,
		PetShop = 1 << 12,
		NoRecall = 1 << 13,
		ImpOnly = 1 << 14,
		GodsOnly = 1 << 15,
		HeroesOnly = 1 << 16,
		NewbiesOnly = 1 << 17,
		Law = 1 << 18,
		Nowhere = 1 << 19
	}

	[Flags]
	public enum OldRoomExitFlags
	{
		None = 0,
		Door = 1 << 0,
		Closed = 1 << 1,
		Locked = 1 << 2,
		PickProof = 1 << 5,
		NoPass = 1 << 6,
		Easy = 1 << 7,
		Hard = 1 << 8,
		Infuriating = 1 << 9,
		NoClose = 1 << 10,
		NoLock = 1 << 11,
	}

	[Flags]
	public enum OldMobileFlags
	{
		None = 0,
		Npc = 1 << 0,
		Sentinel = 1 << 1,
		Scavenger = 1 << 2,
		Aggressive = 1 << 5,
		StayInArea = 1 << 6,
		Wimpy = 1 << 7,
		Pet = 1 << 8,
		Train = 1 << 9,
		Practice = 1 << 10,
		Undead = 1 << 14,
		Cleric = 1 << 16,
		Mage = 1 << 17,
		Thief = 1 << 18,
		Warrior = 1 << 19,
		NoAlign = 1 << 20,
		NoPurge = 1 << 21,
		OutDoors = 1 << 22,
		InDoors = 1 << 24,
		IsHealer = 1 << 26,
		Gain = 1 << 27,
		UpdateAlways = 1 << 28,
		IsChanger = 1 << 29,
		FriendlyBits = Train | Practice | IsHealer | IsChanger
	}

	[Flags]
	public enum OldMobileOffensiveFlags
	{
		None = 0,
		AreaAttack = 1 << 0,
		Backstab = 1 << 1,
		Bash = 1 << 2,
		Berserk = 1 << 3,
		Disarm = 1 << 4,
		Dodge = 1 << 5,
		Fade = 1 << 6,
		Fast = 1 << 7,
		Kick = 1 << 8,
		KickDirt = 1 << 9,
		Parry = 1 << 10,
		Rescue = 1 << 11,
		Tail = 1 << 12,
		Trip = 1 << 13,
		Crush = 1 << 14,
		AssistAll = 1 << 15,
		AssistAlign = 1 << 16,
		AssistRace = 1 << 17,
		AssistPlayer = 1 << 18,
		AssistGuard = 1 << 19,
		AssistId = 1 << 20,
	}

	[Flags]
	public enum OldResistanceFlags
	{
		None = 0,
		Summon = 1 << 0,
		Charm = 1 << 1,
		Magic = 1 << 2,
		Weapon = 1 << 3,
		Bash = 1 << 4,
		Pierce = 1 << 5,
		Slash = 1 << 6,
		Fire = 1 << 7,
		Cold = 1 << 8,
		Lightning = 1 << 9,
		Acid = 1 << 10,
		Poison = 1 << 11,
		Negative = 1 << 12,
		Holy = 1 << 13,
		Energy = 1 << 14,
		Mental = 1 << 15,
		Disease = 1 << 16,
		Drowning = 1 << 17,
		Light = 1 << 18,
		Sound = 1 << 19,
		Wood = 1 << 23,
		Silver = 1 << 24,
		Iron = 1 << 25,
	}

	[Flags]
	public enum OldAffectedByFlags
	{
		None = 0,
		Blindness = 1 << 0,
		Invisible = 1 << 1,
		DetectEvil = 1 << 2,
		DetectInvis = 1 << 3,
		DetectMagic = 1 << 4,
		DetectHidden = 1 << 5,
		DetectGood = 1 << 6,
		Sanctuary = 1 << 7,
		FaerieFire = 1 << 8,
		Infrared = 1 << 9,
		Curse = 1 << 10,
		Poison = 1 << 12,
		ProtectEvil = 1 << 13,
		ProtectGood = 1 << 14,
		Sneak = 1 << 15,
		Hide = 1 << 16,
		Sleep = 1 << 17,
		Charm = 1 << 18,
		Flying = 1 << 19,
		PassDoor = 1 << 20,
		Haste = 1 << 21,
		Calm = 1 << 22,
		Plague = 1 << 23,
		Weaken = 1 << 24,
		DarkVision = 1 << 25,
		Berserk = 1 << 26,
		Swim = 1 << 27,
		Regeneration = 1 << 28,
		Slow = 1 << 29,
	}
}
