using Configurator.Entities;

namespace Configurator.Repositories
{
    public class DirectoryConfigRepository : IConfigRepository
    {
        protected string RootDir;
 
        public DirectoryConfigRepository()
        {
            RootDir = "local\\configs";
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
                var category = Enum.GetName(typeof(IConfigRepository.CATEGORIES), i);
                if (!Directory.Exists(RootDir + "/" + category))
                {
                    Directory.CreateDirectory(RootDir + "/" + category);
                }
            }
        }

        public virtual Task DeleteConfigs(IEnumerable<Config> configs)
        {
            try
            {
                foreach (var config in configs)
                {
                    string filePath = RootDir + "\\" + config.Category + "\\" + config.Name;

                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
            return Task.CompletedTask;
        }

        private async Task<IEnumerable<Config>> GetConfigs() {
            var configs = new List<Config>();
            try
            {
                foreach (int i in Enum.GetValues(typeof(IConfigRepository.CATEGORIES)))
                {
                    var category = Enum.GetName(typeof(IConfigRepository.CATEGORIES), i);

                    var files = Directory.GetFiles(RootDir + "\\" + category, "*.yaml");

                    foreach (var file in files)
                    {
                        string data = await File.ReadAllTextAsync(file);
                        string fileName = Path.GetFileName(file);
                        configs.Add(new(category, fileName, data));
                    }
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

        public virtual async Task<IEnumerable<Config>> ModifyConfigs(IEnumerable<Config> configs)
        {
            try
            {
                foreach (var config in configs)
                {
                    string filePath = RootDir + "\\" + config.Category + "\\" + config.Name;

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
