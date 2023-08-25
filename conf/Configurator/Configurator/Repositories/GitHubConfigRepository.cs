using Configurator.Entities;
using LibGit2Sharp;

namespace Configurator.Repositories
{
    public class GitHubConfigRepository : DirectoryConfigRepository
    {
        private readonly IConfiguration _configuration;

        private readonly string REPO_DIR = Path.Combine("local", "repo");

        private readonly string _repositoryName;
        private readonly string _repositoryURL;
        private readonly string _gitUsername;
        private readonly string _gitEmail;
        private readonly string _gitPat;
        public GitHubConfigRepository(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _repositoryName = _configuration["GitHubSettings:GitHubRepoName"];
            _repositoryURL = _configuration["GithubSettings:GitHubRepoURL"];
            _gitUsername = _configuration["GithubSettings:GitUsername"];
            _gitEmail = _configuration["GithubSettings:GitEmail"];
            _gitPat = _configuration["GithubSettings:PAT"];

            var options = new CloneOptions
            {
                CredentialsProvider = GetCredentialsHandler(),
            };
            RootDir = Path.Combine(REPO_DIR, _repositoryName);
            if (!Repository.IsValid(RootDir))
                Repository.Clone(_repositoryURL, RootDir, options);
        }

        public override async Task<IEnumerable<ConfigId>> DeleteConfigs(IEnumerable<ConfigId> configs)
        {
            Pull();
            var deletedConfigs = await base.DeleteConfigs(configs);
            CommitAndPush();
            return deletedConfigs;
        }

        public override async Task<IEnumerable<Config>> GetAllConfigs()
        {
            Pull();
            return await base.GetAllConfigs();
        }
        public override async Task<IEnumerable<Config>> GetConfigsByCategory(string category)
        {
            Pull();
            return await base.GetConfigsByCategory(category);
        }
        public override async Task<IEnumerable<Config>> ModifyConfigs(IEnumerable<Config> configs)
        {
            Pull();
            var result = await base.ModifyConfigs(configs);
            CommitAndPush();
            return result;
        }

        private LibGit2Sharp.Handlers.CredentialsHandler GetCredentialsHandler()
        {
            return (_url, _user, _cred) => new UsernamePasswordCredentials
            {
                Username = _gitPat,
                Password = _gitPat
            };

        }
        private void CommitAndPush()
        {
            using Repository repo = new(RootDir);

            var status = repo.RetrieveStatus();
            if (!status.IsDirty) { return; }

            Commands.Stage(repo, "*");

            var sig = new Signature(_gitUsername, _gitEmail, DateTimeOffset.Now);
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
            Commands.Pull(repo, new Signature(_gitUsername, _gitEmail, DateTimeOffset.Now), pullOptions);
            InitDirectories();
        }
    }
}
