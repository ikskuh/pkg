using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace pkg
{
	class InstallCommand : Command
	{
		public InstallCommand()
			: base("install", "Installs a package or library.")
		{

		}

		public override void Invoke(params string[] args)
		{
			foreach (var pkg in args)
			{
				var package = Program.Repository.GetPackage(pkg);
				if (package == null)
				{
					Console.WriteLine("Package {0} not found.", pkg);
				}
				else
				{
					string targetPath = Config.Default.LocalStorage + "/" + package.Name + "/";

					if (Directory.Exists(targetPath))
					{
						Console.WriteLine("{0} is already installed.", this.Name);
						return;
					}

					package.Install(targetPath);
				}
			}
		}
	}
}
