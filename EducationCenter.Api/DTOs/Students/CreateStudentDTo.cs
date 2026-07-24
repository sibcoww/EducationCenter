namespace EducationCenter.Api.DTOs.Students
{
    public class CreateStudentDTo
    {
        public string Name { get; set; } = string.Empty;
        public DateOnly BirthDate { get; set; }
        public int? GroupId { get; set; }
    }
}