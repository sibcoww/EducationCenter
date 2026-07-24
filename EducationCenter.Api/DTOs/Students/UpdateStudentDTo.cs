namespace EducationCenter.Api.DTOs.Students
{
    public class UpdateStudentDTo
    {
        public string Name { get; set; } = string.Empty;
        public DateOnly BirthDate { get; set; }
        public int? GroupId { get; set; }
    }
}