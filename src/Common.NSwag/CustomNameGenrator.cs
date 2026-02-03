using NSwag;
using NSwag.CodeGeneration.OperationNameGenerators;
using Ploch.Common;

namespace Ploch.Lists.Api.WebApi;

public class CustomNameGenrator : IOperationNameGenerator
{
    public bool SupportsMultipleClients { get; }

    public string GetClientName(OpenApiDocument document, string path, string httpMethod, OpenApiOperation operation) => operation.Tags[0];

    public string GetOperationName(OpenApiDocument document, string path, string httpMethod, OpenApiOperation operation)
    {
        var name = httpMethod + operation.Tags[0];
        Uri uri = new(path, UriKind.Relative);
        var uriSegment = uri.Segments[uri.Segments.Length - 1];
        Console.WriteLine(uri);
        var targetName = name.ToPascalCase();

        Console.WriteLine($"Generated pascal case from {name} to {targetName}");

        return targetName;
    }
}
