using EducationCenter.Api.Controllers;
using EducationCenter.Api.Data;
using EducationCenter.Api.DTOs.Groups;
using EducationCenter.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace EducationCenter.Tests.Controllers
{
    public class GroupsControllerTests
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
        public async Task Create_ReturnsCreatedGroup()
        {
            var db = GetDatabaseContext();
            var course = new Course { Title = "Test Course" };
            db.Courses.Add(course);
            await db.SaveChangesAsync();

            var controller = new GroupsController(db);
            var dto = new CreateGroupDTo { Name = "Group A", CourseId = course.Id };

            var result = await controller.Create(dto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<GroupDTo>(createdResult.Value);
            Assert.Equal(dto.Name, returnValue.Name);
        }

        [Fact]
        public async Task GetById_ReturnsGroup_WhenExists()
        {
            var db = GetDatabaseContext();
            var course = new Course { Title = "Test Course" };
            db.Courses.Add(course);

            var group = new Group { Name = "Group A", Course = course };
            db.Groups.Add(group);
            await db.SaveChangesAsync();

            var controller = new GroupsController(db);

            var result = await controller.GetById(group.Id);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<GroupDTo>(okResult.Value);
            Assert.Equal(group.Name, returnValue.Name);
        }

        [Fact]
        public async Task Delete_DeletesGroup()
        {
            var db = GetDatabaseContext();
            var group = new Group { Name = "Group A" };
            db.Groups.Add(group);
            await db.SaveChangesAsync();

            var controller = new GroupsController(db);

            var result = await controller.Delete(group.Id);

            Assert.IsType<NoContentResult>(result);
            Assert.Null(await db.Groups.FindAsync(group.Id));
        }

        [Fact]
        public async Task Create_ReturnsNotFound_WhenCourseDoesNotExist()
        {
            var db = GetDatabaseContext();
            var controller = new GroupsController(db);

            var result = await controller.Create(new CreateGroupDTo { Name = "Group A", CourseId = 999 });

            Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Empty(db.Groups);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenTeacherDoesNotExist()
        {
            var db = GetDatabaseContext();
            var course = new Course { Title = "Course" };
            var group = new Group { Name = "Group A", Course = course };
            db.Groups.Add(group);
            await db.SaveChangesAsync();
            var controller = new GroupsController(db);
            var dto = new UpdateGroupDTo { Name = "Group B", CourseId = course.Id, TeacherId = 999 };

            var result = await controller.Update(group.Id, dto);

            Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Null(group.TeacherId);
        }

        [Fact]
        public async Task Delete_ReturnsConflict_WhenGroupContainsStudents()
        {
            var db = GetDatabaseContext();
            var group = new Group { Name = "Group A" };
            db.Groups.Add(group);
            await db.SaveChangesAsync();
            db.Students.Add(new Student { Name = "Alice", GroupId = group.Id });
            await db.SaveChangesAsync();

            var result = await new GroupsController(db).Delete(group.Id);

            Assert.IsType<ConflictObjectResult>(result);
            Assert.NotNull(await db.Groups.FindAsync(group.Id));
        }
    }
}
