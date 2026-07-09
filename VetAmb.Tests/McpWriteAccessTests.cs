using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using VetAmb.Data;
using VetAmb.McpTools;
using VetAmb.Repositories;
using Xunit;

namespace VetAmb.Tests;

public class McpWriteAccessTests
{
    [Fact]
    public void CreateClinic_WhenReadOnlyModeEnabled_ThrowsUnauthorizedAccessException()
    {
        using var context = CreateDbContext();
        var tools = CreateClinicTools(
            context,
            new McpAccessOptions
            {
                Enabled = true,
                ReadOnlyByDefault = true,
                AllowWriteOperations = false
            });

        Assert.Throws<UnauthorizedAccessException>(() =>
            tools.CreateClinic("ReadOnly Clinic", "Address", "01 000 0000", "readonly@vetamb.hr", DateTime.UtcNow.Date, 10, "RO-1"));
    }

    [Fact]
    public void SearchClinics_WhenReadOnlyModeEnabled_ReturnsData()
    {
        using var context = CreateDbContext();
        var tools = CreateClinicTools(
            context,
            new McpAccessOptions
            {
                Enabled = true,
                ReadOnlyByDefault = true,
                AllowWriteOperations = false
            });

        var clinics = tools.SearchClinics(null);

        Assert.NotEmpty(clinics);
    }

    [Fact]
    public void CreateClinic_WhenRoleIsRequiredAndMissing_ThrowsUnauthorizedAccessException()
    {
        using var context = CreateDbContext();
        var user = CreatePrincipal(userId: "user-1", roles: ["Staff"]);
        var tools = CreateClinicTools(
            context,
            new McpAccessOptions
            {
                Enabled = true,
                ReadOnlyByDefault = true,
                AllowWriteOperations = true,
                RequireAuthenticatedUserForWrite = true,
                WriteRoles = ["Admin"]
            },
            user);

        Assert.Throws<UnauthorizedAccessException>(() =>
            tools.CreateClinic("Role Blocked Clinic", "Address", "01 000 1111", "blocked@vetamb.hr", DateTime.UtcNow.Date, 12, "RB-1"));
    }

    [Fact]
    public void CreateClinic_WhenRoleIsPresent_Succeeds()
    {
        using var context = CreateDbContext();
        var user = CreatePrincipal(userId: "admin-1", roles: ["Admin"]);
        var tools = CreateClinicTools(
            context,
            new McpAccessOptions
            {
                Enabled = true,
                ReadOnlyByDefault = true,
                AllowWriteOperations = true,
                RequireAuthenticatedUserForWrite = true,
                WriteRoles = ["Admin"]
            },
            user);

        var created = tools.CreateClinic(
            "Write Allowed Clinic",
            "Ilica 1",
            "01 111 2222",
            "allowed@vetamb.hr",
            DateTime.UtcNow.Date,
            20,
            "WA-1");

        Assert.True(created.Id > 0);
        Assert.Equal("Write Allowed Clinic", created.Name);
    }

    private static VetAmbDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<VetAmbDbContext>()
            .UseInMemoryDatabase($"VetAmbMcpTests_{Guid.NewGuid():N}")
            .Options;

        var context = new VetAmbDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    private static ClinicMcpTools CreateClinicTools(VetAmbDbContext context, McpAccessOptions options, ClaimsPrincipal? user = null)
    {
        var repository = new EfClinicRepository(context);
        var accessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                User = user ?? new ClaimsPrincipal(new ClaimsIdentity())
            }
        };

        var execution = new McpToolExecution(
            Options.Create(options),
            new TestHostEnvironment(),
            accessor,
            NullLogger<McpToolExecution>.Instance);

        return new ClinicMcpTools(repository, execution);
    }

    private static ClaimsPrincipal CreatePrincipal(string userId, string[] roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        var identity = new ClaimsIdentity(claims, authenticationType: "Test");
        return new ClaimsPrincipal(identity);
    }

    private sealed class TestHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = Environments.Development;
        public string ApplicationName { get; set; } = "VetAmb.Tests";
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; } = null!;
    }
}
