using Microsoft.Extensions.Configuration;

namespace VendersCloud.Common.Caching
{
    public static class AppLocalCache {
        private static Dictionary<string, CacheObject> _cache = new Dictionary<string, Caching.CacheObject>();
        private static bool _isCacheEnabled = false;
        private static int _defaultCacheHours = 5;

        private static IConfiguration _configuration;

        public static void UseConfiguration(IConfiguration configuration) {
            _configuration = configuration;
            _isCacheEnabled = !string.IsNullOrWhiteSpace(_configuration["AppLocalCacheEnabled"]) ? bool.Parse(_configuration["AppLocalCacheEnabled"]) : false;
            _defaultCacheHours = !string.IsNullOrWhiteSpace(_configuration["DefaultAppLocalCacheHours"]) ? int.Parse(_configuration["DefaultAppLocalCacheHours"]) : 5;
        }
        public static void Add(string key, CacheObject obj) {
            lock (_cache){
                if (_cache.ContainsKey(key)) {
                    _cache.Remove(key);
                }
                _cache.Add(key, obj);
            }           
        }

        public static void Add<T>(string key, CacheObject<T> obj) {
            if (!_isCacheEnabled) return;
            lock (_cache) {
                if (_cache.ContainsKey(key)) {
                    _cache.Remove(key);
                }
                _cache.Add(key, obj);
            }
        }

        public static CacheObject<T> Get<T>(string key) {
            if (!_isCacheEnabled) return null;
            if (!_cache.ContainsKey(key))
                return null;
            if (_cache[key].ExpireDate < DateTime.Now) {
                Remove(key);
                return null;
            }            
            return (CacheObject<T>)_cache[key];
        }

        public static CacheObject Get(string key) {
            if (!_isCacheEnabled) return null;
            if (!_cache.ContainsKey(key))
                return null;
            if (_cache[key].ExpireDate < DateTime.Now) {
                Remove(key);
                return null;
            }
            return _cache[key];
        }

        public static void Remove(string key) {
            if (!_isCacheEnabled) return;
            lock (_cache) {
                if(_cache.ContainsKey(key))
                    _cache.Remove(key);
            }
        }

        public static void Remove(IEnumerable<string> keys) {
            if (!_isCacheEnabled) return;
            lock (_cache) {
                foreach (var key in keys) {
                    if (_cache.ContainsKey(key))
                        _cache.Remove(key);
                }                
            }
        }

        public static void Clear() {
            if (!_isCacheEnabled) return;
            lock (_cache) {                
                _cache.Clear();
            }
        }

        public static IEnumerable<string> GetAllKeys() {
            return _cache.Keys;
        }

        public static bool ContainsKey(string key) {
            return _cache.ContainsKey(key);
        }

        public static T GetOrCache<T>(string key, Func<T> f) {
            return GetOrCache(key, _defaultCacheHours, f);
        }

        public static T GetOrCache<T>(string key, int cacheHours, Func<T> f) {
            var result = Get<T>(key);
            if(result == null) {
                var data = f();                
                Add<T>(key, new CacheObject<T> { ExpireDate = DateTime.UtcNow.AddHours(cacheHours), Data = data });
                return data;
            }
            return result.Data;
        }

        public static Dictionary<string, CacheObject> GetAllCahedObjects() {
            return _cache;
        }
    }
}
