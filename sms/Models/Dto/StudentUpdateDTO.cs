using System.ComponentModel.DataAnnotations;

namespace sms.Models.Dto
{
    public class StudentUpdateDTO
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public string DateOfBirth { get; set; }

        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Contact Number")]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Please enter a valid phone number.")]
        public string ContactNo { get; set; }

    }
}
