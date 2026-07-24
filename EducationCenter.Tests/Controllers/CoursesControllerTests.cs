using EducationCenter.Api.Controllers;
using EducationCenter.Api.Data;
using EducationCenter.Api.DTOs.Courses;
using EducationCenter.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace EducationCenter.Tests.Controllers
{
    public class CoursesControllerTests
    {
        private AppDbContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new AppDbContext(options);
            databaseContext.Database.EnsureCreated();
            return databaseContext;
        }

        [Fact]
        public async Task Create_ReturnsCreatedCourse()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new CoursesController(dbContext);
            var dto = new CreateCourseDTo { Title = "Programming", Price = 100 };

            // Act
            var actionResult = await controller.Create(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var resultValue = Assert.IsType<CourseDTo>(createdResult.Value);

            Assert.Equal(dto.Title, resultValue.Title);
            Assert.Equal(dto.Price, resultValue.Price);
            Assert.True(resultValue.Id > 0);
        }

        [Fact]
        public async Task Delete_DeletesCourse()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var course = new Course { Title = "C#", Price = 200 };
            dbContext.Courses.Add(course);
            await dbContext.SaveChangesAsync();

            var controller = new CoursesController(dbContext);

            // Act
            var actionResult = await controller.Delete(course.Id);

            // Assert
            Assert.IsType<NoContentResult>(actionResult);
            var dbCourse = await dbContext.Courses.FindAsync(course.Id);
            Assert.Null(dbCourse);
        }

        [Fact]
        public async Task Delete_ReturnsConflict_WhenCourseHasGroups()
        {
            var dbContext = GetDatabaseContext();
            var course = new Course { Title = "C#" };
            dbContext.Courses.Add(course);
            await dbContext.SaveChangesAsync();
            dbContext.Groups.Add(new Group { Name = "Group A", CourseId = course.Id });
            await dbContext.SaveChangesAsync();

            var result = await new CoursesController(dbContext).Delete(course.Id);

            Assert.IsType<ConflictObjectResult>(result);
            Assert.NotNull(await dbContext.Courses.FindAsync(course.Id));
        }
    }
}
