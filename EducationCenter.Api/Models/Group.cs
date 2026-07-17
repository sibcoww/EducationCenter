namespace EducationCenter.Api.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public Course? Course { get; set; } = null!;
        public int? SubjectId { get; set; }
        public Subject? Subject { get; set; }
        public int? TeacherId { get; set; }
        public Teacher? Teacher { get; set; }
        public List<Student> Students { get; set; } = [];
    }
}
