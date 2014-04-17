using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace pkg
{
	public sealed class Repository
	{
		private XmlSerializer packageSerializer;
		private Dictionary<string, Package> packages = new Dictionary<string, Package>();
		private Dictionary<string, Package> templates = new Dictionary<string, Package>();

		public Repository()
		{
			this.packageSerializer = new XmlSerializer(typeof(PackageRepo));
		}

		public void Load(string source)
		{
			if (File.Exists(source))
			{
				using (var fs = File.Open(source, FileMode.Open))
				{
					this.Load(source);
				}
			}
			else
			{
				Uri uri = new Uri(source);
				if (uri.Scheme == Uri.UriSchemeFile)
				{
					using (var fs = File.Open(uri.AbsolutePath, FileMode.Open))
					{
						this.Load(fs);
					}
				}
				else
				{
					using (var client = new WebClient())
					{
						using (var ws = client.OpenRead(uri))
						{
							this.Load(ws);
						}
					}
				}
			}
		}

		public void Load(Stream source)
		{
			var repo = this.packageSerializer.Deserialize(source) as PackageRepo;
			if (repo == null)
				throw new InvalidDataException();

			var backup = this.packages;
			try
			{
				this.packages = new Dictionary<string, Package>();
				foreach (var package in repo.Packages)
				{
					if (this.packages.ContainsKey(package.Name))
					{
						Console.WriteLine("Name conflict: {0} exists in two repository sources.", package.Name);
					}
					else
					{
						this.packages.Add(package.Name, package);
					}
				}
				foreach (var package in repo.Templates)
				{
					if (this.templates.ContainsKey(package.Name))
					{
						Console.WriteLine("Name conflict: {0} exists in two repository sources.", package.Name);
					}
					else
					{
						this.templates.Add(package.Name, package);
					}
				}
			}
			catch (Exception)
			{
				this.packages = backup;
				throw;
			}
		}

		public IEnumerable<Package> Packages
		{
			get { return this.packages.Values; }
		}

		public IEnumerable<Package> Templates
		{
			get { return this.templates.Values; }
		}

		public Package GetPackage(string name)
		{
			if (this.packages.ContainsKey(name))
				return this.packages[name];
			else
				return null;
		}

		public Package GetTemplate(string name)
		{
			if (this.templates.ContainsKey(name))
				return this.templates[name];
			else
				return null;
		}
	}
}
