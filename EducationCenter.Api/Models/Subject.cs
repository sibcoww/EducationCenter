namespace EducationCenter.Api.Models
{
    public class Subject
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; } = null;
        public List<Teacher> Teachers { get; set; } = [];
        public List<Course> Courses { get; set; } = [];
    }
}
