using VendersCloud.Business.Entities.DataModels;
using VendersCloud.Business.Service.Abstract;
using VendersCloud.Data.Repositories.Abstract;
using static System.Net.Mime.MediaTypeNames;
using static VendersCloud.Data.Enum.Enum;

namespace VendersCloud.Business.Service.Concrete
{
    public class ListValuesService:IListValuesService
    {
        private readonly IListValuesRepository _listValuesRepository;
        public ListValuesService(IListValuesRepository listValuesRepository)
        {
            _listValuesRepository = listValuesRepository;
        }

        public async Task<IList<ListValues>> GetListValuesAsync()
        {
            try
            {
                var response = await _listValuesRepository.GetListValuesAsync();
                return response;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ListValues> GetListValuesByNameAsync(string Name)
        {
            try
            {
                if (string.IsNullOrEmpty(Name))
                {
                    throw new ArgumentNullException("Enter Valid Input!!");
                }
                var response = await _listValuesRepository.GetListValuesByNameAsync(Name);
                return response;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<ListValues>> GetListValuesByMasterListIdAsync(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentNullException("Enter Valid Input!!");
                }
                name = char.ToUpper(name[0]) + name.Substring(1).ToLower();
                if (Enum.TryParse(typeof(MasterListValues), name, out var enumValue) && enumValue != null)
                {
                    var response = await _listValuesRepository.GetListValuesByMasterListIdAsync((int)enumValue);
                    return response;
                }
                else
                {
                    throw new ArgumentException("Invalid enum name provided.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

    }
}
