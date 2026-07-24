using EducationCenter.Api.Data;
using EducationCenter.Api.DTOs.Students;
using EducationCenter.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EducationCenter.Api.Controllers;

[ApiController]
[Route("api/students")]
public class StudentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public StudentsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<StudentDTo>> Create(CreateStudentDTo dto)
    {
        var student = new Student
        {
            Name = dto.Name,
            BirthDate = dto.BirthDate,
            GroupId = dto.GroupId
        };

        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        student = await _context.Students
            .Include(s => s.Group)
            .FirstAsync(s => s.Id == student.Id);

        var result = new StudentDTo
        {
            Id = student.Id,
            Name = student.Name,
            BirthDate = student.BirthDate,
            GroupId = student.GroupId,
            GroupName = student.Group?.Name
        };

        return CreatedAtAction(nameof(GetById), new { id = student.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<List<StudentDTo>>> Get()
    {
        var students = await _context.Students
            .Include(s => s.Group)
            .ToListAsync();

        var result = students.Select(s => new StudentDTo
        {
            Id = s.Id,
            Name = s.Name,
            BirthDate = s.BirthDate,
            GroupId = s.GroupId,
            GroupName = s.Group?.Name
        }).ToList();

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StudentDTo>> GetById(int id)
    {
        var student = await _context.Students
            .Include(s => s.Group)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (student == null) return NotFound();

        return Ok(new StudentDTo
        {
            Id = student.Id,
            Name = student.Name,
            BirthDate = student.BirthDate,
            GroupId = student.GroupId,
            GroupName = student.Group?.Name
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<StudentDTo>> Update(int id, UpdateStudentDTo dto)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null) return NotFound();

        student.Name = dto.Name;
        student.BirthDate = dto.BirthDate;
        student.GroupId = dto.GroupId;

        await _context.SaveChangesAsync();

        student = await _context.Students
            .Include(s => s.Group)
            .FirstAsync(s => s.Id == student.Id);

        return Ok(new StudentDTo
        {
            Id = student.Id,
            Name = student.Name,
            BirthDate = student.BirthDate,
            GroupId = student.GroupId,
            GroupName = student.Group?.Name
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null) return NotFound();

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}