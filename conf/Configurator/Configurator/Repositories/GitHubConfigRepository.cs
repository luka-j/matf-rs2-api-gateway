using Configurator.Entities;
using LibGit2Sharp;
using Microsoft.Alm.Authentication;

namespace Configurator.Repositories
{
    public class GitHubConfigRepository : DirectoryConfigRepository
    {
        private readonly IConfiguration _configuration;

        private readonly string REPO_DIR = "local\\repo";

        private readonly string _repositoryName;
        private readonly string _repositoryURL;
        private readonly string _gitUserName;
        private readonly string _gitEmail;

        public GitHubConfigRepository(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _repositoryName = _configuration["GitHubSettings:GitHubRepoName"];
            _repositoryURL = _configuration["GithubSettings:GitHubRepoURL"];
            _gitUserName = _configuration["GithubSettings:GitUserName"];
            _gitEmail = _configuration["GithubSettings:GitEmail"];

            var options = new CloneOptions
            {
                CredentialsProvider = GetCredentialsHandler(),
            };
            RootDir = REPO_DIR + "\\" + _repositoryName;
            if (!Repository.IsValid(RootDir))
                Repository.Clone(_repositoryURL, RootDir, options);
        }

        public override async Task DeleteConfigs(IEnumerable<Config> configs)
        {
            Pull();
            await base.DeleteConfigs(configs);
            CommitAndPush();
        }

        public override async Task<IEnumerable<Config>> GetAllConfigs()
        {
            Pull();
            return await base.GetAllConfigs();
        }

        public override async Task<IEnumerable<Config>> ModifyConfigs(IEnumerable<Config> configs)
        {
            Pull();
            var result = await base.ModifyConfigs(configs);
            CommitAndPush();
            return result;
        }

        private static Credential GetCredentials()
        {
            var secrets = new SecretStore("git");
            var auth = new BasicAuthentication(secrets);
            return auth.GetCredentials(new TargetUri("https://github.com"));
        }

        private LibGit2Sharp.Handlers.CredentialsHandler GetCredentialsHandler()
        {
            var creds = GetCredentials();

            return (_url, _user, _cred) => new UsernamePasswordCredentials
            {
                Username = creds.Username,
                Password = creds.Password
            };
        }
        private void CommitAndPush()
        {
            using Repository repo = new(RootDir);
            RepositoryStatus status = repo.RetrieveStatus();
            List<string> filePaths = status.Modified.Select(entry => entry.FilePath).ToList();
            Commands.Stage(repo, filePaths);

            var sig = new Signature(_gitUserName, _gitEmail, DateTimeOffset.Now);
            repo.Commit("config-" + DateTime.Now.ToString(), sig, sig);

            PushOptions options = new()
            {
                CredentialsProvider = GetCredentialsHandler()
            };
            repo.Network.Push(repo.Head, options);
        }

        private void Pull()
        {
            using Repository repo = new(RootDir);
            PullOptions pullOptions = new()
            {
                MergeOptions = new MergeOptions
                {
                    FileConflictStrategy = CheckoutFileConflictStrategy.Theirs
                },
                FetchOptions = new FetchOptions
                {
                    CredentialsProvider = GetCredentialsHandler()
                }
            };

            Commands.Pull(repo, new Signature(_gitUserName, _gitEmail, DateTimeOffset.Now), pullOptions);
            InitDirectories();
        }
    }
}
