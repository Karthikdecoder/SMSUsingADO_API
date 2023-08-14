using System.ComponentModel.DataAnnotations;

namespace sms.Models.Dto
{
    public class UserTableDTO
    {
        public string? ApplicationUserId { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Name { get; set; }
        public string? RoleId { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedOn { get; set; }
        public string? UpdatedBy { get; set; }
        public string? UpdatedOn { get; set; }
    }
}
