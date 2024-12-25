// See https://aka.ms/new-console-template for more information

using EnumExtensionsTest;
using SourceGenerator;

var one = BaseEnum.One;
var flagged =one.AddFlag(BaseEnum.Three);
Console.WriteLine(flagged.ContainsFlag(BaseEnum.Two));
Console.WriteLine(flagged.ContainsFlag(BaseEnum.Three));
flagged = flagged.RemoveFlag(BaseEnum.Three);
flagged.AddFlag(BaseEnum.SomeOther);
Console.WriteLine(flagged.ContainsFlag(BaseEnum.Three));
var enumValues = Enum.GetValues<BaseEnum>();
foreach (var @enum in enumValues)
{
    Console.WriteLine($"{@enum.GetName()} => {@enum.GetDescription()}");
}




[EnumExtensions]
[Flags]
public enum BaseEnum:byte
{
    One,
    Two,
    Three,
    [EnumName("Four")]
    [EnumDescription("Basically when this is returned it means that the Fourth option applies")]
    SomeOther,
    End
}