namespace DikuLoad.Import.Ascii
{
	public enum SourceType
	{
		ROM,
		Envy,
		Circle
	}

	public enum SubSourceType
	{
		Default,
		Circle31
	}

	public class ImporterSettings
	{
		public string InputFolder { get; private set; }
		public SourceType SourceType { get; private set; }
		public SubSourceType SubSourceType { get; private set; }

		public string[] AreasNames { get; set; }

		public ImporterSettings(string inputFolder, SourceType sourceType, SubSourceType subSourceType)
		{
			InputFolder = inputFolder;
			SourceType = sourceType;
			SubSourceType = subSourceType;
		}
	}
}
