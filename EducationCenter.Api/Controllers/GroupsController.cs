using EducationCenter.Api.Data;
using EducationCenter.Api.DTOs.Groups;
using EducationCenter.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EducationCenter.Api.Controllers;

[ApiController]
[Route("api/groups")]
public class GroupsController : ControllerBase
{
    private readonly AppDbContext _context;

    public GroupsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<GroupDTo>> Create(CreateGroupDTo dto)
    {
        if (!await _context.Courses.AnyAsync(c => c.Id == dto.CourseId))
        {
            return NotFound($"Course with ID {dto.CourseId} not found.");
        }
        if (dto.SubjectId.HasValue && !await _context.Subjects.AnyAsync(s => s.Id == dto.SubjectId.Value))
        {
            return NotFound($"Subject with ID {dto.SubjectId.Value} not found.");
        }
        if (dto.TeacherId.HasValue && !await _context.Teachers.AnyAsync(t => t.Id == dto.TeacherId.Value))
        {
            return NotFound($"Teacher with ID {dto.TeacherId.Value} not found.");
        }

        var group = new Group
        {
            Name = dto.Name,
            CourseId = dto.CourseId,
            SubjectId = dto.SubjectId,
            TeacherId = dto.TeacherId
        };

        _context.Groups.Add(group);
        await _context.SaveChangesAsync();

        // Reload to get related entity names
        group = await _context.Groups
            .Include(g => g.Course)
            .Include(g => g.Subject)
            .Include(g => g.Teacher)
            .FirstAsync(g => g.Id == group.Id);

        var result = new GroupDTo
        {
            Id = group.Id,
            Name = group.Name,
            CourseId = group.CourseId,
            CourseTitle = group.Course?.Title ?? string.Empty,
            SubjectId = group.SubjectId,
            SubjectTitle = group.Subject?.Title,
            TeacherId = group.TeacherId,
            TeacherName = group.Teacher?.Name,
            StudentNames = []
        };

        return CreatedAtAction(nameof(GetById), new { id = group.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<List<GroupDTo>>> Get()
    {
        var groups = await _context.Groups
            .Include(g => g.Course)
            .Include(g => g.Subject)
            .Include(g => g.Teacher)
            .Include(g => g.Students)
            .ToListAsync();

        var result = groups.Select(g => new GroupDTo
        {
            Id = g.Id,
            Name = g.Name,
            CourseId = g.CourseId,
            CourseTitle = g.Course?.Title ?? string.Empty,
            SubjectId = g.SubjectId,
            SubjectTitle = g.Subject?.Title,
            TeacherId = g.TeacherId,
            TeacherName = g.Teacher?.Name,
            StudentNames = g.Students.Select(s => s.Name).ToList()
        }).ToList();

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GroupDTo>> GetById(int id)
    {
        var group = await _context.Groups
            .Include(g => g.Course)
            .Include(g => g.Subject)
            .Include(g => g.Teacher)
            .Include(g => g.Students)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (group == null) return NotFound();

        return Ok(new GroupDTo
        {
            Id = group.Id,
            Name = group.Name,
            CourseId = group.CourseId,
            CourseTitle = group.Course?.Title ?? string.Empty,
            SubjectId = group.SubjectId,
            SubjectTitle = group.Subject?.Title,
            TeacherId = group.TeacherId,
            TeacherName = group.Teacher?.Name,
            StudentNames = group.Students.Select(s => s.Name).ToList()
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<GroupDTo>> Update(int id, UpdateGroupDTo dto)
    {
        var group = await _context.Groups.FindAsync(id);
        if (group == null) return NotFound();

        if (!await _context.Courses.AnyAsync(c => c.Id == dto.CourseId))
        {
            return NotFound($"Course with ID {dto.CourseId} not found.");
        }
        if (dto.SubjectId.HasValue && !await _context.Subjects.AnyAsync(s => s.Id == dto.SubjectId.Value))
        {
            return NotFound($"Subject with ID {dto.SubjectId.Value} not found.");
        }
        if (dto.TeacherId.HasValue && !await _context.Teachers.AnyAsync(t => t.Id == dto.TeacherId.Value))
        {
            return NotFound($"Teacher with ID {dto.TeacherId.Value} not found.");
        }

        group.Name = dto.Name;
        group.CourseId = dto.CourseId;
        group.SubjectId = dto.SubjectId;
        group.TeacherId = dto.TeacherId;

        await _context.SaveChangesAsync();

        group = await _context.Groups
            .Include(g => g.Course)
            .Include(g => g.Subject)
            .Include(g => g.Teacher)
            .FirstAsync(g => g.Id == group.Id);

        return Ok(new GroupDTo
        {
            Id = group.Id,
            Name = group.Name,
            CourseId = group.CourseId,
            CourseTitle = group.Course?.Title ?? string.Empty,
            SubjectId = group.SubjectId,
            SubjectTitle = group.Subject?.Title,
            TeacherId = group.TeacherId,
            TeacherName = group.Teacher?.Name
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var group = await _context.Groups.FindAsync(id);
        if (group == null) return NotFound();

        if (await _context.Students.AnyAsync(s => s.GroupId == id))
        {
            return Conflict($"Group with ID {id} contains one or more students.");
        }

        _context.Groups.Remove(group);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
