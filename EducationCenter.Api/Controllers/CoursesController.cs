using EducationCenter.Api.Data;
using EducationCenter.Api.DTOs.Courses;
using EducationCenter.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EducationCenter.Api.Controllers;

[ApiController]
[Route("api/courses")]
public class CoursesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CoursesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<CourseDTo>> Create(CreateCourseDTo dto)
    {
        var course = new Course
        {
            Title = dto.Title,
            Description = dto.Description,
            Price = dto.Price
        };

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        var result = new CourseDTo
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            Price = course.Price
        };

        return CreatedAtAction(nameof(GetById), new { id = course.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<List<CourseDTo>>> Get()
    {
        var courses = await _context.Courses
            .Include(c => c.Subjects)
            .Include(c => c.Groups)
            .ToListAsync();

        var result = courses.Select(c => new CourseDTo
        {
            Id = c.Id,
            Title = c.Title,
            Description = c.Description,
            Price = c.Price,
            SubjectTitles = c.Subjects.Select(s => s.Title).ToList(),
            GroupNames = c.Groups.Select(g => g.Name).ToList()
        }).ToList();

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CourseDTo>> GetById(int id)
    {
        var course = await _context.Courses
            .Include(c => c.Subjects)
            .Include(c => c.Groups)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null) return NotFound();

        return Ok(new CourseDTo
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            Price = course.Price,
            SubjectTitles = course.Subjects.Select(s => s.Title).ToList(),
            GroupNames = course.Groups.Select(g => g.Name).ToList()
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CourseDTo>> Update(int id, UpdateCourseDTo dto)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null) return NotFound();

        course.Title = dto.Title;
        course.Description = dto.Description;
        course.Price = dto.Price;

        await _context.SaveChangesAsync();

        return Ok(new CourseDTo
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            Price = course.Price
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null) return NotFound();

        if (await _context.Groups.AnyAsync(g => g.CourseId == id))
        {
            return Conflict($"Course with ID {id} is used by one or more groups.");
        }

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();

        return NoContent();
    }

}
