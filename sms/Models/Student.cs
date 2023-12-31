﻿using System.ComponentModel.DataAnnotations;

namespace sms.Models
{
    public class Student
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string RollNo { get; set; }

        [Required]
        public string ClassId { get; set; }

        [Required]
        public string DateOfBirth { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string ContactNo { get; set; }

        public bool IsDeleted;
    }
}
