using Configurator.Entities;

namespace Configurator.Repositories
{
    public class DirectoryConfigRepository : IConfigRepository
    {
        const string ROOT_DIR = "local\\configs";
 
        public DirectoryConfigRepository()
        {
            if (!Directory.Exists(ROOT_DIR)) {
                Directory.CreateDirectory(ROOT_DIR);
            }
            foreach (int i in Enum.GetValues(typeof(IConfigRepository.CATEGORIES)))
            {
                var category = Enum.GetName(typeof(IConfigRepository.CATEGORIES), i);
                if (!Directory.Exists(ROOT_DIR + "/" + category)) {
                    Directory.CreateDirectory(ROOT_DIR + "/" + category);
                }
            }
        }

        public Task DeleteConfigs(IEnumerable<Config> configs)
        {
            try
            {
                foreach (var config in configs)
                {
                    string filePath = ROOT_DIR + "\\" + config.Category + "\\" + config.Name;

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

        public async Task<IEnumerable<Config>> GetAllConfigs()
        {
            var configs = new List<Config>(); 
            try
            {
                foreach (int i in Enum.GetValues(typeof(IConfigRepository.CATEGORIES)))
                {
                    var category = Enum.GetName(typeof(IConfigRepository.CATEGORIES), i);

                    var files = Directory.GetFiles(ROOT_DIR + "\\" + category, "*.yaml");

                    foreach (var file in files) {
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

        public async Task ModifyConfigs(IEnumerable<Config> configs)
        {
            try
            {
                foreach (var config in configs)
                {
                    string filePath = ROOT_DIR + "\\" + config.Category + "\\" + config.Name;

                    await File.WriteAllTextAsync(filePath, config.Data);
                }
            } catch (Exception e) 
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
