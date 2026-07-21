using System.Windows.Controls;
using SomeTexty.Tools;

namespace SomeTexty.Clean;

/// <summary>Whitespace and line clean-up.</summary>
public sealed class CleanTool : ITextTool
{
    public string Name => "Clean";

    public UserControl CreateView() => new CleanView();
}
