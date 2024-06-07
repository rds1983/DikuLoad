namespace DikuLoad.Data
{
	public enum EffectBitType
	{
		None,
		Object,
		Immunity,
		Resistance,
		Vulnerability,
		Weapon
	}

	public enum EffectType
	{
		None = 0,
		Strength,
		Dexterity,
		Intelligence,
		Wisdom,
		Constitution,
		Sex,
		Class,
		Level,
		Age,
		Height,
		Weight,
		Mana,
		Hit,
		Move,
		Gold,
		Exp,
		Ac,
		HitRoll,
		DamRoll,
		Saves,
		SavingPara = Saves,
		SavingRod,
		SavingPetri,
		SavingBreath,
		SavingSpell,
		SpellAffect
	}

	public class GameObjectEffect
	{
		public GameObject GameObject { get; set; }

		public EffectBitType EffectBitType { get; set; }
		public EffectType EffectType { get; set; }
		public int Modifier { get; set; }
		public AffectedByFlags Bits { get; set; }
	}
}
