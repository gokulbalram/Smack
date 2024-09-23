using System;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        bool verbose = args.Contains("-v");

        NicknameManager nicknameManager = new NicknameManager(verbose);

        if (args.Length == 0)
        {
            Console.WriteLine("Ready to go");
            return;
        }

        if (args[0] == "-n" && args.Length == 2) // Set host nickname
        {
            nicknameManager.SetNickname(args[1]);
            Console.WriteLine($"Host nickname set to: {args[1]}");
            return;
        }

        // Pass the verbose flag to FileSender and FileReceiver
        FileSender.SetVerbose(verbose);
        FileReceiver.SetVerbose(verbose);

        if (args[0] == "-s" && args.Length >= 3) // Send command
        {
            string filePathOrFolder = args[1];
            string targetIp = args[2];

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
        else if (args[0] == "-r" && args.Length >= 2) // Receive command
        {
            string saveDirectory = args[1];
            int port = 8080;  // Default port

            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }

            await FileReceiver.StartListening(saveDirectory, port);
        }
        else
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  - To set host nickname: FileTransferApp.exe -n <nickname>");
            Console.WriteLine("  - To send: FileTransferApp.exe -s <file-path-or-folder> <receiver-ip> [-v]");
            Console.WriteLine("  - To receive: FileTransferApp.exe -r <save-directory> [-v]");
        }
    }
}
