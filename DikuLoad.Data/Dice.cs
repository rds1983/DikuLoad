﻿using System.Text.RegularExpressions;

namespace DikuLoad.Data
{
	public struct Dice
	{
		private static readonly Regex Expr1 = new Regex(@"^\s*(\d+)d(\d+)\s*$");
		private static readonly Regex Expr2 = new Regex(@"^\s*(\d+)d(\d+)\s*\+\s*(\d+)\s*$");

		public int Sides { get; set; }
		public int Count { get; set; }
		public int Bonus { get; set; }

		public Dice(int sides, int count, int bonus)
		{
			Sides = sides;
			Count = count;
			Bonus = bonus;
		}

		public override string ToString() => $"{Count}d{Sides}+{Bonus}";

		public static Dice? Parse(string expr)
		{
			expr = expr.ToLower().Trim();
			var match = Expr1.Match(expr);
			if (match.Success)
			{
				return new Dice(int.Parse(match.Groups[2].Value), int.Parse(match.Groups[1].Value), 0);
			}

			match = Expr2.Match(expr);
			if (match.Success)
			{
				return new Dice(int.Parse(match.Groups[2].Value), int.Parse(match.Groups[1].Value), int.Parse(match.Groups[3].Value));
			}

			return null;
		}
	}
}
