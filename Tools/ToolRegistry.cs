using SomeTexty.Autocorrect;
using SomeTexty.CaseConversion;
using SomeTexty.Clean;
using SomeTexty.Count;
using SomeTexty.Replace;

namespace SomeTexty.Tools;

/// <summary>
/// The set of tools the panel offers, in tab order. This is the one place to
/// register a new tool.
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
