using EducationCenter.Api.Validation;
using System.ComponentModel.DataAnnotations;
namespace EducationCenter.Api.DTOs.Students
{
    public class CreateStudentDTo
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [BirthDate]
        public DateOnly BirthDate { get; set; }
        public int? GroupId { get; set; }
    }
}