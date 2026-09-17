using EducationCenter.Api.Validation;
using System.ComponentModel.DataAnnotations;
namespace EducationCenter.Api.DTOs.Teachers
{
    public class CreateTeacherDTo
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [BirthDate]
        public DateOnly BirthDate { get; set; }

    }
}
