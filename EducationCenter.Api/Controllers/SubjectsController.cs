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
    public async Task<ActionResult<SubjectDTo>> GetById(int id)
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
            Description = subject.Description
        };
        return Ok(result);
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
}
