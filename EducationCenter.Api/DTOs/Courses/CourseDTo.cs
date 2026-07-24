namespace EducationCenter.Api.DTOs.Courses
{
    public class CourseDTo
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public List<string> SubjectTitles { get; set; } = [];
        public List<string> GroupNames { get; set; } = [];
    }
}