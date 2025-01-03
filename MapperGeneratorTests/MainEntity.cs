using MapperGenerator;
using SourceGenerator;

namespace MapperGeneratorTests;

public class MainEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Hint { get; set; }
    public DateTimeOffset InsertTime { get; set; }
    public bool IsEnabled { get; set; }
}

[MapFrom<MainEntity>]
[MapTo(nameof(MainEntity))]
public class MainModel
{

    [ToMappingFunction("int.Parse")]
    [FromMappingExtensionFunction("ToString()")]
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Hint { get; init; }
    public DateTimeOffset InsertTime { get; init; }
    public bool IsEnabled { get; init; }
}

[EnumExtensions]
public enum First
{
    One,
    Two
}