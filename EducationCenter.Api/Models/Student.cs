namespace EducationCenter.Api.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateOnly BirthDate { get; set; }

        public List<Course> Courses { get; set; }

        public Group Group { get; set; }
    }
}
