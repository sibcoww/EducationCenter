namespace EducationCenter.Api.DTOs.Teachers
{
    public class UpdateTeacherDTo
    {
        public string Name { get; set; } = string.Empty;
        public DateOnly BirthDate { get; set; }
    }
}
