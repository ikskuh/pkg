using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace pkg
{

	[XmlRoot("repository")]
	public sealed class PackageRepo
	{
		[XmlElement("package")]
		public Package[] Packages { get; set; }

		[XmlElement("template")]
		public Package[] Templates { get; set; }
	}

	public sealed class Package
	{
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		[XmlAttribute("name")]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the download source.
		/// </summary>
		[XmlAttribute("source")]
		public string Source { get; set; }

		/// <summary>
		/// Gets or sets wheather the revision is considered obsolete or not.
		/// </summary>
		[XmlAttribute("obsolete")]
		public bool Obsolete { get; set; }

		internal void Install(string targetPath)
		{
			var uri = new Uri(this.Source);
			string fileName = Path.GetFileName(uri.AbsolutePath);
			var tempFile = Config.Default.TempPath + "/" + fileName;

			Directory.CreateDirectory(targetPath);
			try
			{
				using (WebClient client = new WebClient())
				{
					bool finished = false;
					int progressBarPosition = 0;
					object locko = new object();
					client.DownloadProgressChanged += (s, e) =>
					{
						lock (locko)
						{
							int len = (int)((Console.WindowWidth - 2) * 0.01 * e.ProgressPercentage);
							Console.SetCursorPosition(0, progressBarPosition);
							Console.Write("[");
							Console.Write((new string('=', Math.Max(0, len - 1)) + ">").PadRight(Console.WindowWidth - 2));
							Console.Write("]");
						}
					};
					client.DownloadFileCompleted += (s, e) =>
					{
						finished = true;
					};
					Console.WriteLine("Download {0}...", this.Name);

					progressBarPosition = Console.CursorTop;
					client.DownloadFileAsync(uri, tempFile);

					while (!finished) Thread.Sleep(50);

					Console.WriteLine("Install {0}...", this.Name);

					if (fileName.EndsWith(".exe"))
					{
						Run(tempFile, "");
					}
					else
					{
						if (Extract("x \"" + tempFile + "\" -o\"" + targetPath + "\"") != 0)
						{
							throw new InvalidOperationException("Failed to extract the package.");
						}

						// Double extract .tar.gz
						if (fileName.EndsWith(".tar.gz"))
						{
							string tarFile = targetPath + Path.GetFileNameWithoutExtension(tempFile);

							int exitCode = Extract("x \"" + tarFile + "\" -o\"" + targetPath + "\"");

							// Remove .tar file, we don't need it.
							File.Delete(tarFile);

							if (exitCode != 0)
							{
								throw new InvalidOperationException("Failed to extract the package.");
							}
						}
					}
					Console.WriteLine("Finished {0}.", this.Name);
				}
			}
			catch
			{
				// "Rollback"
				Directory.Delete(targetPath);
			}
			finally
			{
				// Delete temporary file.
				if (File.Exists(tempFile))
					File.Delete(tempFile);
			}
		}

		/// <summary>
		/// Extracts an archive with 7zip
		/// </summary>
		/// <param name="args">Command line arguments</param>
		/// <returns></returns>
		private static int Extract(string args)
		{
			return Run(Config.Default.Extractor, args);
		}

		/// <summary>
		/// Extracts an archive with 7zip
		/// </summary>
		/// <param name="args">Command line arguments</param>
		/// <returns></returns>
		private static int Run(string file, string args)
		{
			Process process = new Process();
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.FileName = file;
			process.StartInfo.Arguments = args;
			process.Start();

			string output;
			while (!process.HasExited)
			{
				output = process.StandardOutput.ReadToEnd();
				if (Config.Default.Verbose)
					Console.Write(output);
			}

			output = process.StandardOutput.ReadToEnd();
			if (Config.Default.Verbose)
				Console.Write(output);
			return process.ExitCode;
		}
	}
}
