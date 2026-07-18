namespace EducationCenter.Api.DTOs.Subjects
{
    public class SubjectDTo
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }
        public List<string> TeacherNames { get; set; } = new List<string>();
        public List<string> CourseNames { get; set; } = new List<string>();
    }
}
