using sms.Models;
using sms.Models.Dto;

namespace sms.Repository.IRepository
{
    public interface IRoleMasterRepository
    {
        Task<List<RoleMasterDTO>> GetAll();
        Task<RoleMasterDTO> Get(int roleId);
        Task<RoleMasterCUDTO> Create(RoleMasterCUDTO roleMaster, string userName);
        Task Remove(int roleId, string userName);
        Task Update(int roleId, RoleMasterCUDTO roleMaster, string userName);
        public bool CreateCheck(RoleMasterCUDTO roleMaster);
        public bool RemoveCheck(int roleId);
        public bool UpdateCheck(int roleId);
    }
}
