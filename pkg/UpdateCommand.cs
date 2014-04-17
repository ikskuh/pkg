using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pkg
{
	class UpdateCommand : Command
	{
		public UpdateCommand()
			: base("update", "Updates the list of packages, libraries and templates.")
		{

		}

		public override void Invoke(params string[] args)
		{
			throw new NotImplementedException();
		}
	}
}
