using EducationCenter.Api.Data;
using EducationCenter.Api.DTOs.Subjects;
using EducationCenter.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;    

namespace EducationCenter.Api.Controllers;

[ApiController]
[Route("api/subjects")]
public class SubjectsController : ControllerBase
{
    private readonly AppDbContext _context;

    public SubjectsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<SubjectDTo>> Create(CreateSubjectDTo dto)
    {
        var subject = new Subject
        {
            Title = dto.Title,
            Description = dto.Description
        };
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        var result = new SubjectDTo
        {
            Id = subject.Id,
            Title = subject.Title,
            Description = subject.Description
        };
        return CreatedAtAction(nameof(GetById), new { id = subject.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<List<SubjectDTo>>> Get()
    {
        var subjects = await _context.Subjects.ToListAsync();
        var result = subjects.Select(s => new SubjectDTo
        {
            Id = s.Id,
            Title = s.Title,
            Description = s.Description
        }).ToList();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SubjectDTo>> GetById([FromRoute] int id)
    {
        var subject = await _context.Subjects.FindAsync(id);
        if (subject == null)
        {
            return NotFound();
        }
        var result = new SubjectDTo
        {
            Id = subject.Id,
            Title = subject.Title,
            Description = subject.Description,
            TeacherNames = subject.Teachers.Select(t => t.Name).ToList(),
            CourseNames = subject.Courses.Select(c => c.Title).ToList()
        }; return Ok(result);
    }
    [HttpPut("{id}")]
    public async Task<ActionResult<SubjectDTo>> Update(int id, UpdateSubjectDTo dto)
    {
        var subject = await _context.Subjects.FindAsync(id);
        if (subject == null)
        {
            return NotFound();
        }
        subject.Title = dto.Title;
        subject.Description = dto.Description;
        await _context.SaveChangesAsync();
        var result = new SubjectDTo
        {
            Id = subject.Id,
            Title = subject.Title,
            Description = subject.Description
        };
        return Ok(result);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var subject = await _context.Subjects.FindAsync(id);
        if (subject == null)
        {
            return NotFound();
        }
        _context.Subjects.Remove(subject);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{SubjectId}/add-teacher/{teacherId}")]
    public async Task<IActionResult> AddTeacherToSubject(int SubjectId, int teacherId)
    {
        var subject = await _context.Subjects
            .Include(s => s.Teachers)
            .FirstOrDefaultAsync(s => s.Id == SubjectId);
        if (subject == null)
        {
            return NotFound($"Subject with ID {SubjectId} not found.");
        }
        var teacher = await _context.Teachers.FindAsync(teacherId);
        if (teacher == null)
        {
            return NotFound($"Teacher with ID {teacherId} not found.");
        }
        if (subject.Teachers.Any(t => t.Id == teacherId))
        {
            return BadRequest($"Teacher with ID {teacherId} is already assigned to the subject.");
        }
        subject.Teachers.Add(teacher);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{SubjectId}/add-course/{courseId}")]
    public async Task<IActionResult> AddCourseToSubject(int SubjectId, int courseId)
    {
        var subject = await _context.Subjects
            .Include(s => s.Courses)
            .FirstOrDefaultAsync(s => s.Id == SubjectId);
        if (subject == null)
        {
            return NotFound($"Subject with ID {SubjectId} not found.");
        }
        var course = await _context.Courses.FindAsync(courseId);
        if (course == null)
        {
            return NotFound($"Course with ID {courseId} not found.");
        }
        if (subject.Courses.Any(c => c.Id == courseId))
        {
            return BadRequest($"Course with ID {courseId} is already assigned to the subject.");
        }
        subject.Courses.Add(course);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

