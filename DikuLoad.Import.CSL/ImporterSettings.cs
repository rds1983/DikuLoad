namespace DikuLoad.Import.CSL
{
	public class ImporterSettings
	{
		public string InputFolder { get; private set; }

		public string[] AreasNames { get; set; }

		public ImporterSettings(string inputFolder)
		{
			InputFolder = inputFolder;
		}
	}
}
