using Microsoft.Extensions.Configuration;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using System.Reflection;

Console.WriteLine($"PdfMerge v." + Assembly.GetEntryAssembly()?.GetName().Version?.ToString());
Console.WriteLine();

var builder = new ConfigurationBuilder().AddCommandLine(args);
var configuration = builder.Build();

args = args.Where(x => !x.StartsWith("--")).ToArray(); // remove options

if (args.Length == 0)
{
    Console.WriteLine($"Description:\n  Merges one or more jpg, png or pdf files into a single pdf file\n");
    Console.WriteLine($"Usage:\n  PdfMerge.exe files [options]\n");
    Console.WriteLine($"Arguments:\n  files:  list of jpg, png, pdf files\n");
    Console.WriteLine($"Options:\n  --outFile, --of:  output file name (the default is merged_yyyyMMdd_HHmmss.pdf)\n");
    Console.WriteLine($"Example:\n  PdfMerge.exe mydoc.pdf logo1.png logo2.jpg --outFile=merged.pdf\n");
    return 1;
}

foreach (var file in args)
{
    var info = new FileInfo(file);

    if (!info.Exists)
    {
        Console.WriteLine($"File '{file}' not found. Exiting.");
        return 1;
    }

    var extension = info.Extension.ToLower();

    if (extension!=".pdf" && extension != ".jpg" && extension != ".png")
    {
        Console.WriteLine($"Only pdf, jpg and png files accepted. Exiting");
        return 1;
    }
}

using (PdfDocument outPdf = new PdfDocument())
{
    foreach (var file in args)
    {
        Console.Write($"Adding file '{file}'... ");

        if (file.ToLower().EndsWith(".pdf"))
        {
            using (PdfDocument doc = PdfReader.Open(file, PdfDocumentOpenMode.Import))
            {
                var nPages = CopyPages(doc, outPdf);
                Console.WriteLine($"Ok ({nPages} pages)");
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

            float pageX = (int)page.MediaBox.Size.Width;
            float pageY = (int)page.MediaBox.Size.Height;

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

            Console.WriteLine("Ok");
        }

    }

    var outFile = configuration["outFile"] ?? configuration["of"] ?? "merged_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";

    outPdf.Save(outFile);

    Console.WriteLine();
    Console.WriteLine($"File '{outFile}' created.");
}

return 0;

int CopyPages(PdfDocument from, PdfDocument to)
{
    for (int i = 0; i < from.PageCount; i++)
    {
        to.AddPage(from.Pages[i]);
    }

    return from.PageCount;
}
