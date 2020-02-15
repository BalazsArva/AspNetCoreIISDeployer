namespace AspNetCoreIISDeployer.Application.Services.ApplicationServices
{
    public class GitRepositoryInfo
    {
        public static readonly GitRepositoryInfo Empty = new GitRepositoryInfo(string.Empty, string.Empty, string.Empty);

        public GitRepositoryInfo(string branch, string commit, string remoteCommit)
        {
            Branch = branch ?? string.Empty;
            Commit = commit ?? string.Empty;
            RemoteCommit = remoteCommit ?? string.Empty;
        }

        public string Branch { get; }

        public string Commit { get; }

        public string RemoteCommit { get; }
    }
}