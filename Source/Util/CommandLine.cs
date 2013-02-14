using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Xi.Util
{
	internal static class CommandLine
	{
		public static string Filename { get; private set; }
		public static List<string> Flags { get; private set; }
		public static List<string> Arguments { get; private set; } 

		static CommandLine()
		{
			Flags = new List<string>();
			Arguments = new List<string>();
			Filename = null;
		}

		public static void ParseArguments(string[] args)
		{
			if (args.Length < 1)
				return;

			int i = 0;

			while(i < args.Length && args[i].First() == '-')
				Flags.Add(args[i++]);

			if (i < args.Length)
				Filename = args[i++];

			while(i < args.Length)
				Arguments.Add(args[i++]);
		}

		public static void PrintUsage()
		{
			const int helpTopicTabLength = 14;

			Dictionary<string, string> helpTopics = new Dictionary<string, string>
			{
				{ "[filename]",		"Filename of script to run." },
				{ "[flags]",		"Flags that change the behavior of the script." },
				{ "--version",		"Display Xi version." },
				{ "--help",			"Display this information." }
			};

			Console.WriteLine("Usage: Xi [flags] [filename] [args]");
			foreach (KeyValuePair<string, string> topic in helpTopics)
			{
				int indentValue = topic.Key.Length;
				Console.Write(topic.Key);

				foreach (string helpValue in topic.Value.Split(new[] { '\n' }))
				{
					for (; indentValue < helpTopicTabLength; ++indentValue)
						Console.Write(" ");

					Console.WriteLine(helpValue);
					indentValue = 0;
				}
			}

			Environment.Exit(0);
		}

		public static void PrintVersion()
		{
			Console.WriteLine("Xi {0}", Assembly.GetExecutingAssembly().GetName().Version);
			Environment.Exit(0);
		}
	}
}
