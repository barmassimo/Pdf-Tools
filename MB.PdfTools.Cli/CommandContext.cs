using MB.PdfTools;
using Microsoft.Extensions.Configuration;

namespace MB.PdfTools.Cli
{
    public class CommandContext
    {
        public Command Command { get; set; }

        public IEnumerable<string> Files { get; private set; }

        public Options Options { get; set; }

        public CommandContext()
        {
            Files = new List<string>();
            Options = new Options();
        }

        public static CommandContext FromCommandLineArgs(string[] args)
        {
            ArgumentNullException.ThrowIfNull(args);

            var result = new CommandContext();

            var builder = new ConfigurationBuilder().AddCommandLine(args);
            var configuration = builder.Build();

            args = args.Where(x => !x.StartsWith("--")).ToArray(); // remove options

            // command
            if (args.Length == 0)
                throw new Exception("You should specify a command.");

            var command = args[0];
            args = args.Skip(1).ToArray();

            switch (command)
            {
                case "m":
                case "merge":
                    result.Command = Command.Merge;
                    break;
                case "s":
                case "split":
                    result.Command = Command.Spit;
                    break;
                default:
                    throw new Exception($"Unknown command {command}.");
            }

            // files
            if (args.Length == 0)
                throw new Exception("You should specify at least one file.");

            result.AddFiles(args);

            // options
            result.Options.OutFile = configuration["outFile"] ?? configuration["of"];
            if (result.Options.OutFile == null)
            {
                result.Options.OutFile = result.Command == Command.Merge
                    ? "merged_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf"
                    : "splitted_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
            }

            if (result.Options.OutFile.ToLower().EndsWith(".jpg"))
            {
                result.Options.OutFile = result.Options.OutFile.Substring(0, result.Options.OutFile.Length - 4);
            }

            result.Options.Orientation = configuration["orientation"] ?? configuration["or"];

            return result;
        }

        private void AddFiles(IEnumerable<string> files)
        {
            ArgumentNullException.ThrowIfNull(files);

            foreach (var f in files)
            {
                ((List<string>)Files).Add(f);
            }
        }
    }

    public enum Command
    {
        Merge,
        Spit
    }

    public class Options
    {
        public string? OutFile { get; set; }
        public string? Orientation { get; set; }
    }
}