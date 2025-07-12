namespace VendersCloud.Business.Service.Concrete
{
    public class MasterListService :IMasterListService
    {
        private readonly IMasterListRepository _masterListRepository;
        public MasterListService(IMasterListRepository masterListRepository)
        {
            _masterListRepository= masterListRepository;
        }

        public async Task<List<MasterList>> GetMasterListAsync()
        {
            try
            {
                var response = await _masterListRepository.GetMasterListAsync();
                return response;
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        public async Task<bool> AddBulkMasterListAsync(List<string> names)
        {
            try
            {
                if (string.IsNullOrEmpty(names[0]))
                {
                    return false;
                }
                var response = await _masterListRepository.AddBulkMasterListAsync(names);
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<MasterList> GetMasterListByIdAndNameAsync(string name)
        {
            try
            {
                if(string.IsNullOrEmpty(name))
                {
                    throw new ArgumentException("Please provide valid input!!");
                }
                var response = await _masterListRepository.GetMasterListByIdAndNameAsync(name);
                return response;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
