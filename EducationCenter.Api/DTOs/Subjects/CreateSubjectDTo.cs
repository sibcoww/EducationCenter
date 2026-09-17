using System.ComponentModel.DataAnnotations;
namespace EducationCenter.Api.DTOs.Subjects
{
    public class CreateSubjectDTo
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
