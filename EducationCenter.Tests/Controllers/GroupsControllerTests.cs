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
    }
}