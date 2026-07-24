namespace EducationCenter.Api.DTOs.Groups
{
    public class CreateGroupDTo
    {
        public string Name { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public int? SubjectId { get; set; }
        public int? TeacherId { get; set; }
    }
}