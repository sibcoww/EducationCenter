using EducationCenter.Api.Controllers;
using EducationCenter.Api.Data;
using EducationCenter.Api.DTOs.Subjects;
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
    public class SubjectsControllerTests
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
        public async Task Create_ReturnsCreatedSubject()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new SubjectsController(dbContext);
            var dto = new CreateSubjectDTo { Title = "Math", Description = "Advanced Math" };

            // Act
            var actionResult = await controller.Create(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var resultValue = Assert.IsType<SubjectDTo>(createdResult.Value);

            Assert.Equal(dto.Title, resultValue.Title);
            Assert.Equal(dto.Description, resultValue.Description);
            Assert.True(resultValue.Id > 0);
        }

        [Fact]
        public async Task Get_ReturnsAllSubjects()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            dbContext.Subjects.Add(new Subject { Title = "Math" });
            dbContext.Subjects.Add(new Subject { Title = "Physics" });
            await dbContext.SaveChangesAsync();

            var controller = new SubjectsController(dbContext);

            // Act
            var actionResult = await controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var resultValue = Assert.IsType<List<SubjectDTo>>(okResult.Value);

            Assert.Equal(2, resultValue.Count);
        }

        [Fact]
        public async Task GetById_ReturnsSubject_WhenItExists()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var subject = new Subject { Title = "Math" };
            dbContext.Subjects.Add(subject);
            await dbContext.SaveChangesAsync();

            var controller = new SubjectsController(dbContext);

            // Act
            var actionResult = await controller.GetById(subject.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var resultValue = Assert.IsType<SubjectDTo>(okResult.Value);

            Assert.Equal(subject.Id, resultValue.Id);
            Assert.Equal(subject.Title, resultValue.Title);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenItDoesNotExist()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var controller = new SubjectsController(dbContext);

            // Act
            var actionResult = await controller.GetById(999);

            // Assert
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task Update_UpdatesSubject_WhenItExists()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var subject = new Subject { Title = "Math", Description = "A" };
            dbContext.Subjects.Add(subject);
            await dbContext.SaveChangesAsync();

            var controller = new SubjectsController(dbContext);
            var updateDto = new UpdateSubjectDTo { Title = "Algebra", Description = "B" };

            // Act
            var actionResult = await controller.Update(subject.Id, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var resultValue = Assert.IsType<SubjectDTo>(okResult.Value);

            Assert.Equal(updateDto.Title, resultValue.Title);
            Assert.Equal(updateDto.Description, resultValue.Description);

            var dbSubject = await dbContext.Subjects.SingleAsync(s => s.Id == subject.Id);
            Assert.Equal(updateDto.Title, dbSubject.Title);
        }

        [Fact]
        public async Task Delete_DeletesSubject_WhenItExists()
        {
            // Arrange
            var dbContext = GetDatabaseContext();
            var subject = new Subject { Title = "Math" };
            dbContext.Subjects.Add(subject);
            await dbContext.SaveChangesAsync();

            var controller = new SubjectsController(dbContext);

            // Act
            var actionResult = await controller.Delete(subject.Id);

            // Assert
            Assert.IsType<NoContentResult>(actionResult);
            var dbSubject = await dbContext.Subjects.FindAsync(subject.Id);
            Assert.Null(dbSubject);
        }

        [Fact]
        public async Task RemoveTeacherFromSubject_RemovesOnlyRelationship()
        {
            var dbContext = GetDatabaseContext();
            var teacher = new Teacher { Name = "John" };
            var subject = new Subject { Title = "Math", Teachers = [teacher] };
            dbContext.Subjects.Add(subject);
            await dbContext.SaveChangesAsync();

            var result = await new SubjectsController(dbContext)
                .RemoveTeacherFromSubject(subject.Id, teacher.Id);

            Assert.IsType<NoContentResult>(result);
            Assert.NotNull(await dbContext.Subjects.FindAsync(subject.Id));
            Assert.NotNull(await dbContext.Teachers.FindAsync(teacher.Id));
            var savedSubject = await dbContext.Subjects.Include(s => s.Teachers)
                .SingleAsync(s => s.Id == subject.Id);
            Assert.Empty(savedSubject.Teachers);
        }

        [Fact]
        public async Task Delete_ReturnsConflict_WhenSubjectIsUsedByGroup()
        {
            var dbContext = GetDatabaseContext();
            var subject = new Subject { Title = "Math" };
            dbContext.Subjects.Add(subject);
            await dbContext.SaveChangesAsync();
            dbContext.Groups.Add(new Group { Name = "Group A", SubjectId = subject.Id });
            await dbContext.SaveChangesAsync();

            var result = await new SubjectsController(dbContext).Delete(subject.Id);

            Assert.IsType<ConflictObjectResult>(result);
            Assert.NotNull(await dbContext.Subjects.FindAsync(subject.Id));
        }
    }
}
