namespace MB.PdfTools
{
    public class SplitCommandParameters
    {
        public string[] Files { get; private set; }
        public string OutFile { get; private set; }

        public SplitCommandParameters(IEnumerable<string> files, string outFile)
        {
            ArgumentNullException.ThrowIfNull(files);
            ArgumentNullException.ThrowIfNull(outFile);

            Files = files.ToArray();
            OutFile = outFile;
        }
    }
}