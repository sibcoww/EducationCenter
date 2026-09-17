using System.ComponentModel.DataAnnotations;
namespace EducationCenter.Api.DTOs.Courses
{
    public class UpdateCourseDTo
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Range(typeof(decimal), "0", "79228162514264337593543950335", ParseLimitsInInvariantCulture = true)]
        public decimal Price { get; set; }
    }
}