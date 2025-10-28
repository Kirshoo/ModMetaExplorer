using System;
using System.Text;
using Newtonsoft.Json;

namespace ModMetaExplorer;

public class ProgramParameters
{
    public string Path { get; set; } = string.Empty;
    public string OutputFile { get; set; } = string.Empty;
    public bool ToJson { get; set; } = false;
}

class Program
{
    private static void PrintInfo(BepInPluginInfo info)
    {
        Console.WriteLine(
            $"""
            GUID: {info.GUID}
            Name: {info.Name}
            Version: {info.Version}
            SHA256: {info.Sha256}
            """
        );
    }

    private static bool TryGetInfo(string filepath, out BepInPluginInfo info)
    {
        info = new();

        try
        {
            info = BepInPluginReader.ReadPluginInfo(filepath);
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine("Exception: " + ex.Message);
            Console.WriteLine($"File at {filepath} doesn't exist. Make sure you are selecting correct file.");
            return false;
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine("Exception: " + ex.Message);
            Console.WriteLine($"File at {filepath} is not a BepInEx plugin.");
            return false;
        }

        return true;
    }

    private static void HandleSinglePlugin(ProgramParameters flags)
    {
        if (!TryGetInfo(flags.Path, out BepInPluginInfo info)) { return; }

        string serialized;
        if (flags.ToJson)
        {
            serialized = JsonConvert.SerializeObject(info);
        }
        else
        {
            serialized = info.ToString();
        }

        if (string.IsNullOrEmpty(flags.OutputFile))
        {
            Console.WriteLine(serialized);
            return;
        }

        string dir = Path.GetDirectoryName(flags.OutputFile)!;
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        File.WriteAllText(flags.OutputFile, serialized);
    }

    private static void HandlePluginDirectory(ProgramParameters flags)
    {
        List<BepInPluginInfo> plugins = new List<BepInPluginInfo>();

        string[] files = Directory.GetFiles(flags.Path, "*.dll", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            if (!TryGetInfo(file, out BepInPluginInfo info)) { continue; }

            plugins.Add(info);
        }

        StringBuilder serialized = new();
        if (flags.ToJson)
        {
            serialized.Append(JsonConvert.SerializeObject(plugins));
        }
        else
        {
            foreach (BepInPluginInfo info in plugins)
            {
                serialized.AppendLine(info.ToString());
            }
        }

        if (string.IsNullOrEmpty(flags.OutputFile))
        {
            Console.WriteLine(serialized.ToString());
            return;
        }

        string dir = Path.GetDirectoryName(flags.OutputFile)!;
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        File.WriteAllText(flags.OutputFile, serialized.ToString());
    }

    private static void Main(string[] args)
    {
        ProgramParameters flags = ParseInput(args);

        if (string.IsNullOrEmpty(flags.Path))
        {
            return;
        }

        if (!flags.Path.EndsWith(".dll"))
        {
            HandlePluginDirectory(flags);
            return;
        }

        HandleSinglePlugin(flags);
    }

    private static ProgramParameters ParseInput(string[] args)
    {
        ProgramParameters parameters = new();

        if (args.Length <= 0)
        {
            Console.WriteLine(
                """
                Please provide the path to .dll file to get plugin information.
                If filepath contains spaces, please make sure to use quotation marks.
                (ex. "C:\My Mod Profile\ThisMod.dll" or "C:\MyUser\That Mod.dll")
                """
             );
        }

        for (int i = 0; i < args.Length; i++)
        {
            if (!args[i].StartsWith('-'))
            {
                parameters.Path = args[i];
                continue;
            }

            if (args[i].Equals("-j"))
            {
                parameters.ToJson = true;
                continue;
            }

            if (args[i].Equals("--output") && i + 1 < args.Length && !args[i + 1].StartsWith('-'))
            {
                parameters.OutputFile = args[i + 1];
                i += 1;
                continue;
            }
        }

        return parameters;
    }
}