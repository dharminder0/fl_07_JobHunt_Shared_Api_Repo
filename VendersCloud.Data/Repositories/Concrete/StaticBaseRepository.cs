namespace VendersCloud.Data.Repositories.Concrete
{
    public class StaticBaseRepository<T> :BaseRepository<T>,IBaseRepository<T> where T : class, IEntityKey, new()
    {
        public StaticBaseRepository(IConfiguration configuration) : base(configuration) { }
    }
    
}
