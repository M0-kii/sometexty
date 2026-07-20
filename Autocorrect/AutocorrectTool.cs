using System.Windows.Controls;
using SomeTexty.Tools;

namespace SomeTexty.Autocorrect;

/// <summary>Offline spelling checker with suggestions. The panel's first real tool.</summary>
public sealed class AutocorrectTool : ITextTool
{
    public string Name => "Autocorrect";

    public UserControl CreateView() => new AutocorrectView();
}
