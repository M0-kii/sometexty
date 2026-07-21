using System.Windows.Controls;
using SomeTexty.Tools;

namespace SomeTexty.CaseConversion;

/// <summary>Converts text between upper, lower, title, and sentence case.</summary>
public sealed class CaseTool : ITextTool
{
    public string Name => "Case";

    public UserControl CreateView() => new CaseView();
}
