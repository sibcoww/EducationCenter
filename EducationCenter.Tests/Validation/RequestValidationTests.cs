using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using EducationCenter.Api;
using EducationCenter.Api.Data;
using EducationCenter.Api.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EducationCenter.Tests.Validation;

public class RequestValidationTests
{
    public static IEnumerable<object[]> InvalidRequests()
    {
        foreach (var resource in new[] { "courses", "subjects", "groups", "students", "teachers" })
        foreach (var method in new[] { "POST", "PUT" })
        {
            var nameField = resource is "courses" or "subjects" ? "Title" : "Name";
            foreach (var invalidName in new string?[] { null, "", " \t\n" })
            {
                var body = ValidBody(resource);
                body[nameField] = invalidName;
                yield return [resource, method, body, nameField];
            }

            var missingName = ValidBody(resource);
            missingName.Remove(nameField);
            yield return [resource, method, missingName, nameField];

            if (resource == "courses")
            {
                var body = ValidBody(resource);
                body["Price"] = -0.01m;
                yield return [resource, method, body, "Price"];
            }

            if (resource is "students" or "teachers")
            {
                foreach (var invalidDate in new string?[] { null, "0001-01-01", "9999-12-31" })
                {
                    var body = ValidBody(resource);
                    body["BirthDate"] = invalidDate;
                    yield return [resource, method, body, "BirthDate"];
                }

                var missingDate = ValidBody(resource);
                missingDate.Remove("BirthDate");
                yield return [resource, method, missingDate, "BirthDate"];
            }
        }
    }

    [Theory]
    [MemberData(nameof(InvalidRequests))]
    public async Task Invalid_request_returns_validation_problem_without_changing_data(
        string resource, string method, Dictionary<string, object?> body, string field)
    {
        using var factory = new TestApiFactory();
        using var client = factory.CreateClient(new() { BaseAddress = new Uri("https://localhost") });
        using var created = await client.PostAsJsonAsync($"/api/{resource}", ValidBody(resource));
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var entity = await created.Content.ReadFromJsonAsync<JsonElement>();
        var id = entity.GetProperty("id").GetInt32();
        var before = await client.GetStringAsync($"/api/{resource}");

        using var request = new HttpRequestMessage(new HttpMethod(method),
            method == "POST" ? $"/api/{resource}" : $"/api/{resource}/{id}")
        {
            Content = JsonContent.Create(body)
        };
        using var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var problem = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Contains(problem.GetProperty("errors").EnumerateObject(),
            error => error.Name == field || error.Name == $"$.{field}");
        Assert.Equal(before, await client.GetStringAsync($"/api/{resource}"));
    }

    [Theory]
    [InlineData("courses")]
    [InlineData("subjects")]
    [InlineData("groups")]
    [InlineData("students")]
    [InlineData("teachers")]
    public async Task Valid_requests_accept_zero_price_and_birth_date_today(string resource)
    {
        using var factory = new TestApiFactory();
        using var client = factory.CreateClient(new() { BaseAddress = new Uri("https://localhost") });
        var body = ValidBody(resource);
        body["Price"] = 0m;
        body["BirthDate"] = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd");
        using var created = await client.PostAsJsonAsync($"/api/{resource}", body);
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var entity = await created.Content.ReadFromJsonAsync<JsonElement>();
        var id = entity.GetProperty("id").GetInt32();

        using var updated = await client.PutAsJsonAsync($"/api/{resource}/{id}", body);
        Assert.Equal(HttpStatusCode.OK, updated.StatusCode);
    }

    private static Dictionary<string, object?> ValidBody(string resource) => resource switch
    {
        "courses" => new() { ["Title"] = "Mathematics", ["Price"] = 100m },
        "subjects" => new() { ["Title"] = "Algebra" },
        "groups" => new() { ["Name"] = "Group A", ["CourseId"] = 1 },
        _ => new() { ["Name"] = "Alice", ["BirthDate"] = "2000-01-01" }
    };

}
