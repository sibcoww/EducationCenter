using EducationCenter.Api.Data;
using EducationCenter.Api.DTOs.Subjects;
using EducationCenter.Api.DTOs.Teachers;
using EducationCenter.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EducationCenter.Api.Controllers;

[ApiController]
[Route("api/teachers")]
public class TeachersController : ControllerBase
{
    private readonly AppDbContext _context;

    public TeachersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<TeacherDTo>> Create(CreateTeacherDTo dto)
    {
        var teacher = new Teacher
        {
            Name = dto.Name,
            BirthDate = dto.BirthDate
        };
        _context.Teachers.Add(teacher);
        await _context.SaveChangesAsync();
        var result = new TeacherDTo
        {
            Name = teacher.Name,
            BirthDate = teacher.BirthDate,
            SubjectTitles = [],
            GroupNames = []
        };
        return CreatedAtAction(nameof(GetById), new { id = teacher.Id }, result);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<TeacherDTo>> GetById(int id)
    {
        var teacher = await _context.Teachers
            .Include(t => t.Subjects)
            .Include(t => t.Groups)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (teacher == null)
        {
            return NotFound();
        }

        return Ok(new TeacherDTo
        {
            Id = teacher.Id,
            Name = teacher.Name,
            BirthDate = teacher.BirthDate,
            SubjectTitles = teacher.Subjects.Select(s => s.Title).ToList(),
            GroupNames = teacher.Groups.Select(g => g.Name).ToList()
        });
    }

    [HttpGet]
    public async Task<ActionResult<List<TeacherDTo>>> Get()
    {
        var teachers = await _context.Teachers
            .Include(t => t.Subjects)
            .Include(t => t.Groups)
            .ToListAsync();
            
        var result = teachers.Select(teacher => new TeacherDTo
        {
            Id = teacher.Id,
            Name = teacher.Name,
            BirthDate = teacher.BirthDate,
            SubjectTitles = teacher.Subjects.Select(s => s.Title).ToList(),
            GroupNames = teacher.Groups.Select(g => g.Name).ToList()
        }).ToList();
        
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TeacherDTo>> Update(int id, CreateTeacherDTo dto)
    {
        var teacher = await _context.Teachers.FindAsync(id);
        if (teacher == null)
        {
            return NotFound();
        }

        teacher.Name = dto.Name;
        teacher.BirthDate = dto.BirthDate;
        
        await _context.SaveChangesAsync();

        return Ok(new TeacherDTo
        {
            Id = teacher.Id,
            Name = teacher.Name,
            BirthDate = teacher.BirthDate,
            // Subjects and Groups don't need to be updated in this endpoint
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var teacher = await _context.Teachers.FindAsync(id);
        if (teacher == null)
        {
            return NotFound();
        }

        _context.Teachers.Remove(teacher);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
    
    [HttpPost("{teacherId}/subjects/{subjectId}")]
    public async Task<IActionResult> AddSubjectToTeacher(int teacherId, int subjectId)
    {
        var teacher = await _context.Teachers
            .Include(t => t.Subjects)
            .FirstOrDefaultAsync(t => t.Id == teacherId);
            
        if (teacher == null) return NotFound();
        
        var subject = await _context.Subjects.FindAsync(subjectId);
        if (subject == null) return NotFound();
        
        if (teacher.Subjects.Any(s => s.Id == subjectId))
        {
            return Conflict();
        }
        
        teacher.Subjects.Add(subject);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
    
    [HttpDelete("{teacherId}/subjects/{subjectId}")]
    public async Task<IActionResult> RemoveSubjectFromTeacher(int teacherId, int subjectId)
    {
        var teacher = await _context.Teachers
            .Include(t => t.Subjects)
            .FirstOrDefaultAsync(t => t.Id == teacherId);
            
        if (teacher == null) return NotFound();
        
        var subject = teacher.Subjects.FirstOrDefault(s => s.Id == subjectId);
        if (subject == null) return NotFound();
        
        teacher.Subjects.Remove(subject);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
}
