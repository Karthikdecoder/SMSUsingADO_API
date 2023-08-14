using sms.Models.Dto;
using sms.Models;

namespace sms.Repository.IRepository
{
    public interface IClassMasterRepository
    {
        Task<List<ClassMasterDTO>> GetAll();
        Task<ClassMasterDTO> Get(int classId);
        Task<ClassMasterCUDTO> Create(ClassMasterCUDTO classMaster, string userName);
        Task Remove(int classId, string userName);
        Task Update(int classId, ClassMasterCUDTO classMaster, string userName);
        public bool CreateCheck(ClassMasterCUDTO classMaster);
        public bool RemoveCheck(int classId);
        public bool UpdateCheck(int classId, string className);
    }
}
