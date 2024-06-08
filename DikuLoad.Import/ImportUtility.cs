using System.Reflection;
using System.Text.RegularExpressions;
using System;
using System.IO;
using DikuLoad.Data;

namespace DikuLoad.Import
{
	public static class ImportUtility
	{
		private static readonly Regex CreditsRegEx1 = new Regex(@"^[\{\[]?\s*(\w+)\s*[\}\]]\s*(\w+)");
		private static readonly Regex CreditsRegEx2 = new Regex(@"^[\{\[]\s*(\w+)\s*-?\s*(\w+)\s*[\}\]]\s*(\w+)");

		public static string ExecutingAssemblyDirectory
		{
			get
			{
				string codeBase = Assembly.GetExecutingAssembly().Location;
				UriBuilder uri = new UriBuilder(codeBase);
				string path = Uri.UnescapeDataString(uri.Path);
				return Path.GetDirectoryName(path);
			}
		}

		public static string CasedName(this string name)
		{
			if (name == null)
			{
				return null;
			}

			name = name.Trim();
			if (string.IsNullOrEmpty(name))
			{
				return string.Empty;
			}

			if (name.Length == 1)
			{
				return name.ToUpper();
			}

			return char.ToUpper(name[0]) + name.Substring(1, name.Length - 1).ToLower();
		}

		public static void Log(string message) => Console.WriteLine(message);

		public static bool ParseLevelsBuilds(this Area area)
		{
			// Try to get levels range from the credits
			var c = area.Credits.Trim();
			var match = CreditsRegEx1.Match(c);
			if (match.Success)
			{
				area.MinimumLevel = area.MaximumLevel = match.Groups[1].Value;
				area.Builders = match.Groups[2].Value;

				Log($"Regex1 worked: parsed {area.MinimumLevel}/{area.Builders} from {c}");
			}
			else
			{

				match = CreditsRegEx2.Match(c);
				if (match.Success)
				{
					area.MinimumLevel = match.Groups[1].Value;
					area.MaximumLevel = match.Groups[2].Value;

					area.Builders = match.Groups[3].Value;

					Log($"Regex2 worked: parsed [{area.MinimumLevel} {area.MaximumLevel}]/{area.Builders} from {c}");
				}
			}

			if (!match.Success)
			{
				Log($"Couldn't parse levels/builders info from {c}");
			}

			return match.Success;
		}
	}
}