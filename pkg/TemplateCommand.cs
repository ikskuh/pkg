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
	class TemplateCommand : Command
	{
		public TemplateCommand()
			: base("template", "Creates a template in the current folder.")
		{

		}

		public override void Invoke(params string[] args)
		{
			foreach (var pkg in args)
			{
				var package = Program.Repository.GetTemplate(pkg);
				if (package == null)
				{
					Console.WriteLine("Package {0} not found.", pkg);
				}
				else
				{
					string targetPath = "./";
					package.Install(targetPath);
				}
			}
		}
	}
}
