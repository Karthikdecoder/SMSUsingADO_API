using sms.Models;
using sms.Models.Dto;

namespace sms.Repository.IRepository
{
    public interface IStudentRepository
    {
        Task<List<StudentGetDTO>> GetAll();
        Task<StudentGetDTO> Get(int StudentId);
        Task<Student> Create(Student student, string userName);
        Task Remove(int StudentId, string userName);
        Task Update(int StudentId, StudentDTO student, string userName);
        public bool CreateCheck(Student student); // inversion of control, seperation of concern
        public bool RemoveCheck(int StudentId);
        public bool UpdateCheck(int StudentID);
        public bool RollNoCheck(int RollNo, int StudentId);
    }
}


