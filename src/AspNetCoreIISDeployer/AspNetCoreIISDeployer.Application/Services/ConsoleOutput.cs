namespace AspNetCoreIISDeployer.Application.Services
{
    public class ConsoleOutput
    {
        public ConsoleOutput(string text, bool isError)
        {
            Text = text;
            IsError = isError;
        }

        public string Text { get; }

        public bool IsError { get; }

        public override string ToString()
        {
            return Text;
        }

        public static ConsoleOutput FromSingleOutputLine(string outputLine)
        {
            return new ConsoleOutput(outputLine, false);
        }

        public static ConsoleOutput FromSingleErrorLine(string errorLine)
        {
            return new ConsoleOutput(errorLine, true);
        }
    }
}