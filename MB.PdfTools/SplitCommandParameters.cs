namespace MB.PdfTools
{
    public class SplitCommandParameters
    {
        public string[] Files { get; private set; }
        public string OutFile { get; private set; }

        public SplitCommandParameters(IEnumerable<string> files, string outFiile)
        {
            Files = files.ToArray();
            OutFile = outFiile;
        }
    }
}