using System.Windows.Controls;
using SomeTexty.Tools;

namespace SomeTexty.Autocorrect;

/// <summary>Offline spelling checker with suggestions.</summary>
public sealed class AutocorrectTool : ITextTool
{
    public string Name => "Autocorrect";

    public UserControl CreateView() => new AutocorrectView();
}
