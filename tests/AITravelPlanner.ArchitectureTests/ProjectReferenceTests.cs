using System.Xml.Linq;

namespace AITravelPlanner.ArchitectureTests;

public class ProjectReferenceTests
{
    [Theory]
    [InlineData("Domain")]
    [InlineData("Application", "AITravelPlanner.Domain")]
    [InlineData("Infrastructure", "AITravelPlanner.Application", "AITravelPlanner.Domain")]
    [InlineData("WebApi", "AITravelPlanner.Application", "AITravelPlanner.Infrastructure")]
    public void Project_references_match_architecture(string projectName, params string[] expectedReferences)
    {
        var repositoryRoot = FindRepositoryRoot();
        var projectPath = Path.Combine(
            repositoryRoot,
            "src",
            projectName,
            $"AITravelPlanner.{projectName}.csproj");
        var project = XDocument.Load(projectPath);

        var actualReferences = project
            .Descendants("ProjectReference")
            .Select(reference =>
            {
                var include = reference.Attribute("Include")?.Value;
                Assert.False(string.IsNullOrWhiteSpace(include));

                return Path.GetFileNameWithoutExtension(include!.Replace('\\', '/'));
            })
            .OrderBy(reference => reference, StringComparer.Ordinal);

        Assert.Equal(
            expectedReferences.OrderBy(reference => reference, StringComparer.Ordinal),
            actualReferences);
    }

    private static string FindRepositoryRoot()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory);
             directory is not null;
             directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "AITravelPlanner.sln")))
            {
                return directory.FullName;
            }
        }

        throw new DirectoryNotFoundException("Could not locate AITravelPlanner.sln.");
    }
}
