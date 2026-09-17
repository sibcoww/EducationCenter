using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using EducationCenter.Api.Data;
using EducationCenter.Api.Models;
using Microsoft.Extensions.DependencyInjection;

namespace EducationCenter.Tests.Controllers;

public class UpdateResponseTests
{
    [Theory]
    [InlineData("courses", "subjectTitles", "Algebra", "groupNames", "Group A")]
    [InlineData("subjects", "teacherNames", "Teacher A", "courseNames", "Existing course")]
    [InlineData("teachers", "subjectTitles", "Algebra", "groupNames", "Group A")]
    [InlineData("groups", "studentNames", "Student A", "studentNames", "Student A")]
    public async Task Put_returns_existing_relationships_and_matches_get(
        string resource, string firstCollection, string firstName, string secondCollection, string secondName)
    {
        using var factory = new TestApiFactory();
        using var client = factory.CreateClient(new() { BaseAddress = new Uri("https://localhost") });
        SeedRelationships(factory);

        object body = resource switch
        {
            "courses" => new { Title = "Updated course", Description = "New description", Price = 250m },
            "subjects" => new { Title = "Updated subject", Description = "New description" },
            "teachers" => new { Name = "Updated teacher", BirthDate = new DateOnly(1990, 2, 3) },
            _ => new { Name = "Updated group", CourseId = 1, SubjectId = 1, TeacherId = 1 }
        };

        using var response = await client.PutAsJsonAsync($"/api/{resource}/1", body);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(updated);
        Assert.Equal(firstName, Assert.Single(updated[firstCollection]!.AsArray())!.GetValue<string>());
        Assert.Equal(secondName, Assert.Single(updated[secondCollection]!.AsArray())!.GetValue<string>());
        Assert.Equal($"Updated {resource switch { "courses" => "course", "subjects" => "subject", "teachers" => "teacher", _ => "group" }}",
            updated[resource is "courses" or "subjects" ? "title" : "name"]!.GetValue<string>());

        var fetched = await client.GetFromJsonAsync<JsonObject>($"/api/{resource}/1");
        Assert.True(JsonNode.DeepEquals(updated, fetched));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Group_put_returns_changed_or_cleared_references_and_preserves_students(bool clearReferences)
    {
        using var factory = new TestApiFactory();
        using var client = factory.CreateClient(new() { BaseAddress = new Uri("https://localhost") });
        SeedRelationships(factory);
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Courses.Add(new Course { Id = 2, Title = "New course" });
            db.Subjects.Add(new Subject { Id = 2, Title = "New subject" });
            db.Teachers.Add(new Teacher { Id = 2, Name = "New teacher", BirthDate = new DateOnly(1990, 1, 1) });
            db.SaveChanges();
        }

        using var response = await client.PutAsJsonAsync("/api/groups/1", new
        {
            Name = "Updated group", CourseId = 2,
            SubjectId = clearReferences ? (int?)null : 2,
            TeacherId = clearReferences ? (int?)null : 2
        });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(updated);
        Assert.Equal(2, updated["courseId"]!.GetValue<int>());
        Assert.Equal("New course", updated["courseTitle"]!.GetValue<string>());
        Assert.Equal(clearReferences ? null : "New subject", updated["subjectTitle"]?.GetValue<string>());
        Assert.Equal(clearReferences ? null : "New teacher", updated["teacherName"]?.GetValue<string>());
        Assert.Equal(clearReferences ? (int?)null : 2, updated["subjectId"]?.GetValue<int>());
        Assert.Equal(clearReferences ? (int?)null : 2, updated["teacherId"]?.GetValue<int>());
        Assert.Equal("Student A", Assert.Single(updated["studentNames"]!.AsArray())!.GetValue<string>());
        Assert.True(JsonNode.DeepEquals(updated, await client.GetFromJsonAsync<JsonObject>("/api/groups/1")));
    }

    private static void SeedRelationships(TestApiFactory factory)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var course = db.Courses.Single();
        var teacher = new Teacher { Id = 1, Name = "Teacher A", BirthDate = new DateOnly(1980, 1, 1) };
        var subject = new Subject { Id = 1, Title = "Algebra", Courses = [course], Teachers = [teacher] };
        var group = new Group { Id = 1, Name = "Group A", Course = course, Subject = subject, Teacher = teacher };
        db.Students.Add(new Student { Name = "Student A", BirthDate = new DateOnly(2000, 1, 1), Group = group });
        db.SaveChanges();
    }
}
