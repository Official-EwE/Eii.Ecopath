using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using EwEMSPLink;
using MEL.Converters;
using MSWSupport;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace MEL;

public class MEL
{
    public const int TICK_DELAY_MS = 100;   //in ms

    private Config? config;
    private string? configstring;
    private cEwEMSPLink shell;
    private List<cPressure> pressures = new();
    private List<cGrid> outputs = new();

    // Given folder Input\ recursively search for all files with the extension .json and create a hashmap with the json filename as key and the full path as value
    private static Dictionary<string, string> GetConfigFiles()
    {
        Dictionary<string, string> jsonFiles = new();
        foreach (string file in Directory.EnumerateFiles("Input", "*.json", SearchOption.AllDirectories))
        {
            string filename = Path.GetFileName(file);
            jsonFiles.Add(filename, file);
        }
        return jsonFiles;
    }

    public MEL(string[] args)
    {
        /* Create a ConsoleTraceListener and add it to the trace listeners. */
        var myWriter = new ConsoleTraceListener();
        Trace.Listeners.Add(myWriter); // this will output all writes to "Debug."

        shell = new cEwEMSPLink();

        Dictionary<string, string> configFiles = GetConfigFiles();
        if (args.Length == 0)
        {
            string randomAvailableConfigFile = configFiles.Keys.ElementAt(new Random().Next(configFiles.Count));
            args = new[] {randomAvailableConfigFile};
            ConsoleLogger.Info("Available config files: " + string.Join(", ", configFiles.Keys));
            ConsoleLogger.Info("No config file provided, using random config file: " + args[0]);
        }
        if (!configFiles.ContainsKey(args[0]))
        {
            throw new Exception("Config file not found: " + args[0]);
        }

        string configFilepath = configFiles[args[0]];
        LoadConfig(configFilepath);

		//Start values for fishing intensity as returned by EwEShell.
		List<cScalar> initialFishingValues = new();
        if (!shell.Configuration(configstring, initialFishingValues))
        {
            //something went wrong here
            throw new Exception("EwE Startup failed");
        }

        // get the directory of the config file path
        string directory = Path.GetDirectoryName(configFilepath) ?? throw new InvalidOperationException();
        LoadPressures(Path.Combine(directory,"pressures1.log"));

		// Dump game version for testing purposes
		ConsoleLogger.Info($"Loaded EwE model '{shell.CurrentGame.Version}', {shell.CurrentGame.Author}, {shell.CurrentGame.Contact}");

		//eweshell initialised fine
		shell.Startup();

        ConsoleLogger.Info("Startup done");
    }

    public void Tick()
    {
        // MEL normally checks here, if simulation is needed, if the game entered a new month

        Stopwatch watch = Stopwatch.StartNew();
        shell.Tick(pressures, outputs);
        StoreTick();
        ConsoleLogger.Info($"Tick done, executed in: {watch.ElapsedMilliseconds} ms");
    }

    private void StoreTick()
    {
        try
        {
            Directory.Delete("output", true);
        }
        catch (DirectoryNotFoundException)
        {
            // ignore
        }
        foreach (cGrid grid in outputs)
        {
            SubmitBitmapForStorage(grid);
        }
    }

    private static void SubmitBitmapForStorage(cGrid grid)
    {
        Directory.CreateDirectory("output");
        using Bitmap bitmap = Rasterizer.ToBitmapSlow(grid.Cell);
        string targetFile = Path.Combine("output", $"{grid.Name}.png");
        if (File.Exists(targetFile))
        {
	        File.Delete(targetFile);
        }
        bitmap.Save(targetFile, ImageFormat.Png);
        ConsoleLogger.Info($"Saved grid to {targetFile}");
    }

    private void LoadPressures(string pressuresFilepath)
    {
        if (!File.Exists(pressuresFilepath))
        {
            throw new Exception("Pressure file not found: " + pressuresFilepath);
        }
        JArray jsonArr = JArray.Parse(File.ReadAllText(pressuresFilepath));
        JsonSerializer serializer = new();
        serializer.Converters.Add(new cPressureConverter());
        List<cPressure>? conv = jsonArr.ToObject<List<cPressure>>(serializer);
        pressures = conv ?? throw new Exception("Failed to convert pressure data");
    }

    private void LoadConfig(string configFilepath)
    {
        if (!File.Exists(configFilepath))
        {
            throw new Exception("Config file not found: " + configFilepath);
        }
        JObject jsonObj = JObject.Parse(File.ReadAllText(configFilepath));
        JObject? melObj = (JObject)jsonObj["datamodel"]["MEL"];
        if (melObj == null)
        {
            throw new Exception("No MEL object found in config file");
        }
        configstring = melObj.ToString();
        Config? conv = melObj.ToObject<Config>();
        config = conv ?? throw new Exception("Failed to convert config data");
        foreach (Outcome o in config.outcomes)
        {
            ConsoleLogger.Info($"Outcome config: {o.name}");
            outputs.Add(new cGrid(o.name, config.columns, config.rows));
        }
    }
}
