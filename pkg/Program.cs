using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace pkg
{
	class Program
	{
		public static readonly string pkgroot;
		public static readonly string pkgname;

		public static readonly Command[] commands;

		public static Repository Repository { get; private set; }

		static Program()
		{
			pkgroot = Config.Default.InstallRoot + "/";
			pkgname = Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]);

			commands = Command.GetCommands();

			string repoFile = pkgroot + "repositories.txt";
			if (!File.Exists(repoFile))
			{
				File.WriteAllLines(repoFile, new[]
				{
					"############################################################################",
					"#Demo Repository List",
					"############################################################################",
					"",
					"#Default Repo",
					@"file://D:\temp\pkg\pkg\bin\Debug\demo.xml",
				});
			}
			Program.Repository = new Repository();
			foreach(var line in File.ReadAllLines(repoFile))
			{
				string l = line.Trim();
				if (string.IsNullOrWhiteSpace(l))
					continue;
				if (l.StartsWith("#"))
					continue;
				try
				{
					Program.Repository.Load(l);
				}
				catch(Exception ex)
				{
					Console.WriteLine("Failed to load repo at {0}:", l);
					Console.WriteLine(ex.Message);
				}
			}
		}

		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				new HelpCommand().Invoke(new string[0]);
				return;
			}
			var commandName = args[0];

			Command cmd = null;
			foreach (var command in commands)
			{
				if (command.Name != commandName)
					continue;
				cmd = command;
				break;
			}
			if (cmd == null)
			{
				Console.WriteLine("Command {0} not found. Type {1} help to get a list of all commands.", commandName, pkgname);
				return;
			}

			var @params = new string[args.Length - 1];
			for (int i = 1; i < args.Length; i++)
			{
				@params[i - 1] = args[i];
			}

#if !DEBUG
			try
			{
#endif
			cmd.Invoke(@params);
#if !DEBUG
			}
			catch(Exception ex)
			{
				Console.WriteLine("{0} crashed:");
				Console.WriteLine(ex.ToString());
			}
#endif
		}
	}
}
