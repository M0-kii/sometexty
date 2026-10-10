using SomeTexty.Autocorrect;
using SomeTexty.CaseConversion;
using SomeTexty.Clean;
using SomeTexty.Count;
using SomeTexty.Replace;

namespace SomeTexty.Tools;

/// <summary>
/// Tools in tab order.
/// </summary>
public static class ToolRegistry
{
    public static IReadOnlyList<ITextTool> Tools { get; } = new ITextTool[]
    {
        new AutocorrectTool(),
        new CaseTool(),
        new CountTool(),
        new CleanTool(),
        new ReplaceTool(),
    };
}
