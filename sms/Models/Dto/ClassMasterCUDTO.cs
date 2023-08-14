using System.ComponentModel.DataAnnotations;

namespace sms.Models
{
    public class ClassMasterCUDTO
    {
        [RegularExpression(@"^STD-(?:[1-9]|1[0-2])$", ErrorMessage = "Please enter right input")]
        public string ClassName { get; set; }
    }
}
