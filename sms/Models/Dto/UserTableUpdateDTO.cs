using System.ComponentModel.DataAnnotations;

namespace sms.Models.Dto
{
    public class UserTableUpdateDTO
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Name { get; set; }
        public string? RoleId { get; set; }
    }
}
