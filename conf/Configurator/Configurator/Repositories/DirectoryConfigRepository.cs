using Configurator.Entities;

namespace Configurator.Repositories
{
    public class DirectoryConfigRepository : IConfigRepository
    {
        protected string RootDir;
 
        public DirectoryConfigRepository()
        {
            RootDir = Path.Combine("local", "configs");
            InitDirectories();
        }

        public void InitDirectories()
        {
            if (!Directory.Exists(RootDir))
            {
                Directory.CreateDirectory(RootDir);
            }
            foreach (int i in Enum.GetValues(typeof(IConfigRepository.CATEGORIES)))
            {
                var category = Enum.GetName(typeof(IConfigRepository.CATEGORIES), i) ?? throw new Exception("Enum error");
                if (!Directory.Exists(Path.Combine(RootDir, category)))
                {
                    Directory.CreateDirectory(Path.Combine(RootDir, category));
                }
            }
        }

        public virtual Task<IEnumerable<ConfigId>> DeleteConfigs(IEnumerable<ConfigId> configs)
        {
            List<ConfigId> deletedConfigs = new();

            try
            {
                foreach (var config in configs)
                {
                    string extension = config.Category == "datasources" ? ".json" : ".yaml";
                    string filePath = Path.Combine(RootDir, config.Category, config.ApiName + "-" + config.ApiVersion + extension);

                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        deletedConfigs.Add(config);
                    }
                }
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return Task.FromResult(deletedConfigs as IEnumerable<ConfigId>);
        }
        private async Task<IEnumerable<Config>> GetCategoryConfigs(string category)
        {
            var configs = new List<Config>();

            if (!Enum.IsDefined(typeof(IConfigRepository.CATEGORIES), category))
            {
                return configs;
            }
            try
            {
                string extension = category == "datasources" ? "*.json" : "*.yaml";

                var files = Directory.GetFiles(Path.Combine(RootDir, category), extension);

                foreach (var file in files)
                {
                    string data = await File.ReadAllTextAsync(file);
                    string fileName = Path.GetFileName(file);
                    string[] splitFileName = fileName.Split("-");
                    string apiName = splitFileName[0];
                    string apiVersion = splitFileName[1].Split(".")[0];
                    configs.Add(new(category, apiName, apiVersion, data));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return configs;
        }

        private async Task<IEnumerable<Config>> GetConfigs() {
            var configs = new List<Config>();

            try
            {
                foreach (int i in Enum.GetValues(typeof(IConfigRepository.CATEGORIES)))
                {
                    var category = Enum.GetName(typeof(IConfigRepository.CATEGORIES), i);

                    if (category == null) continue;

                    configs = configs.Concat(await GetCategoryConfigs(category)).ToList();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return configs;
        }

        public virtual async Task<IEnumerable<Config>> GetAllConfigs()
        {
            return await GetConfigs();
        }

        public virtual async Task<IEnumerable<Config>> GetConfigsByCategory(string category)
        {
            return await GetCategoryConfigs(category);
        }

        public virtual async Task<IEnumerable<Config>> ModifyConfigs(IEnumerable<Config> configs)
        {
            try
            {
                foreach (var config in configs)
                {
                    string extension = config.Category == "datasources" ? ".json" : ".yaml";
                    string filePath = Path.Combine(RootDir, config.Category, config.ApiName + "-" + config.ApiVersion + extension);

                    await File.WriteAllTextAsync(filePath, config.Data);
                }
            } catch (Exception e) 
            {
                Console.WriteLine(e.Message);
            }
            return await GetConfigs();
        }


    }
}
