namespace MB.PdfTools
{
    public class MergeCommandParameters
    {
        public string[] Files { get; private set; }
        public string OutFile { get; private set; }
        public string? Orientation { get; private set; }

        public MergeCommandParameters(IEnumerable<string> files, string outFile, string? orientation)
        {
            Files = files.ToArray();
            OutFile = outFile;
            Orientation = orientation;
        }
    }
}