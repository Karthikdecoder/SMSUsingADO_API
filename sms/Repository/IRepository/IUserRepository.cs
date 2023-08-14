using sms.Models;
using sms.Models.Dto;

namespace sms.Repository.IRepository
{
    public interface IUserRepository
    {
        Task<List<UserTableDTO>> GetAll();
        Task<UserTableDTO> Get(int ApplicationUserId);
        Task Remove(int ApplicationUserId, string userName);
        Task Update(int ApplicationUserId, UserTableUpdateDTO userTableUpdateDTO, string userName);
        public bool RemoveCheck(int ApplicationUserId);
        public bool UpdateCheck(int ApplicationUserId);
        bool IsUniqueUser(String username);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<UserDTO> Register(RegisterationRequestDTO registerationRequestDTO, string userName);
    }
}
