using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace NotesieveUpdater
{
    class Program
    {
        static void Main(string[] args)
        {

			Process[] procs = Process.GetProcessesByName("Notesieve");
			foreach (Process p in procs)
			{
				p.Kill();
				Thread.Sleep(1000);
			}

			string targetDirectory = Environment.CurrentDirectory + @"\" + "Updates";
			if (!Directory.Exists(targetDirectory)) return;

			string[] fileEntries = Directory.GetFiles(targetDirectory);
			foreach (string oldFile in fileEntries)
			{
				Console.WriteLine(oldFile);
				string fileName = Path.GetFileName(oldFile);
				string newFile = Environment.CurrentDirectory + @"\" + fileName;
				if (File.Exists(newFile))
				{
					File.Delete(newFile);
				}
				File.Move(oldFile, newFile);
			}

			Directory.Delete(targetDirectory);

			Process.Start(Environment.CurrentDirectory + @"/" + "Notesieve.exe");			
		}
    }
}
