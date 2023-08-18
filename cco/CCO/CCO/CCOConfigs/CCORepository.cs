namespace CCO.CCOConfigs
{
    public class CCORepository
    {
        private readonly Dictionary<CCOIdentifier, SortedSet<CCOConfig>> _configs = new();

        private static readonly TimeSpan VALIDITY_MIN_OFFSET = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan PRUNE_AFTER = TimeSpan.FromMinutes(1);

        private readonly object _pruningLock = new();

        private class CCOConfigValidityStartComparer : IComparer<CCOConfig?>
        {
            public int Compare(CCOConfig? x, CCOConfig? y)
            {
                if (ReferenceEquals(x, y)) return 0;
                if (ReferenceEquals(null, y)) return 1;
                if (ReferenceEquals(null, x)) return -1;
                return -x.ValidFrom.CompareTo(y.ValidFrom);
            }
        }

        public void AddConfig(CCOSpec spec)
        {
            var now = DateTime.Now;
            CheckStartDateValidity(spec.ValidFrom, now);

            var parsedConfig = new CCOConfig(spec.ValidFrom, spec.Spec);
            var id = parsedConfig.Id;
            if (!_configs.ContainsKey(id))
            {
                _configs.Add(id, new SortedSet<CCOConfig>(new CCOConfigValidityStartComparer()));
            }

            _configs[id].Add(parsedConfig);
            Prune(now);
        }

        public bool HasConfig(CCOIdentifier id)
        {
            return _configs.ContainsKey(id);
        }

        public IEnumerable<CCOIdentifier> GetAllConfigs(DateTime now)
        {
            return _configs.Select(kv => GetCurrentConfig(kv.Key, now))
                .Where(kv => kv != null)
                .Select(config => config!.Id);
        }

        public CCOConfig? GetCurrentConfig(CCOIdentifier id, DateTime now)
        {
            if (!_configs.ContainsKey(id)) return null;
            var eligibleConfigs = _configs[id];
            try
            {
                return eligibleConfigs.First(config => config.IsActive(now));
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public bool DeleteConfig(CCOIdentifier id)
        {
            return _configs.Remove(id);
        }

        public int RevertPendingChanges(DateTime now)
        {
            lock (_pruningLock)
            {
                var removedConfigs = 0;
                foreach (var (key, value) in _configs)
                {
                    removedConfigs += value.RemoveWhere(apiConfig => apiConfig.ValidFrom >= now);
                }

                return removedConfigs;
            }
        }

        private void Prune(DateTime now)
        {
            lock (_pruningLock)
            {
                foreach (var (key, value) in _configs)
                {
                    var configs = new SortedSet<CCOConfig>(new CCOConfigValidityStartComparer());
                    CCOConfig? newest = null;
                    foreach (var config in value)
                    {
                        if (newest != null)
                        {
                            newest = config;
                            configs.Add(newest);
                        }

                        configs.Add(config);
                        if (config.ValidFrom + PRUNE_AFTER < now)
                        {
                            break;
                        }
                    }

                    _configs[key] = configs;
                }
            }
        }

        private static void CheckStartDateValidity(DateTime validFrom, DateTime now)
        {
            if (now + VALIDITY_MIN_OFFSET > validFrom)
            {
                throw new Exception("CCO config validity is set to a too early time!");
            }
        }
    }
}
