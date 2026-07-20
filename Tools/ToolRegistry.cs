using SomeTexty.Autocorrect;

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
        new PlaceholderTool("Case", "Case tools are coming soon."),
    };
}
