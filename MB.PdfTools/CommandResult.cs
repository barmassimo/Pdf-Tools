namespace MB.PdfTools
{
    public class CommandResult
    {
        public string? Output { get; set; }
        public string? ErrorMessage { get; private set; }
        public bool IsOk { get { return ErrorMessage == null; } }

        public static CommandResult Error(string errorMessage, string output)
        {
            ArgumentNullException.ThrowIfNull(errorMessage);
            ArgumentNullException.ThrowIfNull(output);

            return new CommandResult { ErrorMessage = errorMessage, Output = output };
        }

        public static CommandResult Ok(string output)
        {
            ArgumentNullException.ThrowIfNull(output);

            return new CommandResult { Output = output };
        }

        private CommandResult() { }
    }
}