using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace pkg
{
	class FindCommand : Command
	{
		public FindCommand()
			: base("find", "Finds a package, library or template.")
		{

		}

		public override void Invoke(params string[] args)
		{
			if (args.Length == 0)
			{
				Console.WriteLine("Listing all packages...");
				foreach (var package in Program.Repository.Packages)
				{
					Console.WriteLine(package.Name);
				}
			}
			foreach (var arg in args)
			{
				Console.WriteLine("Searching matches for {0}", arg);
				try
				{
					Regex regex = new Regex(arg);
					Console.WriteLine("Packages:");
					foreach (var package in Program.Repository.Packages)
					{
						if (regex.IsMatch(package.Name))
						{
							Console.WriteLine(package.Name);
						}
					}
					Console.WriteLine("Templates:");
					foreach (var package in Program.Repository.Templates)
					{
						if (regex.IsMatch(package.Name))
						{
							Console.WriteLine(package.Name);
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Failed to compile regex: {0}", ex.Message);
				}
			}
		}
	}
}
