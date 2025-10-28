using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Mono.Cecil;
using Newtonsoft.Json;

namespace ModMetaExplorer;

public class BepInPluginInfo
{
    [JsonProperty("guid")]
    public string GUID { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("version")]
    public string Version { get; set; } = string.Empty;

    [JsonProperty("sha256")]
    public string Sha256 { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"""
            GUID: {GUID}
            Name: {Name}
            Version: {Version}
            SHA256: {Sha256}
            """;
    }
}

internal class BepInPluginReader
{
    private const string BepInPluginAttribute = "BepInEx.BepInPlugin";

    public static BepInPluginInfo ReadPluginInfo(string dllPath)
    {
        if (!File.Exists(dllPath))
        {
            throw new FileNotFoundException(dllPath);
        }

        using var asm = AssemblyDefinition.ReadAssembly(dllPath);

        var pluginType = asm.MainModule.Types
            .FirstOrDefault(t => t.CustomAttributes
                .Any(a => a.AttributeType.FullName == BepInPluginAttribute));

        if (pluginType == null)
        {
            throw new InvalidOperationException("No [BepInPlugin] attribute found");
        }

        var attr = pluginType.CustomAttributes.First(a => a.AttributeType.FullName == BepInPluginAttribute);

        string guid = attr.ConstructorArguments[0].Value?.ToString() ?? string.Empty;
        string name = attr.ConstructorArguments[1].Value?.ToString() ?? string.Empty;
        string version = attr.ConstructorArguments[2].Value?.ToString() ?? string.Empty;

        string sha256 = string.Empty;
        using (var stream = File.OpenRead(dllPath))
        using (var sha = SHA256.Create())
        {
            byte[] hash = sha.ComputeHash(stream);
            sha256 = BitConverter.ToString(hash).Replace("-", "").ToUpperInvariant();
        }

        return new BepInPluginInfo
        {
            GUID = guid,
            Name = name,
            Version = version,
            Sha256 = sha256,
        };
    }
}
