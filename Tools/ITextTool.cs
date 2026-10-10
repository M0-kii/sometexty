using System.Windows.Controls;

namespace SomeTexty.Tools;

/// <summary>
/// A tab and its view. Register new tools in <see cref="ToolRegistry"/>.
/// </summary>
public interface ITextTool
{
    /// <summary>Label shown on the tool's tab.</summary>
    string Name { get; }

    /// <summary>Builds the tool's view. Called once; the panel caches the result.</summary>
    UserControl CreateView();
}
