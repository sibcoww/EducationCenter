namespace EducationCenter.Api.Models
{
    public class Teacher
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateOnly BirthDate { get; set; }
        public List<Subject> Subjects { get; set; } = [];
        public List<Group> Groups { get; set; } = [];
    }
}
