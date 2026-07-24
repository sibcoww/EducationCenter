namespace EducationCenter.Api.DTOs.Students
{
    public class StudentDTo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateOnly BirthDate { get; set; }
        public int? GroupId { get; set; }
        public string? GroupName { get; set; }
    }
}