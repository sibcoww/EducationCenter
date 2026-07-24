using EducationCenter.Api.Controllers;
using EducationCenter.Api.Data;
using EducationCenter.Api.DTOs.Students;
using EducationCenter.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace EducationCenter.Tests.Controllers
{
    public class StudentsControllerTests
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
        public async Task Create_ReturnsCreatedStudent()
        {
            var db = GetDatabaseContext();
            var controller = new StudentsController(db);
            var dto = new CreateStudentDTo { Name = "Alice", BirthDate = new DateOnly(2000, 1, 1) };

            var result = await controller.Create(dto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<StudentDTo>(createdResult.Value);
            Assert.Equal(dto.Name, returnValue.Name);
        }

        [Fact]
        public async Task GetById_ReturnsStudent_WhenExists()
        {
            var db = GetDatabaseContext();
            var student = new Student { Name = "Alice", BirthDate = new DateOnly(2000, 1, 1) };
            db.Students.Add(student);
            await db.SaveChangesAsync();

            var controller = new StudentsController(db);

            var result = await controller.GetById(student.Id);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<StudentDTo>(okResult.Value);
            Assert.Equal(student.Name, returnValue.Name);
        }

        [Fact]
        public async Task Delete_DeletesStudent()
        {
            var db = GetDatabaseContext();
            var student = new Student { Name = "Alice", BirthDate = new DateOnly(2000, 1, 1) };
            db.Students.Add(student);
            await db.SaveChangesAsync();

            var controller = new StudentsController(db);

            var result = await controller.Delete(student.Id);

            Assert.IsType<NoContentResult>(result);
            Assert.Null(await db.Students.FindAsync(student.Id));
        }
    }
}