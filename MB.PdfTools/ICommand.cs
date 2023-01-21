namespace MB.PdfTools
{
    public interface ICommand<T>
    {
        public CommandResult Execute(T commandParameters);
    }
}