using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

public class NicknameManager
{
    private const string NicknameFilePath = "nickname.json";
    public string Nickname { get; private set; }
    private string receiverIPAddress;

    public NicknameManager()
    {
        LoadNickname();
    }

    public void SetNickname(string nickname)
    {
        Nickname = nickname;
        SaveNickname();
        receiverIPAddress = GetIPAddressByNickname(nickname);
    }

    public void LoadNickname()
    {
        if (File.Exists(NicknameFilePath))
        {
            string json = File.ReadAllText(NicknameFilePath);
            Nickname = json;
        }
        else
        {
            Nickname = null;
        }
    }

    private void SaveNickname()
    {
        File.WriteAllText(NicknameFilePath, Nickname);
    }

    public void InformIfNoNickname()
    {
        if (string.IsNullOrEmpty(Nickname))
        {
            Console.WriteLine("Nickname not set. Using local IP address instead.");
            receiverIPAddress = GetLocalIPAddress();
        }
    }

    public string GetReceiverIPAddress()
    {
        return receiverIPAddress ?? GetLocalIPAddress();
    }

    public string GetIPAddressByNickname(string nickname)
    {
        return GetLocalIPAddress();
    }

    private string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }
}
