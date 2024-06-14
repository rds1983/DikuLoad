namespace DikuLoad.Import.Ascii
{
	public enum SourceType
	{
		ROM,
		Envy,
		Circle
	}

	public class ImporterSettings
	{
		public string InputFolder { get; private set; }
		public SourceType SourceType { get; private set; }
		public string AreaNameFilter { get; set; }

		public ImporterSettings(string inputFolder, SourceType sourceType)
		{
			InputFolder = inputFolder;
			SourceType = sourceType;
		}
	}
}
