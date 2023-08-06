using System.ComponentModel.DataAnnotations;

namespace sms.Models.Dto
{
    public class StudentDTO
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Roll Number")]
        public int RollNo { get; set; }

        [Required]
        [Display(Name = "Class")]
        [Range(1, 12, ErrorMessage = "Please select a valid class between 1 and 12")]
        public int Class { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date, ErrorMessage = "Please enter a date of birth")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public string? DateOfBirth { get; set; }

        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Contact Number")]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Please enter a contact number")]
        public string ContactNo { get; set; }

    }
}
