namespace MB.PdfTools
{
    public class MergeCommandParameters
    {
        public string[] Files { get; private set; }
        public string OutFile { get; private set; }
        public string? Orientation { get; private set; }

        public MergeCommandParameters(IEnumerable<string> files, string outFile, string? orientation)
        {
            ArgumentNullException.ThrowIfNull(files);
            ArgumentNullException.ThrowIfNull(outFile);
            ArgumentNullException.ThrowIfNull(orientation);

            Files = files.ToArray();
            OutFile = outFile;
            Orientation = orientation;
        }
    }
}