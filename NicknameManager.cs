using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

public class NicknameData
{
    public string HostNickname { get; set; }
}

public class NicknameManager
{
    private const string NicknameFilePath = "hostname.json";
    private bool verbose;
    public string HostNickname { get; private set; }

    public NicknameManager(bool verboseMode)
    {
        verbose = verboseMode;
        LoadNickname();
    }

    public void SetNickname(string nickname)
    {
        HostNickname = nickname;
        SaveNickname();
        if (verbose)
        {
            Console.WriteLine($"[DEBUG] Nickname set to: {HostNickname}");
        }
    }

    private void LoadNickname()
    {
        if (File.Exists(NicknameFilePath))
        {
            var json = File.ReadAllText(NicknameFilePath);
            var data = JsonSerializer.Deserialize<NicknameData>(json, NicknameManagerContext.Default.NicknameData);
            HostNickname = data?.HostNickname;

            if (verbose)
            {
                Console.WriteLine($"[DEBUG] Loaded nickname from file: {HostNickname}");
            }
        }
        else
        {
            if (verbose)
            {
                Console.WriteLine("[DEBUG] No nickname file found. Starting fresh.");
            }
        }
    }

    private void SaveNickname()
    {
        var data = new NicknameData { HostNickname = HostNickname };
        var json = JsonSerializer.Serialize(data, NicknameManagerContext.Default.NicknameData);
        File.WriteAllText(NicknameFilePath, json);

        if (verbose)
        {
            Console.WriteLine($"[DEBUG] Nickname saved to file: {NicknameFilePath}");
        }
    }
}

[JsonSerializable(typeof(NicknameData))]
public partial class NicknameManagerContext : JsonSerializerContext
{
}
