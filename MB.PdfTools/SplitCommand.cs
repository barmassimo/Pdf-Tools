using System.Text;
using ImageMagick;

namespace MB.PdfTools
{
    public class SplitCommand : ICommand<SplitCommandParameters>
    {
        public CommandResult Execute(SplitCommandParameters commandParameters)
        {
            var sbOut = new StringBuilder();
            var totalPages = 1;

            //MagickNET.SetGhostscriptDirectory(@"c:\ghostScript");

            var settings = new MagickReadSettings();
            settings.Density = new Density(300, 300);

            foreach (var file in commandParameters.Files)
            {
                using (var images = new MagickImageCollection())
                {
                    sbOut.Append($"Splitting file '{file}'... ");

                    try
                    {
                        images.Read(file, settings);
                    }
                    catch (ImageMagick.MagickDelegateErrorException ex)
                    {
                        sbOut.AppendLine($"\nERROR: cannot create images; please check that Ghostscript is installed on your machine (see command help for details)");
                        return CommandResult.Error(ex.Message, sbOut.ToString());
                    }

                    foreach (var image in images)
                    {
                        image.Format = MagickFormat.Jpg;
                        image.Write($"{commandParameters.OutFile}_{totalPages++:0000}.jpg");
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