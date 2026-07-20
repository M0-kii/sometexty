using System.Windows.Controls;

namespace SomeTexty.Tools;

/// <summary>
/// A single tool hosted in the flyout panel. Each tool contributes a tab and
/// the view shown when that tab is active. Adding a feature means implementing
/// this and registering it in <see cref="ToolRegistry"/>; the panel needs no changes.
/// </summary>
public interface ITextTool
{
    /// <summary>Label shown on the tool's tab.</summary>
    string Name { get; }

    /// <summary>Builds the tool's view. Called once; the panel caches the result.</summary>
    UserControl CreateView();
}
