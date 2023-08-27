using Configurator.Entities;
using Google.Protobuf;

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
                var path = Path.Combine(RootDir, category);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            foreach (int i in Enum.GetValues(typeof(IConfigRepository.DATASOURCES)))
            {
                var datasource = Enum.GetName(typeof(IConfigRepository.DATASOURCES), i) ?? throw new Exception("Enum error");
                var path = Path.Combine(RootDir, "datasources", datasource);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
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
                    string filePath;
                    if (config.Category == "datasources")
                    {
                        filePath = Path.Combine(RootDir, config.Category, config.Type, config.ApiName + ".json");
                    } else
                    {
                        filePath = Path.Combine(RootDir, config.Category, config.ApiName + "-" + config.ApiVersion + ".yaml");
                    }

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
                if (category == "datasources")
                {
                    foreach (int i in Enum.GetValues(typeof(IConfigRepository.DATASOURCES)))
                    {
                        var type = Enum.GetName(typeof(IConfigRepository.DATASOURCES), i) ?? throw new Exception("Enum error");

                        var files = Directory.GetFiles(Path.Combine(RootDir, category, type), "*.json");

                        foreach (var file in files)
                        {
                            string data = await File.ReadAllTextAsync(file);
                            string fileName = Path.GetFileName(file);
                            string name = fileName.Split(".")[0];
                            configs.Add(new(category, name, "", type, data));
                        }

                    }
                    
                }
                else
                {
                    var files = Directory.GetFiles(Path.Combine(RootDir, category), "*.yaml");

                    foreach (var file in files)
                    {
                        string data = await File.ReadAllTextAsync(file);
                        string fileName = Path.GetFileName(file);
                        string[] splitFileName = fileName.Split("-");
                        string apiName = splitFileName[0];
                        string apiVersion = splitFileName[1].Split(".")[0];
                        configs.Add(new(category, apiName, apiVersion, "", data));
                    }
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
                    string filePath;
                    if (config.Category == "datasources")
                    {
                        filePath = Path.Combine(RootDir, config.Category, config.Type, config.ApiName + ".json");
                    }
                    else
                    {
                        filePath = Path.Combine(RootDir, config.Category, config.ApiName + "-" + config.ApiVersion + ".yaml");
                    }

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
