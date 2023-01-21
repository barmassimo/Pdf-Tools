using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using System.Text;

namespace MB.PdfTools
{
    public class MergeCommand : ICommand
    {
        private MergeCommandParameters _commandParameters;

        public MergeCommand(MergeCommandParameters commandParameters)
        {
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

                if (extension != ".pdf" && extension != ".jpg" && extension != ".png")
                    return CommandResult.Error($"Only pdf, jpg and png files accepted.", sbOut.ToString());
            }

            using (PdfDocument outPdf = new PdfDocument())
            {
                foreach (var file in _commandParameters.Files)
                {
                    sbOut.Append($"Adding file '{file}'... ");

                    if (file.ToLower().EndsWith(".pdf"))
                    {
                        using (PdfDocument doc = PdfReader.Open(file, PdfDocumentOpenMode.Import))
                        {
                            var nPages = CopyPages(doc, outPdf);
                            sbOut.AppendLine($"Ok ({nPages} pages)");
                        }
                    }
                    else if (file.ToLower().EndsWith(".jpg") || file.ToLower().EndsWith(".png"))
                    {
                        PdfPage page = outPdf.AddPage();

                        // page.Size = PdfSharpCore.PageSize.A5;

                        XGraphics gfx = XGraphics.FromPdfPage(page);
                        XImage image = XImage.FromFile(file);

                        float imgX = image.PixelWidth;
                        float imgY = image.PixelHeight;

                        page.Orientation = PdfSharpCore.PageOrientation.Portrait; // default
                        
                        if (_commandParameters.Orientation == "l")
                            page.Orientation = PdfSharpCore.PageOrientation.Landscape;

                        if (_commandParameters.Orientation == "a" && imgX > imgY) // auto orientation
                            page.Orientation = PdfSharpCore.PageOrientation.Landscape;

                        float pageX = (int)page.MediaBox.Size.Width;
                        float pageY = (int)page.MediaBox.Size.Height;

                        if (page.Orientation == PdfSharpCore.PageOrientation.Landscape)
                        {
                            var tmp = pageX;
                            pageX = pageY;
                            pageY = tmp;
                        }

                        var imgRatio = imgX / imgY;
                        var pageRatio = pageX / pageY;

                        int startX, startY, finalX, finalY;

                        if (imgRatio > pageRatio)
                        {
                            finalX = (int)pageX;
                            finalY = (int)(imgY * pageX / imgX);

                            startX = 0;
                            startY = (int)(pageY - finalY) / 2;
                        }
                        else
                        {
                            finalX = (int)(imgX * pageY / imgY);
                            finalY = (int)pageY;

                            startX = (int)(pageX - finalX) / 2; ;
                            startY = 0;
                        }

                        gfx.DrawImage(image, startX, startY, finalX, finalY);

                        sbOut.AppendLine("Ok");
                    }
                }

                outPdf.Save(_commandParameters.OutFile);

                sbOut.AppendLine();
                sbOut.AppendLine($"File '{_commandParameters.OutFile}' created.");

                return CommandResult.Ok(sbOut.ToString());
            }
        }

        private int CopyPages(PdfDocument from, PdfDocument to)
        {
            for (int i = 0; i < from.PageCount; i++)
            {
                to.AddPage(from.Pages[i]);
            }

            return from.PageCount;
        }
    }
}