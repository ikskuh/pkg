using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pkg
{
	abstract class Command
	{
		private readonly string name;

		private readonly string helpText;

		public Command(string name, string helpText)
		{
			this.name = name;
			this.helpText = helpText;
			this.LongHelpText = helpText;
		}

		public abstract void Invoke(params string[] args);

		public string Name
		{
			get { return name; }
		}

		public string HelpText
		{
			get { return helpText; }
		}

		public string LongHelpText
		{
			get;
			protected set;
		}

		public static Command[] GetCommands()
		{
			List<Command> commands = new List<Command>();
			foreach(var type in typeof(Command).Assembly.GetTypes())
			{
				if(type.IsSubclassOf(typeof(Command)))
				{
					var cmd = Activator.CreateInstance(type) as Command;
					if (cmd != null)
						commands.Add(cmd);
				}
			}
			return commands.ToArray();
		}
	}
}
