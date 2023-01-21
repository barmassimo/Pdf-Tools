using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using System.Text;

namespace MB.PdfTools
{
    public class MergeCommand : ICommand<MergeCommandParameters>
    {
        public CommandResult Execute(MergeCommandParameters commandParameters)
        {
            var sbOut = new StringBuilder();

            using (PdfDocument outPdf = new PdfDocument())
            {
                foreach (var file in commandParameters.Files)
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

                        page.Orientation = PdfSharpCore.PageOrientation.Portrait;
                        
                        if (commandParameters.Orientation == "l")
                            page.Orientation = PdfSharpCore.PageOrientation.Landscape;

                        if (commandParameters.Orientation == "a" && imgX > imgY) // auto orientation
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

                outPdf.Save(commandParameters.OutFile);

                sbOut.AppendLine();
                sbOut.AppendLine($"File '{commandParameters.OutFile}' created.");

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