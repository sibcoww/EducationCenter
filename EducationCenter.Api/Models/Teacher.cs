namespace EducationCenter.Api.Models
{
    public class Teacher
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateOnly BirthDate { get; set; }

        public List<Course> Courses { get; set; }

        public Group Group { get; set; }
    }
}
