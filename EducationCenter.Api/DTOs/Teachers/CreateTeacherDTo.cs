namespace EducationCenter.Api.DTOs.Teachers
{
    public class CreateTeacherDTo
    {
        public string Name { get; set; } = string.Empty;
        public DateOnly BirthDate { get; set; }

    }
}
