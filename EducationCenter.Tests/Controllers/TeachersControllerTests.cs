using EducationCenter.Api.Controllers;
using EducationCenter.Api.Data;
using EducationCenter.Api.DTOs.Teachers;
using EducationCenter.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EducationCenter.Tests.Controllers
{
    public class TeachersControllerTests
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
        public async Task Create_ReturnsCreatedTeacher()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new TeachersController(dbContext);
            var dto = new CreateTeacherDTo { Name = "John Doe", BirthDate = new DateOnly(1980, 1, 1) };

            // Act
            var actionResult = await controller.Create(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var resultValue = Assert.IsType<TeacherDTo>(createdResult.Value);

            Assert.Equal(dto.Name, resultValue.Name);
            Assert.Equal(dto.BirthDate, resultValue.BirthDate);
        }

        [Fact]
        public async Task GetById_ReturnsTeacher_WhenExists()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var teacher = new Teacher { Name = "John Doe", BirthDate = new DateOnly(1980, 1, 1) };
            dbContext.Teachers.Add(teacher);
            await dbContext.SaveChangesAsync();

            var controller = new TeachersController(dbContext);

            // Act
            var actionResult = await controller.GetById(teacher.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var resultValue = Assert.IsType<TeacherDTo>(okResult.Value);

            Assert.Equal(teacher.Id, resultValue.Id);
            Assert.Equal(teacher.Name, resultValue.Name);
        }

        [Fact]
        public async Task AddSubjectToTeacher_AddsSuccessfully()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var teacher = new Teacher { Name = "John" };
            var subject = new Subject { Title = "Math" };
            dbContext.Teachers.Add(teacher);
            dbContext.Subjects.Add(subject);
            await dbContext.SaveChangesAsync();

            var controller = new TeachersController(dbContext);

            // Act
            var actionResult = await controller.AddSubjectToTeacher(teacher.Id, subject.Id);

            // Assert
            Assert.IsType<NoContentResult>(actionResult);

            var dbTeacher = await dbContext.Teachers.Include(t => t.Subjects).FirstOrDefaultAsync(t => t.Id == teacher.Id);
            Assert.Single(dbTeacher.Subjects);
            Assert.Equal(subject.Id, dbTeacher.Subjects.First().Id);
        }

        [Fact]
        public async Task AddSubjectToTeacher_ReturnsConflict_WhenAlreadyAdded()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var subject = new Subject { Title = "Math" };
            var teacher = new Teacher { Name = "John", Subjects = [subject] };
            dbContext.Teachers.Add(teacher);
            await dbContext.SaveChangesAsync();

            var controller = new TeachersController(dbContext);

            // Act
            var actionResult = await controller.AddSubjectToTeacher(teacher.Id, subject.Id);

            // Assert
            Assert.IsType<ConflictResult>(actionResult);
        }
    }
}