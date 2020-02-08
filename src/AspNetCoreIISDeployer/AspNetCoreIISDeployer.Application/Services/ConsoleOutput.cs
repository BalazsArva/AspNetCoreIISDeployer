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
    }
}