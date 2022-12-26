using Microsoft.Extensions.Configuration;
using System.Reflection;


using MB.PdfTools;

var executableName = Assembly.GetExecutingAssembly().GetName().Name + ".exe";

Console.WriteLine($"PdfTools v." + Assembly.GetEntryAssembly()?.GetName().Version?.ToString());
Console.WriteLine();

var builder = new ConfigurationBuilder().AddCommandLine(args);
var configuration = builder.Build();

var help = $@"Description:
An utility to merge and split pdf files and images

Usage:
  {executableName} command files [options]

Available commands:
  m, merge: merges one or more jpg, png or pdf files into a single pdf file
  
Parameters:
  files: a list of jpg, png, pdf files
  
Options:
  --outFile, --of:  output file name (the default is merged_yyyyMMdd_HHmmss.pdf)

Example:
  {executableName} merge mydoc.pdf logo1.png logo2.jpg --outFile=merged.pdf
";

args = args.Where(x => !x.StartsWith("--")).ToArray(); // remove options

string? command = null;

if (args.Length > 0)
{
    command = args[0];
    args = args.Skip(1).ToArray();
}

if (args.Length == 0)
{
    Console.WriteLine(help);
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

    if (extension != ".pdf" && extension != ".jpg" && extension != ".png")
    {
        Console.WriteLine($"Only pdf, jpg and png files accepted. Exiting");
        return 1;
    }
}

if (command == "m" || command == "merge")
{
    var outFile = configuration["outFile"] ?? configuration["of"] ?? "merged_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";

    var result = new MergeCommand().Execute(new MergeCommandParameters(args, outFile));
    if (result.IsOk)
    {
        Console.WriteLine(result.Output);
        return 0;
    }
    else
    {
        Console.WriteLine(result.Output);
        Console.WriteLine(result.ErrorMessage);
        return 1;
    }
}
else
{
    Console.WriteLine($"Command '{command}' not available. Exiting.");
    return 1;
}


