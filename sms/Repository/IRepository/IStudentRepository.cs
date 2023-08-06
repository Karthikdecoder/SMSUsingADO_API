using sms.Models;
using sms.Models.Dto;

namespace sms.Repository.IRepository
{
    public interface IStudentRepository
    {
        Task<List<Student>> GetAll();
        Task<Student> Get(int RollNo, int Class);
        Task<Student> Create(Student student);
        Task Remove(int RollNo, int Class);
        Task Update(int RollNo, int Class, Student student);
        public bool CreateCheck(Student student); // inversion of control, seperation of concern
        public bool UpdateCheck(int RollNo, int Class);
        public bool RemoveCheck(int RollNo, int Class);
    }
}


