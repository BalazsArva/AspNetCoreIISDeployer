namespace AspNetCoreIISDeployer.Application.Services.DotNet
{
    public class GitPublishInfo
    {
        public static readonly GitPublishInfo Empty = new GitPublishInfo(string.Empty, string.Empty);

        public GitPublishInfo(string branch, string commit)
        {
            Branch = branch ?? string.Empty;
            Commit = commit ?? string.Empty;
        }

        public string Branch { get; }

        public string Commit { get; }
    }
}