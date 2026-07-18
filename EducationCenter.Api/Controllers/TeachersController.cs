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
        var teacher = await _context.Teachers.FindAsync(id);
        if (teacher == null)
        {
            return NotFound();
        }
        var result = new TeacherDTo
        {
            Id = teacher.Id,
            Name = teacher.Name,
            BirthDate = teacher.BirthDate,
            SubjectTitles = teacher.Subjects.Select(s => s.Title).ToList(),
            GroupNames = teacher.Groups.Select(g => g.Name).ToList()
        };
        return Ok(result);
    }
    [HttpGet]
    public async Task<ActionResult<List<TeacherDTo>>> Get()
    {
        var teachers = await _context.Teachers.Include(t => t.Subjects).Include(t => t.Groups).ToListAsync();
        var result = teachers.Select(t => new TeacherDTo
        {
            Id = t.Id,
            Name = t.Name,
            BirthDate = t.BirthDate,
            SubjectTitles = t.Subjects.Select(s => s.Title).ToList(),
            GroupNames = t.Groups.Select(g => g.Name).ToList()

        }).ToList();
        return Ok(result);
    }
    [HttpPut("{id}")]
    public async Task<ActionResult<TeacherDTo>> Update(int id, UpdateTeacherDTo dto)
    {
        var teacher = await _context.Teachers.Include(t => t.Subjects).Include(t => t.Groups).FirstOrDefaultAsync(t => t.Id == id);
        if (teacher == null)
        {
            return NotFound();
        }
        teacher.Name = dto.Name;
        teacher.BirthDate = dto.BirthDate;
        await _context.SaveChangesAsync();
        var result = new TeacherDTo
        {
            Id = teacher.Id,
            Name = teacher.Name,
            BirthDate = teacher.BirthDate,
            SubjectTitles = teacher.Subjects.Select(s => s.Title).ToList(),
            GroupNames = teacher.Groups.Select(g => g.Name).ToList()
        };
        return Ok(result);
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
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
    [HttpPost("{teacherId}/add-subject/{subjectId}")]
    public async Task<ActionResult> AddSubject(int teacherId, int subjectId)
    {
        var teacher = await _context.Teachers.Include(t => t.Subjects).FirstOrDefaultAsync(t => t.Id == teacherId);
        if (teacher == null)
        {
            return NotFound();
        }
        var subject = await _context.Subjects.FindAsync(subjectId);
        if (subject == null)
        {
            return NotFound();
        }
        teacher.Subjects.Add(subject);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    [HttpPost("{teacherId}/add-group/{groupId}")]
    public async Task<ActionResult> AddGroup(int teacherId, int groupId)
    {
        var teacher = await _context.Teachers.Include(t => t.Groups).FirstOrDefaultAsync(t => t.Id == teacherId);
        if (teacher == null)
        {
            return NotFound();
        }
        var group = await _context.Groups.FindAsync(groupId);
        if (group == null)
        {
            return NotFound();
        }
        teacher.Groups.Add(group);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    [HttpDelete("{teacherId}/remove-subject/{subjectId}")]
    public async Task<ActionResult> RemoveSubject(int teacherId, int subjectId)
    {
        var teacher = await _context.Teachers.Include(t => t.Subjects).FirstOrDefaultAsync(t => t.Id == teacherId);
        if (teacher == null)
        {
            return NotFound();
        }
        var subject = await _context.Subjects.FindAsync(subjectId);
        if (subject == null)
        {
            return NotFound();
        }
        teacher.Subjects.Remove(subject);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    [HttpDelete("{teacherId}/remove-group/{groupId}")]
    public async Task<ActionResult> RemoveGroup(int teacherId, int groupId)
    {
        var teacher = await _context.Teachers.Include(t => t.Groups).FirstOrDefaultAsync(t => t.Id == teacherId);
        if (teacher == null)
        {
            return NotFound();
        }
        var group = await _context.Groups.FindAsync(groupId);
        if (group == null)
        {
            return NotFound();
        }
        teacher.Groups.Remove(group);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
