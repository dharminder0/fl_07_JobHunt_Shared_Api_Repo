namespace VendersCloud.Common.Caching
{
    public abstract class CacheObject {
        public DateTime ExpireDate { get; set; }
    }
    public class CacheObject<T> : CacheObject {
        public CacheObject() {

        }
        public CacheObject(T data, DateTime expire) {
            Data = data;
            ExpireDate = expire;
        }
        public T Data { get; set; }
    }
}
