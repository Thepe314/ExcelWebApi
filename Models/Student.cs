
using System.ComponentModel.DataAnnotations;

namespace ExcelUploadApi.Models
{
    public class Student
    {
        
        [Key]
        public int Id{get;set;}

        [Required]
        [EmailAddress]
        public required string Email{get;set;}

        [Required]
        [MinLength(6)]
        public required string Password{get;set;}

        






    }
}