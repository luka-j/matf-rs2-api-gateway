using Configurator.Entities;
using System.Diagnostics;

namespace Configurator.Repositories
{
    public class GitHubConfigRepository : DirectoryConfigRepository
    {
        private readonly IConfiguration _configuration;

        private readonly string REPO_DIR = "local\\repo";

        private readonly string _repositoryName;
        private readonly string _repositoryURL;

        public GitHubConfigRepository(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _repositoryName = _configuration["GitHubSettings:GitHubRepoName"];
            _repositoryURL = _configuration["GithubSettings:GitHubRepoURL"];

            RootDir = REPO_DIR + "\\" + _repositoryName;
        }

        public override async Task DeleteConfigs(IEnumerable<Config> configs)
        {
            await CloneOrPull();
            await base.DeleteConfigs(configs);
            await CommitAndPush();
        }

        public override async Task<IEnumerable<Config>> GetAllConfigs()
        {
            await CloneOrPull();
            return await base.GetAllConfigs();
        }

        public override async Task<IEnumerable<Config>> ModifyConfigs(IEnumerable<Config> configs)
        {
            await CloneOrPull();
            var result = await base.ModifyConfigs(configs);
            await CommitAndPush();
            return result;
        }

        private async Task CommitAndPush()
        {
            ProcessStartInfo add = new()
            {
                WorkingDirectory = RootDir,
                FileName = "git",
                Arguments = "add ."
            };
            Process pAdd = new()
            {
                StartInfo = add
            };

            pAdd.Start();
            await pAdd.WaitForExitAsync();

            ProcessStartInfo commit = new()
            {
                WorkingDirectory = RootDir,
                FileName = "git",
                Arguments = "commit -m \"config-" + DateTime.Now.ToString() + "\""
            };
            Process pCommit = new()
            {
                StartInfo = commit
            };

            pCommit.Start();
            await pCommit.WaitForExitAsync();

            ProcessStartInfo push = new()
            {
                WorkingDirectory = RootDir,
                FileName = "git",
                Arguments = "push"
            };
            Process pPush = new()
            {
                StartInfo = push
            };

            pPush.Start();
            await pPush.WaitForExitAsync();
        }

        private async Task CloneOrPull()
        {
            if (!Directory.Exists(REPO_DIR))
            {
                Directory.CreateDirectory(REPO_DIR);
            }

            if (!Directory.Exists(RootDir))
            {

                ProcessStartInfo clone = new()
                {
                    WorkingDirectory = REPO_DIR,
                    FileName = "git",
                    Arguments = "clone " + _repositoryURL
                };
                Process pClone = new()
                {
                    StartInfo = clone
                };
                pClone.Start();
                await pClone.WaitForExitAsync();
            } else
            {
                ProcessStartInfo reset = new()
                {
                    WorkingDirectory = RootDir,
                    FileName = "git",
                    Arguments = "reset --hard HEAD"
                };
                Process pReset = new()
                {
                    StartInfo = reset
                };
                pReset.Start();
                await pReset.WaitForExitAsync();

                ProcessStartInfo pull = new()
                {
                    WorkingDirectory = RootDir,
                    FileName = "git",
                    Arguments = "pull"
                };
                Process pPull = new()
                {
                    StartInfo = pull
                };
                pPull.Start();
                await pPull.WaitForExitAsync();
            }
            
            InitDirectories();
        }
    }
}
