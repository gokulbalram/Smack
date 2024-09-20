using System;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        NicknameManager nicknameManager = new NicknameManager();
        nicknameManager.InformIfNoNickname();

        bool verbose = args.Length > 0 && args[0] == "-v";
        FileSender.SetVerbose(verbose);

        if (args.Length == 0)
        {
            Console.WriteLine("Ready to go");
            return;
        }

        if (args[0] == "-n" && args.Length == 2)
        {
            nicknameManager.SetNickname(args[1]);
            Console.WriteLine($"Nickname set to: {args[1]}");
            return;
        }

        string targetIp = nicknameManager.GetReceiverIPAddress();

        if (args[0] == "-s" && args.Length >= 2)
        {
            string filePathOrFolder = args[1];

            if (Directory.Exists(filePathOrFolder))
            {
                await FileSender.SendFolderAsync(filePathOrFolder, targetIp);
            }
            else if (File.Exists(filePathOrFolder))
            {
                await FileSender.SendFileAsync(filePathOrFolder, Path.GetFileName(filePathOrFolder), targetIp);
            }
            else
            {
                Console.WriteLine("The specified path does not exist.");
            }
        }
        else if (args[0] == "-r" && args.Length >= 2)
        {
            string saveDirectory = args[1];
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }
            await FileReceiver.StartListening(saveDirectory);
        }
        else
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  - To set nickname: FileTransferApp.exe -n <nickname>");
            Console.WriteLine("  - To send: FileTransferApp.exe -s <file-path-or-folder>");
            Console.WriteLine("  - To receive: FileTransferApp.exe -r <save-directory>");
        }
    }
}
