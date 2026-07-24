namespace EducationCenter.Api.DTOs.Courses
{
    public class CreateCourseDTo
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
    }
}