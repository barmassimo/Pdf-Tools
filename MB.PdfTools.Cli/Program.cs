using MB.PdfTools.Cli;
using System.Reflection;

var executableName = Assembly.GetExecutingAssembly().GetName().Name + ".exe";

var help = $@"Description:
  PdfTools is a command line utility to merge and split pdf files and images

Usage:
  {executableName} command files [options]

Available commands:

## merge
  m, merge: merges one or more jpg, png or pdf files into a single pdf file
  
Parameters:
  files: a list of jpg, png, pdf files
  
Options:
  --outFile, --of:  output file name (the default is merged_yyyyMMdd_HHmmss.pdf)
  --orientation, --or: orientation for pages created from images
                       values are: p=portrait (default), l=landscape, a=auto

Examples:
  {executableName} m mydoc.pdf logo1.png logo2.jpg --outFile=merged.pdf --or=a

## split
  s, split: splits one or more pdf files into n jpg images, one per page
  (note: this command requires ghostscript: https://ghostscript.com/releases/gsdnld.html)
  
Parameters:
  files: a list of pdf files
  
Options:
  --outFile, --of:  output file name (the default is splitted_yyyyMMdd_HHmmss_n.jpg)

Examples:
  {executableName} s mydoc.pdf --outFile=hello
  {executableName} s mydoc.pdf --outFile=hello.jpg

  both commands create: hello_0001.jpg, hello_0002.jpg, ...
";

Console.WriteLine($"PdfTools v." + Assembly.GetEntryAssembly()?.GetName().Version?.ToString());
Console.WriteLine();

if (args.Length == 0)
{
    Console.WriteLine(help);
    return 0;
}

CommandContext ctx;

try
{
    ctx = CommandContext.FromCommandLineArgs(args);
}
catch (Exception ex)
{
    Console.WriteLine($"{ex.Message} See help for details.");
    return 1;
}

var result = new CommandBuilder().BuildCommand(ctx).Execute();

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