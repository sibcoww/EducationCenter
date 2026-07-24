namespace EducationCenter.Api.DTOs.Groups
{
    public class GroupDTo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public int? SubjectId { get; set; }
        public string? SubjectTitle { get; set; }
        public int? TeacherId { get; set; }
        public string? TeacherName { get; set; }
        public List<string> StudentNames { get; set; } = [];
    }
}