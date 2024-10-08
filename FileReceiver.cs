﻿using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

public class FileReceiver
{
    private static bool verbose = false; // Control verbose mode

    public static async Task StartListening(string saveDirectory, int port = 8080)
    {
        TcpListener listener = new TcpListener(System.Net.IPAddress.Any, port);
        listener.Start();
        Console.WriteLine($"Listening for incoming files in: {saveDirectory}...");

        while (true)
        {
            using TcpClient client = await listener.AcceptTcpClientAsync();
            using NetworkStream networkStream = client.GetStream();

            using StreamReader reader = new StreamReader(networkStream);
            string relativePath = (await reader.ReadLineAsync())?.Replace("%20", " ");
            if (relativePath == null) continue;

            string fullPath = Path.Combine(saveDirectory, relativePath);
            string folder = Path.GetDirectoryName(fullPath);

            if (verbose)
            {
                Console.WriteLine($"Receiving {Path.GetFileName(fullPath)} on thread ID: {Thread.CurrentThread.ManagedThreadId}");
            }

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            using FileStream fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
            byte[] buffer = new byte[1024];
            int bytesRead;
            while ((bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await fileStream.WriteAsync(buffer, 0, bytesRead);
            }

            Console.WriteLine($"File received and saved to: {fullPath}");
        }
    }

    public static void SetVerbose(bool isVerbose)
    {
        verbose = isVerbose;
    }
}
