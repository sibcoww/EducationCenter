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
    public async Task<ActionResult<CreateTeacherDTo>> Create(CreateTeacherDTo dto)
    {
        var teacher = new Teacher
        {
            Name = dto.Name,
            BirthDate = dto.BirthDate
        };
        _context.Teachers.Add(teacher);
        await _context.SaveChangesAsync();
        var result = new CreateTeacherDTo
        {
            Name = teacher.Name,
            BirthDate = teacher.BirthDate
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
            BirthDate = teacher.BirthDate
        };
        return Ok(result);
    }
    [HttpGet]
    public async Task<ActionResult<TeacherDTo>> Get()
    {
        var teachers = await _context.Teachers.ToListAsync();
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
        var result = new TeacherDTo
        {
            Id = teacher.Id,
            Name = teacher.Name,
            BirthDate = teacher.BirthDate
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
    
}
