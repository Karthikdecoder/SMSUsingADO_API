using System.ComponentModel.DataAnnotations;

namespace sms.Models
{
    public class StudentGetDTO
    {
        [Required]
        public string StudentId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string RollNo { get; set; }

        [Required]
        public string ClassName { get; set; }

        [Required]
        public string DateOfBirth { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string ContactNo { get; set; }

        [Required]
        public string CreatedOn { get; set; } = null;
        [Required]
        public string CreatedBy { get; set; } = null;
        [Required]
        public string UpdatedOn { get; set; } = null;
        [Required]
        public string UpdatedBy { get; set; } = null;

        public bool IsDeleted;
    }
}
