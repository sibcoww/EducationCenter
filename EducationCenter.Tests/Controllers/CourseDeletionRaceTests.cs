using EducationCenter.Api.Controllers;
using EducationCenter.Api.Data;
using EducationCenter.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;

namespace EducationCenter.Tests.Controllers;

public class CourseDeletionRaceTests
{
    [Theory]
    [InlineData(PostgresErrorCodes.ForeignKeyViolation, "FK_Groups_Courses_CourseId", true)]
    [InlineData(PostgresErrorCodes.ForeignKeyViolation, "OtherConstraint", false)]
    [InlineData(PostgresErrorCodes.UniqueViolation, "FK_Groups_Courses_CourseId", false)]
    public async Task Delete_handles_only_the_course_group_constraint(
        string sqlState, string constraint, bool expectedConflict)
    {
        var interceptor = new DeleteFailureInterceptor(sqlState, constraint);
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .AddInterceptors(interceptor)
            .Options;
        using var db = new AppDbContext(options);
        db.Courses.Add(new Course { Id = 1, Title = "Course" });
        await db.SaveChangesAsync();

        var controller = new CoursesController(db);
        if (expectedConflict)
        {
            Assert.IsType<ConflictObjectResult>(await controller.Delete(1));
        }
        else
        {
            await Assert.ThrowsAsync<DbUpdateException>(() => controller.Delete(1));
        }

        // A separate context verifies persisted state after the failed save.
        using var verification = new AppDbContext(options);
        Assert.True(await verification.Courses.AnyAsync(c => c.Id == 1));
    }

    private sealed class DeleteFailureInterceptor(string sqlState, string constraint) : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Context!.ChangeTracker.Entries<Course>()
                .Any(entry => entry.State == EntityState.Deleted))
            {
                throw new DbUpdateException("Concurrent reference prevents deletion.",
                    new PostgresException("Constraint violation", "ERROR", "ERROR", sqlState,
                        constraintName: constraint));
            }
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
