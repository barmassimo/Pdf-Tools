using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using System.Reflection;

Console.WriteLine($"PdfMerge v." + Assembly.GetEntryAssembly()?.GetName().Version?.ToString());
Console.WriteLine($"Merges one or more jpg or pdf files into a single pdf file.");
Console.WriteLine();

if (args.Length==0)
{
    Console.WriteLine($"Usage: PdfMerge.exe file1 [file2 file3 ... fileN]");
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

    if (extension!=".pdf" && extension!=".jpg")
    {
        Console.WriteLine($"Only pdf and jpg files accepted. Exiting");
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
                CopyPages(doc, outPdf);
            }
        }
        else if (file.ToLower().EndsWith(".jpg"))
        {
            PdfPage page = outPdf.AddPage();
            // page.Size = PdfSharpCore.PageSize.A5;
            XGraphics gfx = XGraphics.FromPdfPage(page);
            DrawImage(gfx, file, 0, 0, (int)page.Width, (int)page.Height);
        }

        Console.WriteLine("Ok");
    }

    var outFile = DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";

    outPdf.Save(outFile);

    Console.WriteLine($"File '{outFile}' created.");
}

return 0;

void CopyPages(PdfDocument from, PdfDocument to)
{
    for (int i = 0; i < from.PageCount; i++)
    {
        to.AddPage(from.Pages[i]);
    }
}

void DrawImage(XGraphics gfx, string file, int x, int y, int width, int height)
{
    XImage image = XImage.FromFile(file);
    gfx.DrawImage(image, x, y, width, height);
}