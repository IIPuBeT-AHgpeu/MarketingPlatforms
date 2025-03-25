using MarketingPlatforms.Business;

namespace MarketingPlatforms.Infrastructure
{
    public class ConfigurationRepository : IRepository<IEnumerable<string>>
    {
        public Dictionary<string, List<string>>? Config { get; private set; } = null;

        public ConfigurationRepository() 
        {
        }

        public bool TrySetValue(IEnumerable<string> data)
        {
            Config = new Dictionary<string, List<string>>();

            foreach (var row in data)
            {
                string[] keyValues = row.Split(":");
                if (keyValues.Length != 2) return false;

                var locations = keyValues[1].Split(",");
                if (locations.Length == 0) return false;

                foreach (var location in locations)
                {
                    if (Config.ContainsKey(location))
                    {
                        bool isExist = false;
                        foreach (var platform in Config[location])
                            if (platform == keyValues[0]) isExist = true;

                        if (!isExist) Config[location].Add(keyValues[0]);
                    }
                    else
                        Config.Add(location, new List<string>() { keyValues[0] });
                }
            }

            return true;
        }

        public IEnumerable<string>? GetDataByKey(string key)
        {
            if (Config == null) return null;

            List<string> result = new List<string>();

            string[] levels = key.Split("/");
            string partOfLocation = "";
            for (int i = 1; i < levels.Length; i++)
            {
                partOfLocation += "/" + levels[i];
                if (Config.ContainsKey(partOfLocation)) result.AddRange(Config[partOfLocation]);
            }

            return result;
        }
    }
}
