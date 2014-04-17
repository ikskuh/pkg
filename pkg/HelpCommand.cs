using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pkg
{
	class HelpCommand : Command
	{
		public HelpCommand()
			: base("help", "Shows this help text or gives help to a specific command.")
		{

		}

		public override void Invoke(params string[] args)
		{
			int maxCommandLength = 0;
			var commands = new Dictionary<string, Command>();
			foreach (var cmd in Program.commands)
			{
				commands.Add(cmd.Name, cmd);
				maxCommandLength = Math.Max(cmd.Name.Length, maxCommandLength);
			}

			if (args.Length > 0)
			{
				foreach(var arg in args)
				{
					if(commands.ContainsKey(arg))
					{
						Console.WriteLine("Help for {0}:", arg);
						Console.WriteLine(commands[arg].LongHelpText);
					}
					else
					{
						Console.WriteLine("Command {0} not found.", arg);
					}
				}
			}
			else
			{
				Console.WriteLine("{0} package manager.", Program.pkgname);
				Console.WriteLine("Available commands:");
				foreach (var cmd in commands)
				{
					Console.WriteLine(" {0} - {1}", cmd.Key.PadRight(maxCommandLength, ' '), cmd.Value.HelpText);
				}
			}
		}
	}
}
