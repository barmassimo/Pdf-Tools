using System.Text;
using ImageMagick;

namespace MB.PdfTools
{
    public class SplitCommand : ICommand
    {
        private SplitCommandParameters _commandParameters;

        public SplitCommand(SplitCommandParameters commandParameters)
        {
            ArgumentNullException.ThrowIfNull(commandParameters);

            _commandParameters = commandParameters;
        }

        public CommandResult Execute()
        {
            var sbOut = new StringBuilder();

            // checking files
            foreach (var file in _commandParameters.Files)
            {
                var info = new FileInfo(file);

                if (!info.Exists)
                    return CommandResult.Error($"File '{file}' not found.", sbOut.ToString());

                var extension = info.Extension.ToLower();

                if (extension != ".pdf")
                    return CommandResult.Error($"Only pdf files accepted.", sbOut.ToString());
            }

            var totalPages = 1;

            //MagickNET.SetGhostscriptDirectory(@"c:\ghostScript");

            var settings = new MagickReadSettings();
            settings.Density = new Density(300, 300);

            foreach (var file in _commandParameters.Files)
            {
                using (var images = new MagickImageCollection())
                {
                    sbOut.Append($"Splitting file '{file}'... ");

                    try
                    {
                        images.Read(file, settings);
                    }
                    catch (MagickDelegateErrorException ex)
                    {
                        sbOut.AppendLine($"\nERROR: cannot create images; please check that Ghostscript is installed on your machine (see command help for details)");
                        return CommandResult.Error(ex.Message, sbOut.ToString());
                    }

                    foreach (var image in images)
                    {
                        image.Format = MagickFormat.Jpg;
                        image.Write($"{_commandParameters.OutFile}_{totalPages++:0000}.jpg");
                    }

                    sbOut.AppendLine($"Ok ({images.Count} pages)");
                }
            }

            sbOut.AppendLine();
            sbOut.AppendLine($"File(s) created.");

            return CommandResult.Ok(sbOut.ToString());
        }
    }
}