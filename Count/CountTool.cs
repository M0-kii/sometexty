using System.Windows.Controls;
using SomeTexty.Tools;

namespace SomeTexty.Count;

/// <summary>Live word, character, and line counts.</summary>
public sealed class CountTool : ITextTool
{
    public string Name => "Count";

    public UserControl CreateView() => new CountView();
}
