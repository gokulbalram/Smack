using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

public class FileSender
{
    private static bool verbose = false; // Control verbose mode

    public static async Task SendFileAsync(string filePath, string relativePath, string ipAddress, int port = 8080)
    {
        try
        {
            using TcpClient client = new TcpClient(ipAddress, port);
            using NetworkStream networkStream = client.GetStream();
            using FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            byte[] pathBytes = System.Text.Encoding.UTF8.GetBytes(relativePath.Replace(" ", "%20") + "\n");
            await networkStream.WriteAsync(pathBytes, 0, pathBytes.Length);

            if (verbose)
            {
                Console.WriteLine($"Sending {Path.GetFileName(filePath)} on thread ID: {Thread.CurrentThread.ManagedThreadId}");
            }

            byte[] buffer = new byte[1024];
            int bytesRead;
            while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await networkStream.WriteAsync(buffer, 0, bytesRead);
            }

            Console.WriteLine($"File {Path.GetFileName(filePath)} sent successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending file {filePath}: {ex.Message}");
        }
    }

    public static async Task SendFolderAsync(string folderPath, string ipAddress, int port = 8080)
    {
        var allFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);
        List<Task> fileSendTasks = new List<Task>();

        foreach (var file in allFiles)
        {
            string relativePath = Path.GetRelativePath(Directory.GetParent(folderPath).FullName, file);
            fileSendTasks.Add(SendFileAsync(file, relativePath, ipAddress, port));
        }

        await Task.WhenAll(fileSendTasks);
        Console.WriteLine($"All files in folder '{folderPath}' sent successfully.");
    }

    public static void SetVerbose(bool isVerbose)
    {
        verbose = isVerbose;
    }
}
