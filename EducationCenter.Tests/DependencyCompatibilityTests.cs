using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using EducationCenter.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace EducationCenter.Tests;

public class DependencyCompatibilityTests
{
    [Fact]
    public async Task Development_exposes_openapi_document_and_swagger_ui()
    {
        using var factory = new TestApiFactory();
        using var developmentFactory = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Development"));
        using var client = developmentFactory.CreateClient(new() { BaseAddress = new Uri("https://localhost") });

        using var response = await client.GetAsync("/openapi/v1.json");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var document = await response.Content.ReadFromJsonAsync<JsonElement>();
        var paths = document.GetProperty("paths");
        foreach (var resource in new[] { "courses", "subjects", "teachers", "groups", "students" })
        {
            Assert.True(paths.GetProperty($"/api/{resource}").TryGetProperty("post", out _));
            Assert.True(paths.GetProperty($"/api/{resource}/{{id}}").TryGetProperty("put", out _));
        }

        using var swagger = await client.GetAsync("/swagger/index.html");
        Assert.Equal(HttpStatusCode.OK, swagger.StatusCode);
        Assert.Contains("Swagger UI", await swagger.Content.ReadAsStringAsync());
    }

    [Fact]
    public void PostgreSql_provider_generates_queries_with_relationships()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=localhost;Database=compatibility_check")
            .Options;
        using var db = new AppDbContext(options);

        // Translation exercises the relational provider without connecting to a database.
        var sql = db.Groups
            .Include(group => group.Course)
            .Include(group => group.Subject)
            .Include(group => group.Teacher)
            .Include(group => group.Students)
            .Where(group => group.Id == 1)
            .ToQueryString();

        foreach (var table in new[] { "Groups", "Courses", "Subjects", "Teachers", "Students" })
        {
            Assert.Contains($"\"{table}\"", sql);
        }
    }
}
