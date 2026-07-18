namespace EducationCenter.Api.DTOs.Teachers
{
    public class TeacherDTo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateOnly BirthDate { get; set; }
        public List<string> SubjectTitles { get; set; } = [];
        public List<string> GroupNames { get; set; } = [];
    }
}
