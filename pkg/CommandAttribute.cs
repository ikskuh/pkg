using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pkg
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	sealed class CommandAttribute : Attribute
	{
		readonly string positionalString;

		public CommandAttribute(string helpText)
		{
			this.positionalString = helpText;
		}

		public string HelpText
		{
			get { return positionalString; }
		}
	}
}
