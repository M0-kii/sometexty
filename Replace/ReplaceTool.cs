using System.Windows.Controls;
using SomeTexty.Tools;

namespace SomeTexty.Replace;

/// <summary>Plain find-and-replace across the text.</summary>
public sealed class ReplaceTool : ITextTool
{
    public string Name => "Replace";

    public UserControl CreateView() => new ReplaceView();
}
