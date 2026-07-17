namespace EducationCenter.Api.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public List<Subject> Subjects { get; set; } = [];
        public List<Group> Groups { get; set; } = [];
    }
}
